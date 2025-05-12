using UnityEngine;

[CreateAssetMenu(fileName = "Character_Base", menuName = "Character/Character_Base")]
public class SOBattleCharacter : ScriptableObject
{
    [SerializeField] int identifier;
    [SerializeField] RaceType raceType;
    [SerializeField] SOSkillBase[] skills;
    [SerializeField] string characterName;
    [SerializeField] BattleStatus status;
    //스파인 데이터 추가?

    public int GetIdentifier()=> identifier;
    public SOSkillBase[] GetSkills() => skills;
    public string GetCharacterName() => characterName;
    public BattleStatus GetStatus() => status;
    public RaceType GetRaceType() => raceType;  
    
}
