using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    private const int MAX_SKILLS = 7;
    
    [SerializeField] private List<SOSkillBase> activeSkills = new List<SOSkillBase>();
    [SerializeField] private List<SOSkillBase> forgottenSkills = new List<SOSkillBase>();
    
    private BattleCharacterBase playerCharacter;

    private void Awake()
    {
        playerCharacter = GetComponent<BattleCharacterBase>();
        if (playerCharacter == null)
        {
            Debug.LogError("PlayerSkillManager requires BattleCharacterBase component");
            return;
        }
    }

    /// <summary>
    /// 새로운 스킬을 배우는 메서드
    /// </summary>
    public bool LearnNewSkill(SOSkillBase newSkill)
    {
        // 이미 해당 스킬을 알고 있는 경우
        if (activeSkills.Contains(newSkill))
        {
            Debug.Log($"이미 {newSkill.skillName} 스킬을 알고 있습니다.");
            return false;
        }

        // 잊어버린 스킬 목록에서 제거
        if (forgottenSkills.Contains(newSkill))
        {
            forgottenSkills.Remove(newSkill);
        }

        // 스킬 슬롯이 가득 찬 경우
        if (activeSkills.Count >= MAX_SKILLS)
        {
            Debug.Log("스킬 슬롯이 가득 찼습니다. 기존 스킬을 잊어야 합니다.");
            return false;
        }

        // 새로운 스킬 추가
        activeSkills.Add(Instantiate(newSkill));
        UpdateCharacterSkills();
        Debug.Log($"새로운 스킬 {newSkill.skillName}을(를) 배웠습니다!");
        return true;
    }

    /// <summary>
    /// 기존 스킬을 잊고 새로운 스킬을 배우는 메서드
    /// </summary>
    public bool ForgetAndLearnSkill(SOSkillBase skillToForget, SOSkillBase newSkill)
    {
        if (!activeSkills.Contains(skillToForget))
        {
            Debug.LogError($"스킬 {skillToForget.skillName}을(를) 가지고 있지 않습니다.");
            return false;
        }

        if (activeSkills.Contains(newSkill))
        {
            Debug.LogError($"이미 스킬 {newSkill.skillName}을(를) 알고 있습니다.");
            return false;
        }

        // 기존 스킬을 잊어버린 스킬 목록으로 이동
        forgottenSkills.Add(skillToForget);
        activeSkills.Remove(skillToForget);

        // 새로운 스킬 추가
        activeSkills.Add(Instantiate(newSkill));
        UpdateCharacterSkills();

        Debug.Log($"스킬 {skillToForget.skillName}을(를) 잊고, {newSkill.skillName}을(를) 배웠습니다!");
        return true;
    }

    /// <summary>
    /// 잊어버린 스킬을 다시 배우는 메서드 (특정 아이템 사용 시)
    /// </summary>
    public bool ReLearnForgottenSkill(SOSkillBase skillToReLearn)
    {
        if (!forgottenSkills.Contains(skillToReLearn))
        {
            Debug.LogError($"스킬 {skillToReLearn.skillName}은(는) 잊어버린 스킬 목록에 없습니다.");
            return false;
        }

        if (activeSkills.Count >= MAX_SKILLS)
        {
            Debug.LogError("스킬 슬롯이 가득 찼습니다. 기존 스킬을 잊어야 합니다.");
            return false;
        }

        // 잊어버린 스킬 목록에서 제거하고 활성 스킬로 추가
        forgottenSkills.Remove(skillToReLearn);
        activeSkills.Add(Instantiate(skillToReLearn));
        UpdateCharacterSkills();

        Debug.Log($"잊어버린 스킬 {skillToReLearn.skillName}을(를) 다시 배웠습니다!");
        return true;
    }

    /// <summary>
    /// 현재 보유한 스킬 목록을 반환
    /// </summary>
    public List<SOSkillBase> GetActiveSkills()
    {
        return activeSkills;
    }

    /// <summary>
    /// 잊어버린 스킬 목록을 반환
    /// </summary>
    public List<SOSkillBase> GetForgottenSkills()
    {
        return forgottenSkills;
    }

    /// <summary>
    /// 캐릭터의 스킬 배열을 업데이트
    /// </summary>
    private void UpdateCharacterSkills()
    {
        if (playerCharacter != null)
        {
            // BattleCharacterBase의 skills 배열 업데이트
            SOSkillBase[] newSkillArray = activeSkills.ToArray();
            playerCharacter.UpdateSkills(newSkillArray);
        }
    }

    /// <summary>
    /// 스킬 슬롯이 가득 찼는지 확인
    /// </summary>
    public bool IsSkillSlotsFull()
    {
        return activeSkills.Count >= MAX_SKILLS;
    }

    /// <summary>
    /// 특정 스킬을 보유하고 있는지 확인
    /// </summary>
    public bool HasSkill(SOSkillBase skill)
    {
        return activeSkills.Contains(skill);
    }

    /// <summary>
    /// 특정 스킬이 잊어버린 스킬 목록에 있는지 확인
    /// </summary>
    public bool HasForgottenSkill(SOSkillBase skill)
    {
        return forgottenSkills.Contains(skill);
    }
} 