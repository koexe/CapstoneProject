using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleCharacterBase : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] protected string characterName;
    [SerializeField] protected BattleManager battleManager;
    [SerializeField] SpineModelController spineModelController;
    [SerializeField] SOSkillBase[] skills;
    [SerializeField] SOSkillBase selectedSkill;
    [SerializeField] protected BuffSystem buffSystem;
    [SerializeField] TextMeshPro hpText;
    [SerializeField] ParticleSystem hitParticle;

    [Header("정보")]
    [SerializeField] float maxHP;
    [SerializeField] float currentHP;
    [SerializeField] StatBlock statBlock;
    [SerializeField] int isUsedDefence;
    [SerializeField] bool isInDefence;
    [SerializeField] bool isActionDone = false;
    [SerializeField] protected bool isDie;
    [SerializeField] protected bool isActionDisabled;
    [SerializeField] CharacterActionType currentAction;
    [SerializeField] protected SOBattleCharacter soBattleCharacter;

    public RaceType raceType;
    #region Get/Set
    public float MaxHP() => this.maxHP;
    public void SetActionDisabled(bool _isActionDisabled) => this.isActionDisabled = _isActionDisabled;
    public void SetAction(CharacterActionType _action) => this.currentAction = _action;
    public CharacterActionType GetAction() => this.currentAction;
    public string GetCharacterName() => this.characterName;
    public bool IsDie() => this.isDie;
    public void SetActionDone(bool _is) => this.isActionDone = _is;
    public bool IsActionDone() => this.isActionDone;
    public SOSkillBase GetSelectedSkill()
    {
        return this.selectedSkill;
    }
    public float GetStat(StatType _type)
    {
        return this.statBlock.GetStat(_type);
    }
    public BattleStatus GetStatus() => this.soBattleCharacter.GetStatus();
    #endregion
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
        this.soBattleCharacter = Instantiate(_character);
        this.characterName = soBattleCharacter.GetCharacterName();
        InitStats(soBattleCharacter.GetStatus());
        this.skills = new SOSkillBase[soBattleCharacter.GetSkills().Length];
        for (int i = 0; i < skills.Length; i++)
            this.skills[i] = Instantiate(_character.GetSkills()[i]);
        this.transform.name = _character.GetCharacterName();
        this.raceType = _character.GetRaceType();
        this.hpText.text = this.currentHP.ToString();
        this.spineModelController.PlayAnimation(AnimationType.idle);
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
                return true;
            case CharacterActionType.Item:
                return false;
            case CharacterActionType.None:
                return false;
            default: return false;
        }
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
                await RunTask();
                break;
        }
    }
    protected virtual void Die()
    {
        Debug.Log($"{name} has died.");
        this.isDie = true;
        this.spineModelController.PlayAnimation(AnimationType.die);
    }
    public async UniTask OnTurnStart()
    {
        await this.buffSystem.OnTurnStartAsync(this);
        if (!this.isInDefence)
            this.isUsedDefence = 0;
        this.isInDefence = false;

    }

    #region 턴 처리
    #region 공격/피격
    public async UniTask AttackMovePoint()
    {
        Vector3 t_middlePoint = this.battleManager.GetMiddlePoint().position;
        this.spineModelController.PlayAnimation(AnimationType.walk);
        await MovePoint(t_middlePoint);
    }
    public async UniTask MovePoint(Vector3 _point)
    {
        while (Vector3.Distance(this.transform.position, _point) >= 0.5f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, _point, Time.fixedDeltaTime * 10f);
            await UniTask.WaitForFixedUpdate();
        }
        return;
    }

    public async UniTask AttackMotion()
    {
        this.battleManager.ShowText($"{this.name}의 {this.selectedSkill.skillName} 공격!!");
        await AttackMovePoint();
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        await this.spineModelController.PlayAnimationAsync(AnimationType.meleeAttack);
    }

    public async UniTask ResetPosition()
    {
        var t_position = this.battleManager.GetPlayerTranform(this).position;
        await MovePoint(t_position);
    }

    public async UniTask AttackTask(HitInfo _hitInfo)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        await _hitInfo.target.HitTask(_hitInfo);
        this.battleManager.ShowText($"다음차례!");
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
    }
    public async UniTask TakeDamage(HitInfo _hitInfo)
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

        this.hitParticle.Play();
        await this.spineModelController.PlayAnimationAsync(AnimationType.hit);
        this.battleManager.ShowText($"{(int)t_finalDamage}의 데미지를 {this.name} 이 받았다!!");
        this.currentHP = Mathf.Max(0f, this.currentHP - (int)t_finalDamage);
        SetHpText();
        this.spineModelController.PlayAnimation(AnimationType.idle);
        if (this.currentHP <= 0f)
            Die();


    }
    void SetHpText()
    {
        this.hpText.text = this.currentHP.ToString();
    }
    public virtual async UniTask HitTask(HitInfo _hitInfo)
    {
        await TakeDamage(_hitInfo);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        if (_hitInfo.statusEffect != StatusEffectID.None)
        {
            this.buffSystem.Add(_hitInfo.statusEffect);
            this.battleManager.ShowText($"{this.name}이  {_hitInfo.statusEffect} 에 걸렸다!!");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
    }
    #endregion
    #region 방어
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
    #region 도망
    public async UniTask RunTask()
    {
        this.battleManager.ShowText($"{this.name} 이 도망을 시작했다!");
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        await this.battleManager.RunCheck();
    }

    #endregion
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
    public virtual void GainExp(int _exp)
    {
        this.soBattleCharacter.GetStatus().GainExp(_exp);
    }
    public virtual bool CheckLevelup()
    {
        return this.soBattleCharacter.GetStatus().CheckLevelup();
    }
    public SOBattleCharacter GetBattleCharacter()
    {
        return this.soBattleCharacter;
    }
    public void LevelUp_UpdateStat()
    {
        float t_beforeMaxHp = this.statBlock.GetStat(StatType.Hp);
        this.statBlock.UpdateBaseStats(this.soBattleCharacter.GetStatus());
        this.maxHP = GetStat(StatType.Hp);
        float t_afterMaxHp = this.statBlock.GetStat(StatType.Hp);
        Debug.Log($"{t_afterMaxHp}        {t_beforeMaxHp}");
        Heal(t_afterMaxHp - t_beforeMaxHp);
        SetHpText();

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