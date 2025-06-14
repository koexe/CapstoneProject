using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

[CreateAssetMenu(fileName = "New Sub Skill", menuName = "Skill/Sub Skill")]
public class SOSubSkill : SOSkillBase
{
    [Header("방어력 증가 설정")]
    [SerializeField] private float defenseIncreasePercent = 0.2f; // 방어력 20% 증가
    [SerializeField] private int duration = 3; // 3턴 지속
    [SerializeField] private bool hasHealEffect = false; // 회복 효과 여부
    [SerializeField] private float healPercent = 0.1f; // 최대 체력의 10% 회복

    [SerializeField] ParticleSystem BuffParticle;

    public override async UniTask Execute()
    {
        await base.Execute();
        if (this.character == null)
        {
            Debug.LogError("No Character!!!");
            return;
        }

        GameManager.instance.GetCamera().SetTarget(this.character.transform);

        await this.character.AttackPosition(this.attackParticle, this.attackSound);
        foreach (var t_target in this.target)
        {
            // 방어력 증가 턴제 버프 적용
            t_target.GetBuffSystem().AddTurnBuff(StatType.Def, defenseIncreasePercent, duration, StatSign.Percentage);
                await t_target.GetBuff(BuffParticle, this.attackSound);
            // 회복 효과가 있다면 적용
            if (hasHealEffect)
            {
                float healAmount = t_target.MaxHP() * healPercent;
                t_target.Heal(healAmount);
            }
        }

        await this.character.ResetPosition();
        this.character.SetActionDone(true);
        GameManager.instance.GetCamera().SetTarget(null);
    }
}
