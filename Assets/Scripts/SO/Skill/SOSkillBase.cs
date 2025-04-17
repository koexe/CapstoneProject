using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SKill_Base", menuName = "Skill/SkillBase")]
public class SOSkillBase : ScriptableObject
{
    public BattleCharacterBase character;
    public BattleCharacterBase[] target;
    public string skillName;
    public Sprite skillIcon;
    public AttackRangeType attackRange;
    
    public enum AttackRangeType
    {
        All,
        Select,
        Random,
        Ally
    }

    public void Execute(BattleCharacterBase _character)
    {
        Debug.Log($"{_character.name} Used :    SkillName    {this.skillName} ");
    }
}
