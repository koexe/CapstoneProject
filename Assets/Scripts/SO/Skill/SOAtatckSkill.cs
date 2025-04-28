using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "SKill_Attack_", menuName = "Skill/Attack")]
public class SOAtatckSkill : SOSkillBase
{
    public float attackModifier;
    public StatusEffectID selfPanlty;

    public override void Execute()
    {
        base.Execute();
        if (this.target == null || this.character == null)
        {
            Debug.LogError("No Target or Chracter!!!");
            return;
        }
        foreach (var t_target in this.target)
        {
            float t_damage = this.character.GetStat(StatType.Atk) * this.attackModifier;

            t_damage += (1 - (t_target.GetStat(StatType.Def) / t_target.GetStat(StatType.Def) * 100));

        }



    }

}

