using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterBase : MonoBehaviour
{
    [SerializeField] protected int maxHP = 100;
    [SerializeField] protected int currentHP;
    [SerializeField] protected bool isActionDisabled;
    public void SetActionDisabled(bool _isActionDisabled) => this.isActionDisabled = _isActionDisabled;
    public int MaxHP() => this.maxHP;

    public int speed;
    [SerializeField] BattleManager battleManager;
    [SerializeField] SpriteRenderer skin;
    [SerializeField] Animator animator;
    [SerializeField] SOSkillBase[] skills;
    [SerializeField] SOSkillBase selectedSkill;

    [SerializeField] BuffSystem buffSystem;

    [SerializeField] bool isActionDone = false;

    [SerializeField] Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();


    public void SetActionDone() => this.isActionDone = true;

    public bool IsActionDone() => this.isActionDone;
    public SOSkillBase[] GetSkills() => this.skills;
    public void Initialization(BattleManager _battleManager)
    {
        this.battleManager = _battleManager;
        this.buffSystem = new BuffSystem();
        Temp_SetStat();
        return;
    }
    public void SetSelectedSkill(SOSkillBase _skill, BattleCharacterBase[] _target)
    {
        this.selectedSkill = Instantiate(_skill);
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

    public bool IsReadySkill()
    {
        if (this.selectedSkill == null) return false;
        else if (this.selectedSkill.target == null) return false;
        else return true;
    }

    public SOSkillBase GetSelectedSkill()
    {
        return this.selectedSkill;
    }

    public void StartAction()
    {
        this.selectedSkill.Execute(this);
        this.battleManager.ShowText($"{this.name} Used {selectedSkill.skillName}");
        Invoke("SetActionDone", 3.0f);
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
        if (currentHP == 0)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} has died.");
    }

    public void OnTurnStart()
    {
        this.buffSystem.OnTurnStart(this);
    }
    #region 상태이상 대응 메서드

    public void ModifyStat(StatType _type, int _delta)
    {
        if (this.stats.TryGetValue(_type, out var t_stat))
        {
            t_stat.SetCurrent(t_stat.Current + _delta);
            this.stats[_type] = t_stat;
            Debug.Log($"{this.name}'s {_type} changed by {_delta}. Now: {t_stat.Current}");
        }
    }
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
#if UNITY_EDITOR
    void Temp_SetStat()
    {
        AddStat(StatType.Hp, 100);
        AddStat(StatType.Atk, 20);
        AddStat(StatType.Def, 10);
        AddStat(StatType.Evasion, 15);
        AddStat(StatType.Acc, 90);
        AddStat(StatType.Spd, 12);
        AddStat(StatType.HealEffecincy, 100);
    }
#endif
    #region Stats
    private void AddStat(StatType _type, int _initialValue)
    {
        Stat t_stat = new Stat();
        t_stat.Init(_initialValue);

        // statType을 직접 보관하고 싶다면 여기서 설정
        typeof(Stat).GetField("statType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValueDirect(__makeref(t_stat), _type);

        this.stats[_type] = t_stat;
    }
    [System.Serializable]
    public struct Stat
    {
        [SerializeField] StatType statType;
        [SerializeField] private int max;
        [SerializeField] private int current;

        public int Max => this.max;
        public int Current => this.current;

        public void Init(int _value)
        {
            this.max = _value;
            this.current = _value;
        }

        public void SetMax(int _value, bool _fill = true)
        {
            this.max = _value;
            if (_fill) this.current = _value;
            else this.current = Mathf.Min(this.current, this.max);
        }

        public void SetCurrent(int _value)
        {
            this.current = Mathf.Clamp(_value, 0, this.max);
        }

        public void Add(int _value)
        {
            this.current = Mathf.Clamp(this.current + _value, 0, this.max);
        }

        public void Fill()
        {
            this.current = this.max;
        }

        public void Empty()
        {
            this.current = 0;
        }
    }
    public enum StatType
    {
        None = 0,
        Hp = 1,
        Atk = 2,
        Def = 3,
        Evasion = 4,
        Acc = 5,
        Spd = 6,
        HealEffecincy = 7,
        CriticalChance = 8,
        DamageReduce = 9,
    }
    #endregion

}
