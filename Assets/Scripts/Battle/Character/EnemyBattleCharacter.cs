using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyBattleCharacter : BattleCharacterBase
{
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
