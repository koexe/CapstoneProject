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
    [SerializeField] Sprite battleSprite;
    [SerializeField] SkeletonDataAsset skeletonDataAsset;
    [SerializeField] AnimationReferenceAsset basicAttackAnim;
    [SerializeField] AnimationReferenceAsset additionalAttackAnim;
    [SerializeField] AnimationReferenceAsset hitAnim;
    [SerializeField] AnimationReferenceAsset idleAnim;
    [SerializeField] bool isModelBasicRight;
    [SerializeField] Vector2 hpBarOffset;
    [SerializeField] Vector2 effectOffset;
    [SerializeField] float effectTiming;

    public int GetIdentifier() => identifier;
    public SOSkillBase[] GetSkills() => skills;
    public string GetCharacterName() => characterName;
    public BattleStatus GetStatus() => status;
    public RaceType GetRaceType() => raceType;
    public bool IsModelBasicRight() => isModelBasicRight;
    public Sprite GetBattleSprite() => battleSprite;
    public Vector2 GetEffectOffset() => this.effectOffset;
    public Vector2 GetHpBarOffset() => this.hpBarOffset;
    public float GetEffectTiming() => this.effectTiming;

    public SkeletonDataAsset GetSkeletonDataAsset() => skeletonDataAsset;
    public (AnimationReferenceAsset, AnimationReferenceAsset, AnimationReferenceAsset) GetAnimations()
        => (this.idleAnim, this.basicAttackAnim, this.hitAnim);
    public AnimationReferenceAsset GetAdditionalAttackAnim() => this.additionalAttackAnim;

}

