using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using System;
using Random = UnityEngine.Random;
using System.Threading.Tasks;


public class BuffSystem
{
    private Dictionary<StatusEffectID, StatusEffectInstance> activeEffects = new Dictionary<StatusEffectID, StatusEffectInstance>();
    private Dictionary<StatType, TurnBuff> turnBuffs = new Dictionary<StatType, TurnBuff>();

    public class TurnBuff
    {
        public float value;
        public int remainingTurns;
        public StatSign sign;

        public TurnBuff(float _value, int _turns, StatSign _sign)
        {
            this.value = _value;
            this.remainingTurns = _turns;
            this.sign = _sign;
        }
    }

    public async Task OnTurnStart(BattleCharacterBase _target)
    {
        // 기존 상태이상 처리
        foreach (var t_effect in this.activeEffects.Values)
            await t_effect.Tick(_target);

        var t_expired = new List<StatusEffectID>();
        foreach (var t_pair in this.activeEffects)
        {
            if (t_pair.Value.IsExpired)
                t_expired.Add(t_pair.Key);
        }

        foreach (var t_key in t_expired)
            this.activeEffects.Remove(t_key);

        // 턴제 버프 처리
        var expiredBuffs = new List<StatType>();
        foreach (var buff in turnBuffs)
        {
            buff.Value.remainingTurns--;
            if (buff.Value.remainingTurns <= 0)
            {
                expiredBuffs.Add(buff.Key);
            }
        }

        foreach (var statType in expiredBuffs)
        {
            turnBuffs.Remove(statType);
        }
    }

    public async UniTask OnTurnStartAsync(BattleCharacterBase _target)
    {
        foreach (var t_effect in this.activeEffects.Values)
        {
            await t_effect.Tick(_target);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }


        var t_expired = new List<StatusEffectID>();
        foreach (var t_pair in this.activeEffects)
        {
            if (t_pair.Value.IsExpired)
                t_expired.Add(t_pair.Key);
        }

        foreach (var t_key in t_expired)
            this.activeEffects.Remove(t_key);
    }


    public bool Has(StatusEffectID _name) => this.activeEffects.ContainsKey(_name);
    public StatusEffectInstance Get(StatusEffectID _name) => this.activeEffects.TryGetValue(_name, out var t_effect) ? t_effect : null;

    public void Add(StatusEffectID _id)
    {
        if (this.activeEffects.ContainsKey(_id))
        {
            // 이미 존재하면 갱신하거나 중첩 처리
            this.activeEffects[_id].Refresh();
        }
        else
        {
            // 없으면 새로 추가
            this.activeEffects.Add(_id, new StatusEffectInstance(DataLibrary.instance.GetStateInfo(_id)));
        }
    }

    public void AddTurnBuff(StatType _type, float _value, int _turns, StatSign _sign)
    {
        if (turnBuffs.ContainsKey(_type))
        {
            // 이미 존재하는 버프가 있다면 갱신
            turnBuffs[_type] = new TurnBuff(_value, _turns, _sign);
        }
        else
        {
            // 새로운 버프 추가
            turnBuffs.Add(_type, new TurnBuff(_value, _turns, _sign));
        }
    }

    public float GetTurnBuffValue(StatType _type)
    {
        if (turnBuffs.TryGetValue(_type, out var buff))
        {
            return buff.value;
        }
        return 0f;
    }

    public StatSign GetTurnBuffSign(StatType _type)
    {
        if (turnBuffs.TryGetValue(_type, out var buff))
        {
            return buff.sign;
        }
        return StatSign.Constant;
    }

    public bool HasTurnBuff(StatType _type)
    {
        return turnBuffs.ContainsKey(_type);
    }
}
public class StatusEffectInstance
{
    public StatusEffectInfo info;
    public int remainingTurns;
    public int stack;

    public StatusEffectInstance(StatusEffectInfo _info)
    {
        this.info = _info;
        this.remainingTurns = _info.duration;
        this.stack = 1;
    }

    public void Refresh()
    {
        if (!this.info.isStackable)
        {
            this.remainingTurns = this.info.duration;
        }
        else if (this.stack < this.info.maxStack)
        {
            this.stack++;
            this.remainingTurns += this.info.duration;
        }
    }

    public async Task Tick(BattleCharacterBase _target)
    {
        await this.ApplyEffect(_target);
        this.remainingTurns--;
    }

    public async UniTask ApplyEffect(BattleCharacterBase _target)
    {
        switch (this.info.id)
        {
            // 매 턴 피해
            case StatusEffectID.Bleed:
            case StatusEffectID.Poison:
            case StatusEffectID.Corrosion:
                {
                    float t_rate = this.info.id == StatusEffectID.Bleed ? 0.05f :
                                   this.info.id == StatusEffectID.Poison ? 0.07f : 0.03f;
                    float t_dmg = Mathf.CeilToInt(_target.MaxHP() * t_rate);
                    await _target.HitTask(
                         new BattleCharacterBase.HitInfo()
                         {
                             hitDamage = t_dmg,
                             isCritical = false,
                             isRaceAdvantage = 0,
                             isDotDamage = true,
                             attackRace = RaceType.None,

                         });

                    Debug.Log($"{_target.name} takes {t_dmg} damage from {this.info.id}");
                    break;
                }

            // 행동 불가
            case StatusEffectID.Stun:
                _target.SetActionDisabled(true);
                break;

            // 명중률 감소
            case StatusEffectID.Blind:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Acc, StatSign.Constant, -30f));
                break;

            // 방어력/공격력 감소 (중첩 반영)
            case StatusEffectID.DEFDown:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Def, StatSign.Constant, -20f * this.stack));
                break;
            case StatusEffectID.ATKDown:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Atk, StatSign.Constant, -20f * this.stack));
                break;

            // 받는 피해 증가 (퍼센트 기반)
            case StatusEffectID.Frail:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.DamageReduce, StatSign.Constant, -20f));
                break;

            // 회복량 감소
            case StatusEffectID.HealBlock:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.HealEffecincy, StatSign.Percentage, -0.5f));
                break;

            // 명중률 + 회피율 감소
            case StatusEffectID.SensoryLoss:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Acc, StatSign.Constant, -20f));
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Evasion, StatSign.Constant, -20f));
                break;

            // ATK/DEF 감소 (턴마다 누적 약화)
            case StatusEffectID.Decay:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Atk, StatSign.Constant, -5f * this.stack));
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Def, StatSign.Constant, -5f * this.stack));
                break;

            // 회복 불가
            case StatusEffectID.HealDisable:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.HealEffecincy, StatSign.Percentage, 0f));
                break;

            // 크리티컬 확률 감소
            case StatusEffectID.FocusLoss:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.CriticalChance, StatSign.Constant, -30f));
                break;

            // 도망 불가
            case StatusEffectID.Bind:
                _target.SetCanEscape(false);
                break;

            // 혼란: 일정 확률로 자해
            case StatusEffectID.Confusion:
                if (Random.value < 0.35f)
                {
                    _target.ForceAttackSelf();
                    Debug.Log($"{_target.name} is confused and attacks self!");
                }
                break;

            // 스킬 사용 불가
            case StatusEffectID.MentalBreak:
                _target.SetSkillUseDisabled(true);
                break;

            // 공격력, 속도 감소
            case StatusEffectID.Lethargy:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Atk, StatSign.Percentage, 0.3f));
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Spd, StatSign.Percentage, 0.3f));
                break;

            // 저항력 약화: 디버프 지속 시간 증가
            case StatusEffectID.WeakResistance:
                _target.IncreaseDebuffDuration(1);
                break;

            // 분열: 자해 확률
            case StatusEffectID.Split:
                if (Random.value < 0.10f)
                {
                    float t_splitDmg = Mathf.CeilToInt(_target.MaxHP() * 0.05f);
                    await _target.TakeDamage(
                        new BattleCharacterBase.HitInfo()
                        {
                            hitDamage = t_splitDmg,
                            isCritical = false,
                            isRaceAdvantage = 0,
                            attackRace = RaceType.None,
                            isDotDamage = true,
                        });

                    Debug.Log($"{_target.name} is split and takes {t_splitDmg} self-damage");
                }
                break;

            // 자해: 50% 확률
            case StatusEffectID.SelfHarm:
                if (Random.value < 0.5f)
                {
                    _target.ForceAttackSelf();
                    Debug.Log($"{_target.name} harms itself!");
                }
                break;

            // 소멸 표식: 마지막 턴에 폭발
            case StatusEffectID.MarkOfDoom:
                if (this.remainingTurns <= 1)
                {
                    float t_explosion = Mathf.CeilToInt(_target.MaxHP() * 0.25f);
                    await _target.TakeDamage(
                   new BattleCharacterBase.HitInfo()
                   {
                       hitDamage = t_explosion,
                       isCritical = false,
                       isRaceAdvantage = 0,
                       attackRace = RaceType.None,
                       isDotDamage = true,
                   });
                    Debug.Log($"{_target.name} is hit by Mark of Doom explosion! {t_explosion} damage");
                }
                break;
            case StatusEffectID.Spended:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.DamageReduce, StatSign.Constant, -0.2f));
                break;
            case StatusEffectID.Exhaustion:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Atk, StatSign.Percentage, 0.3f));
                break;
            case StatusEffectID.CantAction:
                _target.AddStatBuff(this.info.id, new BuffBlock(StatType.Spd, StatSign.Percentage, 0.3f));
                break;
        }
    }


    public bool IsExpired => this.remainingTurns <= 0;
}

public class BuffBlock
{
    public BuffBlock(StatType _type, StatSign _sign, float _value)
    {
        this.type = _type;
        this.sign = _sign;
        this.value = _value;
    }
    StatType type;
    StatSign sign;
    float value;

    public StatType GetStatType() => this.type;
    public StatSign GetStatSign() => this.sign;
    public float GetStatValue() => this.value;
}

[System.Serializable]
public class StatBlock
{
    private Dictionary<StatType, float> baseStats = new Dictionary<StatType, float>();
    private Dictionary<StatusEffectID, BuffBlock> buffStats = new Dictionary<StatusEffectID, BuffBlock>();
    Dictionary<StatType, float> instnatStatBuff = new Dictionary<StatType, float>();
    private BuffSystem buffSystem;

    public StatBlock(BuffSystem _buffSystem)
    {
        this.buffSystem = _buffSystem;
        this.baseStats.Add(StatType.Hp, 100f);
        this.baseStats.Add(StatType.Atk, 10f);
        this.baseStats.Add(StatType.Def, 5f);
        this.baseStats.Add(StatType.Evasion, 5f);
        this.baseStats.Add(StatType.CriticalChance, 10f);
        this.baseStats.Add(StatType.Acc, 95f);
        this.baseStats.Add(StatType.Spd, 10f);
        this.baseStats.Add(StatType.HealEffecincy, 100f);
        this.baseStats.Add(StatType.DamageReduce, 0f);
    }

    public StatBlock(BattleStatus _status, BuffSystem _buffSystem)
    {
        this.buffSystem = _buffSystem;
        this.baseStats.Add(StatType.Hp, _status.GetHp());
        this.baseStats.Add(StatType.Mp, _status.GetMp());
        this.baseStats.Add(StatType.Atk, _status.GetAtk());
        this.baseStats.Add(StatType.Def, _status.GetDef());
        this.baseStats.Add(StatType.Spd, _status.GetSpd());
        this.baseStats.Add(StatType.Evasion, 5f);
        this.baseStats.Add(StatType.Acc, 95f);
        this.baseStats.Add(StatType.HealEffecincy, 100f);
        this.baseStats.Add(StatType.DamageReduce, 0f);
    }

    public void SetBase(StatType _type, float _value)
    {
        this.baseStats[_type] = _value;
    }

    public void AddBuff(StatusEffectID _id, BuffBlock _block)
    {
        if (!this.buffStats.ContainsKey(_id))
            this.buffStats.Add(_id, _block);
        else
            this.buffStats[_id] = _block;
    }
    public void AddInstantBuff(StatType _type, float _value)
    {
        if (instnatStatBuff.ContainsKey(_type))
            this.instnatStatBuff[_type] = _value;
        else
            this.instnatStatBuff.Add(_type, _value);
    }

    public void ClearBuff(StatusEffectID _type)
    {
        if (this.buffStats.ContainsKey(_type))
            this.buffStats.Remove(_type);
    }

    public void UpdateBaseStats(BattleStatus _status)
    {
        this.baseStats[StatType.Hp] = _status.GetHp();
        this.baseStats[StatType.Mp] = _status.GetMp();
        this.baseStats[StatType.Atk] = _status.GetAtk();
        this.baseStats[StatType.Def] = _status.GetDef();
        this.baseStats[StatType.Spd] = _status.GetSpd();
    }
    public float GetStat(StatType _type)
    {
        float t_base = this.baseStats.TryGetValue(_type, out var t_val) ? t_val : 0f;

        // 일반 버프 적용
        foreach (var t_buff in this.buffStats.Values)
        {
            if (t_buff.GetStatType() == _type)
            {
                switch (t_buff.GetStatSign())
                {
                    case StatSign.Constant:
                        t_base += t_buff.GetStatValue();
                        break;
                    case StatSign.Percentage:
                        t_base *= (1f + t_buff.GetStatValue());
                        break;
                }
            }
        }

        // 턴제 버프 적용
        if (buffSystem.HasTurnBuff(_type))
        {
            float turnBuffValue = buffSystem.GetTurnBuffValue(_type);
            StatSign turnBuffSign = buffSystem.GetTurnBuffSign(_type);

            switch (turnBuffSign)
            {
                case StatSign.Constant:
                    t_base += turnBuffValue;
                    break;
                case StatSign.Percentage:
                    t_base *= (1f + turnBuffValue);
                    break;
            }
        }

        return (int)t_base;
    }
    public float GetBase(StatType _type) => this.baseStats.TryGetValue(_type, out var t_val) ? t_val : 0f;
    public BuffBlock GetBuff(StatusEffectID _type) => this.buffStats.TryGetValue(_type, out var t_val) ? t_val : null;
}
public enum StatType
{
    None = 0,
    Hp = 1,
    Mp = 10,
    Atk = 2,
    Def = 3,
    Evasion = 4,
    Acc = 5,
    Spd = 6,
    HealEffecincy = 7,
    CriticalChance = 8,
    DamageReduce = 9,
}
public enum StatSign
{
    Constant,
    Percentage
}
