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

    //public void SetActionDone(bool _actionDone) => this.isActionDone = _actionDone;

    public void SetActionDone() => this.isActionDone = true;

    public bool IsActionDone() => this.isActionDone;
    public SOSkillBase[] GetSkills() => this.skills;
    public void Initialization(BattleManager _battleManager)
    {
        this.battleManager = _battleManager;
        this.buffSystem = new BuffSystem();
        return;
    }
    public void SetSelectedSkill(SOSkillBase _skill , BattleCharacterBase[] _target)
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

    [System.Serializable]
    public struct Stat
    {
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

}
