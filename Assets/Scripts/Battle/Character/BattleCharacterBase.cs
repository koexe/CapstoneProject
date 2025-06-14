using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using TMPro;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

public class BattleCharacterBase : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] protected string characterName;
    [SerializeField] protected BattleManager battleManager;
    [SerializeField] protected SpineModelController spineModelController;
    [SerializeField] protected SOSkillBase[] skills;
    [SerializeField] protected SOSkillBase selectedSkill;
    [SerializeField] protected BuffSystem buffSystem;

    [SerializeField] protected GameObject arrow;

    [SerializeField] protected SpriteRenderer characterShadow;

    [SerializeField] protected ParticleSystem hitParticle;

    [Header("정보")]
    [SerializeField] protected Vector3 initialPosition;
    [SerializeField] protected float maxHP;
    [SerializeField] protected float currentHP;
    [SerializeField] protected float maxMp;
    [SerializeField] protected float currentMp;
    [SerializeField] protected StatBlock statBlock;
    [SerializeField] protected int isUsedDefence;
    [SerializeField] protected bool basicRightDir; // true = 1 false = -1
    [SerializeField] protected bool isInDefence;
    [SerializeField] protected bool isActionDone = false;
    [SerializeField] protected bool isDie;
    [SerializeField] protected bool isActionDisabled;
    [SerializeField] protected CharacterActionType currentAction;
    [SerializeField] protected SOBattleCharacter soBattleCharacter;

    [SerializeField] protected HealthPreferences healthPreferences;
    [SerializeField] protected Vector3 hpBarOffset = new Vector3(0f, 1.5f, 0f);

    [SerializeField] protected float moveTime = 1f;
    public RaceType raceType;
    #region Get/Set
    public float MaxHP() => this.maxHP;
    public float GetMp() => this.currentMp;
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

    public void SetArrow(bool _is) => this.arrow.SetActive(_is);
    public void SetShadow(bool _is) => this.characterShadow.gameObject.SetActive(_is);
    public BuffSystem GetBuffSystem() => this.buffSystem;
    #endregion
    public SOSkillBase[] GetSkills() => this.skills;
    public void RecalculateMaxHPMP()
    {
        this.maxHP = this.statBlock.GetStat(StatType.Hp);
        this.maxMp = this.statBlock.GetStat(StatType.Mp);
        this.healthPreferences.SetTotalHealth(maxHP);

        // 비율 유지
        float t_ratio = Mathf.Clamp01(this.currentHP / this.maxHP);
        this.currentHP = this.maxHP * t_ratio;
        this.healthPreferences.SetCurrentHealth(currentHP);

        float t_ratioMp = Mathf.Clamp01(this.currentMp / this.maxMp);
        this.currentMp = this.maxMp * t_ratioMp;
    }
    public async UniTask Summary()
    {
        await OnTurnStart();
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
    }
    public void Heal(float _amount)
    {
        this.currentHP = Mathf.Min(this.maxHP, this.currentHP + _amount);
        this.healthPreferences.SetCurrentHealth(currentHP);
    }
    public void HealMp(float _amount)
    {
        this.currentMp = Mathf.Min(this.maxMp, this.currentMp + _amount);
    }
    public virtual void UseMp(float _amount)
    {
        this.currentMp = Mathf.Max(0f, this.currentMp - _amount);
    }
    public virtual void EnemyInitialization(BattleManager _battleManager, SOBattleCharacter _character, int _level)
    {

    }
    public virtual void PlayerInitialization(BattleManager _battleManager, SOBattleCharacter _character, (TextMeshProUGUI _hpUI, TextMeshProUGUI _mpUI, TextMeshProUGUI _nameUI) _ui)
    {

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
        this.battleManager.CharacterDie(this);
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
    public async UniTask MoveMiddlePoint()
    {
        Vector3 t_middlePoint = this.battleManager.GetMiddlePoint().position;
        //this.spineModelController.PlayAnimation(AnimationType.walk);
        await MovePoint(t_middlePoint);
    }
    public async UniTask MovePoint(Vector3 _point)
    {
        float t_time = 0f;
        Vector3 startPos = this.transform.position;
        float jumpHeight = 1.5f; // 점프 높이

        while (t_time < moveTime)
        {
            float t_lerp = t_time / moveTime;

            // X와 Z축은 선형 보간
            float t_xPos = Mathf.Lerp(startPos.x, _point.x, t_lerp);
            float t_zPos = Mathf.Lerp(startPos.z, _point.z, t_lerp);

            // Y축은 포물선 형태로 보간 (sin 곡선 활용)
            float t_yPos = Mathf.Lerp(startPos.y, _point.y, t_lerp) + (Mathf.Sin(t_lerp * Mathf.PI) * jumpHeight);

            this.transform.position = new Vector3(t_xPos, t_yPos, t_zPos);
            t_time += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate();
        }

        // 정확한 최종 위치 설정
        this.transform.position = _point;
        return;
    }
    public async UniTask AttackPosition(ParticleSystem _particle, AudioClip _sound = null)
    {
        this.battleManager.ShowText($"{this.name}의 {this.selectedSkill.skillName} 공격!!");
        await MoveMiddlePoint();
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        await this.spineModelController.PlayAnimationAsync(
            AnimationType.meleeAttack,
            _effectTiming: this.soBattleCharacter.GetEffectTiming(),
            onEffect: () =>
            {
                if (_particle != null)
                {
                    Vector3 t_offset = new Vector3(this.basicRightDir ? 1 : -1 * this.soBattleCharacter.GetEffectOffset().x, this.soBattleCharacter.GetEffectOffset().y);


                    var t_effect = Instantiate(
                        _particle,
                        this.transform.position +
                        t_offset,
                        _particle.transform.rotation);
                    t_effect.transform.localScale = this.spineModelController.transform.localScale;
                }

                if (_sound != null)
                    SoundManager.instance.PlaySE(_sound);
            });
    }
    public async UniTask AttackPosition(Vector3 _position, ParticleSystem _particle, AudioClip _sound = null)
    {
        this.battleManager.ShowText($"{this.name}의 {this.selectedSkill.skillName} 공격!!");
        await MovePoint(_position);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        await this.spineModelController.PlayAnimationAsync(
            AnimationType.meleeAttack,
            _effectTiming: this.soBattleCharacter.GetEffectTiming(),
            onEffect: () =>
                        {
                            if (_particle != null)
                            {
                                Vector3 t_offset = new Vector3((this.basicRightDir ? 1 : -1) * this.soBattleCharacter.GetEffectOffset().x, this.soBattleCharacter.GetEffectOffset().y);


                                var t_effect = Instantiate(
                                    _particle,
                                    this.transform.position +
                                    t_offset,
                                    _particle.transform.rotation);
                                t_effect.transform.localScale = this.spineModelController.transform.localScale;
                            }
                            if (_sound != null)
                                SoundManager.instance.PlaySE(_sound);
                        });



    }

    public async UniTask PlayAttackAnimationAsync()
    {
        await this.spineModelController.PlayAnimationAsync(AnimationType.meleeAttack);
    }
    public async UniTask ResetPosition()
    {
        this.spineModelController.PlayAnimation(AnimationType.idle);
        await MovePoint(this.initialPosition);
        this.spineModelController.PlayAnimation(AnimationType.idle);
    }
    public async UniTask GetBuff(ParticleSystem _particle, AudioClip _sound = null)
    {
        if (_particle != null)
        {
            var t_effect = Instantiate(
                _particle,
                this.transform.position,
                _particle.transform.rotation);
        }
        this.battleManager.ShowText($"{GetCharacterName()}{GameStatics.GetSubjectParticle(GetCharacterName())} 방어력이 올랐다!");
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
    }


    public async UniTask AttackTask(HitInfo _hitInfo)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        await _hitInfo.target.HitTask(_hitInfo);
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
    }
    public virtual async UniTask TakeDamage(HitInfo _hitInfo)
    {
        float t_finalDamage = _hitInfo.hitDamage;
        if (!_hitInfo.isDotDamage && this.isInDefence)
        {
            if (_hitInfo.isRaceAdvantage == 1)
            {
                t_finalDamage *= 0.5f;
            }
            else
            {
                t_finalDamage *= 0.25f;
            }
        }

        if (_hitInfo.hitParticle == null)
            this.hitParticle.Play();
        else
            Instantiate(_hitInfo.hitParticle, this.transform.position, this.transform.rotation);


        await this.spineModelController.PlayAnimationAsync(AnimationType.hit);
        string t_text = "";
        if (_hitInfo.isCritical)
        {
            t_text += $"치명타! ";
        }
        t_text += $"{(int)t_finalDamage}의 데미지를 {this.name}{GameStatics.GetSubjectParticle(this.name)} 받았다!!";
        this.battleManager.ShowText(t_text);
        this.currentHP = Mathf.Max(0f, this.currentHP - (int)t_finalDamage);
        this.healthPreferences.SetCurrentHealth(currentHP);
        this.spineModelController.PlayAnimation(AnimationType.idle);
        if (this.currentHP <= 0f)
            Die();
    }
    public virtual async UniTask HitTask(HitInfo _hitInfo)
    {
        await TakeDamage(_hitInfo);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        if (_hitInfo.statusEffect != StatusEffectID.None)
        {
            this.buffSystem.Add(_hitInfo.statusEffect);
            this.battleManager.ShowText($"{this.name}{GameStatics.GetSubjectParticle(this.name)}  {GameStatics.GetStatusText(_hitInfo.statusEffect)} 에 걸렸다!!");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
    }
    #endregion
    #region 방어
    public async UniTask DefenceTask()
    {
        this.battleManager.ShowText($"{this.name}{GameStatics.GetSubjectParticle(this.name)} 방어를 시작했다!");

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
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
    }
    #endregion
    #region 도망
    public async UniTask RunTask()
    {
        this.battleManager.ShowText($"{this.name}{GameStatics.GetSubjectParticle(this.name)} 도망을 시작했다!");
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
        this.statBlock = new StatBlock(_status, this.buffSystem);
        this.maxHP = this.currentHP = this.statBlock.GetStat(StatType.Hp);
        this.maxMp = this.currentMp = this.statBlock.GetStat(StatType.Mp);
    }
    #endregion
    public struct HitInfo
    {
        public bool isCritical;
        public int isRaceAdvantage;
        public float hitDamage;
        public bool isDotDamage;
        public RaceType attackRace;
        public StatusEffectID statusEffect;
        public BattleCharacterBase target;
        public ParticleSystem hitParticle;
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
        this.healthPreferences.SetCurrentHealth(currentHP);
    }
    public void UpdateSkills(SOSkillBase[] newSkills)
    {
        this.skills = newSkills;
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