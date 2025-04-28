using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "SKill_Base", menuName = "Skill/SkillBase")]
public class SOSkillBase : ScriptableObject
{
    public BattleCharacterBase character;
    public BattleCharacterBase[] target;
    public string skillName;
    public Sprite skillIcon;
    public AttackRangeType attackRange;
    public StatusEffectID statusEffect;
    public RaceType attackRaceType;

    public enum AttackRangeType
    {
        All,
        Select,
        Random,
        Ally,
        Self,
    }

    public virtual async void Execute()
    {
        await Task.Delay(3000); // 밀리초 단위 (3초)
        this.character.SetActionDone(true);
    }
}
