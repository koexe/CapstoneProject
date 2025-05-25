using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_Base", menuName = "Character/Character_Base")]
public class SOBattleCharacter : ScriptableObject
{
    [SerializeField] int identifier;
    [SerializeField] RaceType raceType;
    [SerializeField] SOSkillBase[] skills;
    [SerializeField] string characterName;
    [SerializeField] BattleStatus status;
    [SerializeField] SkeletonDataAsset skeletonDataAsset;
    [SerializeField] AnimationReferenceAsset basicAttackAnim;
    [SerializeField] AnimationReferenceAsset additionalAttackAnim;
    [SerializeField] AnimationReferenceAsset hitAnim;
    [SerializeField] AnimationReferenceAsset idleAnim;
    [SerializeField] bool isModelBasicRight;

    public int GetIdentifier() => identifier;
    public SOSkillBase[] GetSkills() => skills;
    public string GetCharacterName() => characterName;
    public BattleStatus GetStatus() => status;
    public RaceType GetRaceType() => raceType;
    public bool IsModelBasicRight() => isModelBasicRight;

    public SkeletonDataAsset GetSkeletonDataAsset() => skeletonDataAsset;
    public (AnimationReferenceAsset, AnimationReferenceAsset, AnimationReferenceAsset) GetAnimations()
        => (this.idleAnim, this.basicAttackAnim, this.hitAnim  );
    public AnimationReferenceAsset GetAdditionalAttackAnim() => this.additionalAttackAnim;

}

