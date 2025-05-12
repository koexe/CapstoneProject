using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

public class BattleCharacterBase : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] BattleManager battleManager;
    [SerializeField] SpriteRenderer skin;
    [SerializeField] Animator animator;
    [SerializeField] SOSkillBase[] skills;
    [SerializeField] SOSkillBase selectedSkill;
    [SerializeField] BuffSystem buffSystem;
    [SerializeField] TextMeshPro hpText;

    [Header("정보")]
    [SerializeField] float maxHP;
    [SerializeField] float currentHP;
    [SerializeField] StatBlock statBlock;
    [SerializeField] int isUsedDefence;
    [SerializeField] bool isInDefence;
    [SerializeField] bool isActionDone = false;
    [SerializeField] bool isDie;
    [SerializeField] protected bool isActionDisabled;
    [SerializeField] CharacterActionType currentAction;
    public RaceType raceType;

    public float MaxHP() => this.maxHP;
    public void SetActionDisabled(bool _isActionDisabled) => this.isActionDisabled = _isActionDisabled;
    public void SetAction(CharacterActionType _action) => this.currentAction = _action;
    public CharacterActionType GetAction() => this.currentAction;

    public bool IsDie() => this.isDie;
    public void SetActionDone(bool _is) => this.isActionDone = _is;

    public bool IsActionDone() => this.isActionDone;
    public SOSkillBase[] GetSkills() => this.skills;


    public void RecalculateMaxHP()
    {
        this.maxHP = this.statBlock.GetStat(StatType.Hp);

        // 비율 유지
        float t_ratio = Mathf.Clamp01(this.currentHP / this.maxHP);
        this.currentHP = this.maxHP * t_ratio;
    }

    public async UniTask Summary()
    {
        await OnTurnStart();
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
    }

    public void Heal(float _amount)
    {
        this.currentHP = Mathf.Min(this.maxHP, this.currentHP + _amount);
    }

    public void Initialization(BattleManager _battleManager, SOBattleCharacter _character)
    {
        this.battleManager = _battleManager;
        this.buffSystem = new BuffSystem();
        InitStats(_character.GetStatus());
        this.skills = new SOSkillBase[_character.GetSkills().Length];
        for (int i = 0; i < skills.Length; i++)
            this.skills[i] = Instantiate(_character.GetSkills()[i]);

        this.raceType = _character.GetRaceType();
        this.hpText.text = this.currentHP.ToString();
        return;
    }
    public void SetSelectedSkill(SOSkillBase _skill, BattleCharacterBase[] _target)
    {
        this.selectedSkill = Instantiate(_skill);
        this.selectedSkill.character = this;
        this.selectedSkill.target = _target;
    }
    public void ResetSelectedSkill()
    {
        if (this.selectedSkill != null)
        {
            Destroy(this.selectedSkill);
            this.selectedSkill = null;
        }
    }

    public bool IsReady()
    {
        switch (this.currentAction)
        {
            case CharacterActionType.Attack:
                if (this.selectedSkill == null) return false;
                else if (this.selectedSkill.target == null) return false;
                else return true;
            case CharacterActionType.Defence:
                return true;
            case CharacterActionType.Skill:
                if (this.selectedSkill == null) return false;
                else if (this.selectedSkill.target == null) return false;
                else return true;
            case CharacterActionType.Talk:
                return false;
            case CharacterActionType.Run:
                return false;
            case CharacterActionType.Item:
                return false;
            case CharacterActionType.None:
                return false;
            default: return false;
        }
    }

    public SOSkillBase GetSelectedSkill()
    {
        return this.selectedSkill;
    }

    public async UniTask StartAction()
    {
        switch (this.currentAction)
        {
            case CharacterActionType.Attack:
                await this.selectedSkill.Execute();
                break;
            case CharacterActionType.Defence:
                await DefenceTask();
                break;
            case CharacterActionType.Skill:
                await this.selectedSkill.Execute();
                break;
            case CharacterActionType.Item:
                LogUtil.Log("아이템 사용 예정");
                break;
            case CharacterActionType.Run:
                LogUtil.Log("도망 구현 예정");
                break;
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} has died.");
        this.isDie = true;
    }

    public async UniTask OnTurnStart()
    {
        await this.buffSystem.OnTurnStartAsync(this);
        if (!this.isInDefence)
            this.isUsedDefence = 0;
        this.isInDefence = false;

    }
    public float GetStat(StatType _type)
    {
        if (_type == StatType.Hp)
            return this.maxHP; // max 기준으로 리턴
        return this.statBlock.GetStat(_type);
    }

    #region 턴 처리
    public async UniTask AttackTask(HitInfo _hitInfo)
    {
        this.battleManager.ShowText($"{this.name}의 {this.selectedSkill.skillName} 공격!!");
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        await _hitInfo.target.HitTask(_hitInfo);
        this.battleManager.ShowText($"다음차례!");
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
    }


    public void TakeDamage(HitInfo _hitInfo)
    {
        float t_finalDamage = _hitInfo.hitDamage;
        if (this.isInDefence)
        {
            if (_hitInfo.isRaceAdvantage == 1)
            {
                t_finalDamage *= 0.5f;
            }
            else
            {
                t_finalDamage *= 0.75f;
            }
        }


        this.battleManager.ShowText($"{(int)t_finalDamage}의 데미지를 {this.name} 이 받았다!!");
        this.currentHP = Mathf.Max(0f, this.currentHP - (int)t_finalDamage);
        this.hpText.text = this.currentHP.ToString();
        if (this.currentHP <= 0f)
            Die();
    }

    public async UniTask HitTask(HitInfo _hitInfo)
    {
        TakeDamage(_hitInfo);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        if (_hitInfo.statusEffect != StatusEffectID.None)
        {
            this.buffSystem.Add(_hitInfo.statusEffect);
            this.battleManager.ShowText($"{this.name}이  {_hitInfo.statusEffect} 에 걸렸다!!");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
    }

    public async UniTask DefenceTask()
    {
        this.battleManager.ShowText($"{this.name} 이 방어를 시작했다!");

        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        if (this.isUsedDefence == 0)
        {
            this.isInDefence = true;
            this.isUsedDefence += 1;
        }
        else
        {
            if (0.2f * this.isUsedDefence < Random.Range(0f, 1f))
            {
                this.isInDefence = true;
                this.isUsedDefence += 1;
            }
            else
            {
                this.isInDefence = false;
                this.isUsedDefence = 0;
                this.battleManager.ShowText($"하지만 실패했다!");
            }
        }
    }

    #endregion

    #region 상태이상 대응 메서드


    public void SetCanEscape(bool _value)
    {
        Debug.Log($"{this.name} escape ability: {_value}");
        // 필요 시 도망 가능 여부 플래그 추가
    }

    public void SetSkillUseDisabled(bool _value)
    {
        Debug.Log($"{this.name} skill use disabled: {_value}");
        // 필요 시 스킬 사용 제한 변수 추가
    }

    public void IncreaseDebuffDuration(int _turns)
    {
        Debug.Log($"{this.name} will receive longer debuffs (+{_turns})");
        // BuffSystem에서 적용되도록 연동 필요
    }

    public void ForceAttackSelf()
    {
        Debug.Log($"{this.name} attacks itself! (implement attack logic)");
        // 직접 자신 공격 처리
    }

    #endregion

    #region 스탯 관련 메서드
    public void AddStatBuff(StatusEffectID _id, BuffBlock _block)
    {
        this.statBlock.AddBuff(_id, _block);
    }

    public void InitStats(BattleStatus _status)
    {
        this.statBlock = new StatBlock(_status);
        this.maxHP = this.currentHP = this.statBlock.GetStat(StatType.Hp);

    }
    #endregion
    public struct HitInfo
    {
        public bool isCritical;
        public int isRaceAdvantage;
        public float hitDamage;
        public RaceType attackRace;
        public StatusEffectID statusEffect;
        public BattleCharacterBase target;
    }

}
public enum RaceType
{
    None,
    Shadow,
    Phantasm,
    Mutation,
    VoidBorn,  // or Beast
    Radiance,
}