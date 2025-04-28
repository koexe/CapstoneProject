using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleCharacterBase : MonoBehaviour
{
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float currentHP;
    [SerializeField]
    private StatBlock statBlock;

    public float MaxHP() => this.maxHP;

    [SerializeField] protected bool isActionDisabled;
    public void SetActionDisabled(bool _isActionDisabled) => this.isActionDisabled = _isActionDisabled;

    public int speed;
    [SerializeField] BattleManager battleManager;
    [SerializeField] SpriteRenderer skin;
    [SerializeField] Animator animator;
    [SerializeField] SOSkillBase[] skills;
    [SerializeField] SOSkillBase selectedSkill;

    [SerializeField] BuffSystem buffSystem;

    [SerializeField] bool isActionDone = false;

    [SerializeField] TextMeshPro hpText;

    public void RecalculateMaxHP()
    {
        this.maxHP = this.statBlock.GetStat(StatType.Hp);

        // 비율 유지
        float t_ratio = Mathf.Clamp01(this.currentHP / this.maxHP);
        this.currentHP = this.maxHP * t_ratio;
    }

    public void TakeDamage(float _amount)
    {
        this.currentHP = Mathf.Max(0f, this.currentHP - _amount);
        this.hpText.text = this.currentHP.ToString();
        if (this.currentHP <= 0f)
            Die();
    }

    public void Heal(float _amount)
    {
        this.currentHP = Mathf.Min(this.maxHP, this.currentHP + _amount);
    }

    public void SetActionDone() => this.isActionDone = true;

    public bool IsActionDone() => this.isActionDone;
    public SOSkillBase[] GetSkills() => this.skills;
    public void Initialization(BattleManager _battleManager)
    {
        this.battleManager = _battleManager;
        this.buffSystem = new BuffSystem();
        //Temp_SetStat();
        InitStats();
        this.hpText.text = this.currentHP.ToString();
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
        this.selectedSkill.Execute();
        this.battleManager.ShowText($"{this.name} Used {selectedSkill.skillName}");
        Invoke("SetActionDone", 3.0f);
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} has died.");
    }

    public void OnTurnStart()
    {
        this.buffSystem.OnTurnStart(this);
    }
    public float GetStat(StatType _type)
    {
        if (_type == StatType.Hp)
            return this.maxHP; // max 기준으로 리턴
        return this.statBlock.GetStat(_type);
    }
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

    public void AddStatBuff(StatusEffectID _id, BuffBlock _block)
    {
        this.statBlock.AddBuff(_id, _block);
    }

    public void InitStats()
    {
        this.statBlock = new StatBlock();
        this.maxHP = this.currentHP = this.statBlock.GetStat(StatType.Hp);
    }


}
public enum RaceType
{
    Shadow,
    Phantasm,
    Mutation,
    VoidBorn,  // or Beast
    Radiance,
}