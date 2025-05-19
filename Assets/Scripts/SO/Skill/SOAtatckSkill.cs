using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;

[CreateAssetMenu(fileName = "SKill_Attack_", menuName = "Skill/Attack")]
public class SOAtatckSkill : SOSkillBase
{
    public float attackModifier;
    public StatusEffectID selfPanlty;

    public override async UniTask Execute()
    {
        await base.Execute();
        if (this.target == null || this.character == null)
        {
            Debug.LogError("No Target or Chracter!!!");
            return;
        }

        await this.character.AttackMotion();
        foreach (var t_target in this.target)
        {
            if (t_target.IsDie()) continue;


            BattleCharacterBase.HitInfo t_hitInfo = new BattleCharacterBase.HitInfo();

            float t_damage = this.character.GetStat(StatType.Atk) * this.attackModifier;

            t_damage *= 1 - (t_target.GetStat(StatType.Def) / (t_target.GetStat(StatType.Def) + 100));

            if (GameStatics.GetRaceCompatibility(this.attackRaceType, t_target.raceType) == 1)
            {
                t_damage *= 1.2f;
                t_hitInfo.isRaceAdvantage = 1;
            }
            else if (GameStatics.GetRaceCompatibility(this.attackRaceType, t_target.raceType) == -1)
            {
                t_damage *= 0.7f;
                t_hitInfo.isRaceAdvantage = -1;
            }
            else
            {
                t_hitInfo.isRaceAdvantage = 0;
            }


            if (this.character.GetStat(StatType.CriticalChance) > Random.Range(0f, 1f))
            {
                t_damage *= 1.5f;
                t_hitInfo.isCritical = true;
            }

            if (this.statusEffect != StatusEffectID.None)
            {
                if (DataLibrary.instance.GetStateInfo(this.statusEffect).activationChance > Random.Range(0f, 1f))
                {
                    t_hitInfo.statusEffect = this.statusEffect;
                }
            }
            t_hitInfo.target = t_target;
            t_hitInfo.hitDamage = t_damage;
            t_hitInfo.attackRace = this.attackRaceType;
            await this.character.AttackTask(t_hitInfo);
        }
        await this.character.ResetPosition();
        this.character.SetActionDone(true);
    }
}




