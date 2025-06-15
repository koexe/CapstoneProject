using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyBattleCharacter : BattleCharacterBase
{
    public override void EnemyInitialization(BattleManager _battleManager, SOBattleCharacter _character, int _level)
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
        this.initialPosition = this.battleManager.GetOriginTranform(this).position;
        this.buffSystem = new BuffSystem();
        this.soBattleCharacter = Instantiate(_character);
        this.soBattleCharacter.GetStatus().SetLevel(_level);
        this.characterName = soBattleCharacter.GetCharacterName();
        InitStats(soBattleCharacter.GetStatus());
        this.skills = new SOSkillBase[soBattleCharacter.GetSkills().Length];
        for (int i = 0; i < skills.Length; i++)
            this.skills[i] = Instantiate(_character.GetSkills()[i]);
        this.transform.name = _character.GetCharacterName();
        this.raceType = _character.GetRaceType();
        this.characterShadow.sprite = _character.GetBattleSprite();



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
        // 체력 UI 초기화
        if (this.healthPreferences == null)
        {
            Debug.LogError($"[{this.name}] HealthPreferences 컴포넌트가 없습니다.");
            return;
        }
        this.healthPreferences.transform.localPosition = this.soBattleCharacter.GetHpBarOffset();
        this.arrow.transform.localPosition = this.healthPreferences.transform.localPosition + new Vector3(this.basicRightDir ? 0.5f : -0.5f, 0.5f, 0f);

        this.healthPreferences.SetTotalHealth(maxHP);
        this.healthPreferences.SetCurrentHealth(currentHP);
        this.spineModelController.PlayAnimation(AnimationType.idle);

        this.hitParticle.transform.localPosition = this.soBattleCharacter.GetCenterOffset();
        this.deathParticle.transform.localPosition = this.soBattleCharacter.GetCenterOffset();

        return;
    }

    public async override UniTask HitTask(HitInfo _hitInfo)
    {
        await TakeDamage(_hitInfo);
        if (this.isDie)
        {
            this.battleManager.ShowText($"{this.name}{GameStatics.GetSubjectParticle(this.name)} 쓰러졌다!");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            await this.battleManager.GainExp(1000);
            return;
        }

        if (_hitInfo.statusEffect != StatusEffectID.None)
        {
            this.buffSystem.Add(_hitInfo.statusEffect);
            this.battleManager.ShowText($"{this.name}{GameStatics.GetSubjectParticle(this.name)}  {GameStatics.GetStatusText(_hitInfo.statusEffect)} 에 걸렸다!!");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
    }
}
