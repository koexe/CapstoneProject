using System;
using UnityEngine.Events;

public class SkillManageUIData : UIData
{
    public PlayerSkillManager skillManager;
    public UnityAction<SOSkillBase> onSkillForget;  // 스킬 잊기 선택 시 콜백
    public UnityAction<SOSkillBase> onSkillRelearn;  // 잊은 스킬 다시 배우기 선택 시 콜백
    public UnityAction<SOSkillBase, SOSkillBase> onSkillReplace;  // 스킬 교체 시 콜백 (잊을 스킬, 새로운 스킬)
    public SOSkillBase newSkillToLearn;  // 새로 배울 스킬 (있는 경우)
} 