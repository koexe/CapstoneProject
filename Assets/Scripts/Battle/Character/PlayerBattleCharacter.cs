using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;

public class PlayerBattleCharacter : BattleCharacterBase
{
    [SerializeField] TextMeshProUGUI hpUI;
    [SerializeField] TextMeshProUGUI mpUI;
    [SerializeField] TextMeshProUGUI nameUI;


    public override void PlayerInitialization(BattleManager _battleManager, SOBattleCharacter _character, (TextMeshProUGUI _hpUI, TextMeshProUGUI _mpUI, TextMeshProUGUI _nameUI) _ui)
    {
        this.battleManager = _battleManager;
        if (_character.GetSkeletonDataAsset() != null)
        {
            var t_anim = _character.GetAnimations();
            this.spineModelController.Initialization(
                _character.GetSkeletonDataAsset(),
                t_anim.Item1,
                t_anim.Item2,
                t_anim.Item3);
        }

        this.buffSystem = new BuffSystem();
        this.soBattleCharacter = Instantiate(_character);
        this.characterName = soBattleCharacter.GetCharacterName();
        InitStats(soBattleCharacter.GetStatus());
        this.skills = new SOSkillBase[soBattleCharacter.GetSkills().Length];
        for (int i = 0; i < skills.Length; i++)
            this.skills[i] = Instantiate(_character.GetSkills()[i]);
        this.transform.name = _character.GetCharacterName();
        this.raceType = _character.GetRaceType();
        this.characterShadow.sprite = _character.GetBattleSprite();
        // 체력 UI 초기화


        if (this.spineModelController == null)
        {
            Debug.LogError($"[{this.name}] SpineModelController 컴포넌트가 없습니다.");
            return;
        }
        if (this.battleManager.IsAlly(this))
        {
            if (this.soBattleCharacter.IsModelBasicRight())
            {
                this.spineModelController.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                this.spineModelController.transform.localScale = new Vector3(1, 1, 1);
            }
            this.basicRightDir = true;
        }
        else
        {
            if (this.soBattleCharacter.IsModelBasicRight())
            {
                this.spineModelController.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                this.spineModelController.transform.localScale = new Vector3(-1, 1, 1);
            }
            this.basicRightDir = false;
        }

        this.spineModelController.PlayAnimation(AnimationType.idle);
        if (this.healthPreferences == null)
        {
            Debug.LogError($"[{this.name}] HealthPreferences 컴포넌트가 없습니다.");
            return;
        }
        this.healthPreferences.transform.localPosition = this.soBattleCharacter.GetHpBarOffset();
        this.arrow.transform.localPosition = this.healthPreferences.transform.localPosition + new Vector3(this.basicRightDir ? 0.5f : -0.5f, 0.5f, 0f);

        this.initialPosition = this.battleManager.GetOriginTranform(this).position;

        this.healthPreferences.SetTotalHealth(maxHP);
        this.healthPreferences.SetCurrentHealth(currentHP);
        this.healthPreferences.gameObject.SetActive(false);

        this.hpUI = _ui.Item1;
        this.mpUI = _ui.Item2;
        this.nameUI = _ui.Item3;

        this.hpUI.text = $"{this.currentHP}/{this.maxHP}";
        this.mpUI.text = $"{this.currentMp}/{this.maxMp}";
        this.nameUI.text = this.characterName;

        this.hitParticle.transform.localPosition = this.soBattleCharacter.GetCenterOffset();
        this.deathParticle.transform.localPosition = this.soBattleCharacter.GetCenterOffset();

        return;
    }

    public async override UniTask HitTask(HitInfo _hitInfo)
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

    public override async UniTask TakeDamage(HitInfo _hitInfo)
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
        this.hpUI.text = $"{this.currentHP}/{this.maxHP}";
        this.spineModelController.PlayAnimation(AnimationType.idle);
        if (this.currentHP <= 0f)
            Die();
    }

    public override void UseMp(float _amount)
    {
        base.UseMp(_amount);
        this.mpUI.text = $"{this.currentMp}/{this.maxMp}";
    }

    public override void LevelUp_UpdateStat()
    {
        float t_beforeMaxHp = this.statBlock.GetStat(StatType.Hp);
        this.statBlock.UpdateBaseStats(this.soBattleCharacter.GetStatus());
        this.maxHP = GetStat(StatType.Hp);
        float t_afterMaxHp = this.statBlock.GetStat(StatType.Hp);
        Debug.Log($"{t_afterMaxHp}        {t_beforeMaxHp}");
        Heal(t_afterMaxHp - t_beforeMaxHp);
        this.hpUI.text = $"{this.currentHP}/{this.maxHP}";
        this.mpUI.text = $"{this.currentMp}/{this.maxMp}";
    }
} 