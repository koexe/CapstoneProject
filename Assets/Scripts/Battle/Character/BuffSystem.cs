using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    private Dictionary<StatusEffectID, StatusEffectInstance> activeEffects = new Dictionary<StatusEffectID, StatusEffectInstance>();

    public void OnTurnStart(BattleCharacterBase _target)
    {
        foreach (var t_effect in this.activeEffects.Values)
            t_effect.Tick(_target);

        var t_expired = new List<StatusEffectID>();
        foreach (var t_pair in this.activeEffects)
        {
            if (t_pair.Value.IsExpired)
                t_expired.Add(t_pair.Key);
        }

        foreach (var t_key in t_expired)
            this.activeEffects.Remove(t_key);
    }

    public bool Has(StatusEffectID _name) => this.activeEffects.ContainsKey(_name);
    public StatusEffectInstance Get(StatusEffectID _name) => this.activeEffects.TryGetValue(_name, out var t_effect) ? t_effect : null;
}
public class StatusEffectInstance
{
    public StatusEffectInfo info;
    public int remainingTurns;
    public int stack;

    public StatusEffectInstance(StatusEffectInfo _info)
    {
        this.info = _info;
        this.remainingTurns = _info.duration;
        this.stack = 1;
    }

    public void Refresh()
    {
        if (!this.info.isStackable)
        {
            this.remainingTurns = this.info.duration;
        }
        else if (this.stack < this.info.maxStack)
        {
            this.stack++;
            this.remainingTurns += this.info.duration;
        }
    }

    public void Tick(BattleCharacterBase _target)
    {
        this.ApplyEffect(_target);
        this.remainingTurns--;
    }

    public void ApplyEffect(BattleCharacterBase _target)
    {
        switch (this.info.id)
        {
            case StatusEffectID.Bleed:
            case StatusEffectID.Poison:
            case StatusEffectID.Corrosion:
                float t_rate = this.info.id == StatusEffectID.Bleed ? 0.05f :
                               this.info.id == StatusEffectID.Poison ? 0.07f : 0.03f;

                int t_dmg = Mathf.CeilToInt(_target.MaxHP() * t_rate);
                _target.TakeDamage(t_dmg);
                Debug.Log($"{_target.name} takes {t_dmg} damage from {this.info.id}");
                break;

            case StatusEffectID.Stun:
                _target.SetActionDisabled(true);
                break;

                // TODO: 다른 효과 처리 추가
        }
    }

    public bool IsExpired => this.remainingTurns <= 0;
}