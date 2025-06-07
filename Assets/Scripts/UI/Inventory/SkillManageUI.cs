using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillManageUI : UIBase
{
    [Header("Active Skills")]
    [SerializeField] private Transform activeSkillContent;
    [SerializeField] private GameObject activeSkillItemPrefab;
    
    [Header("Forgotten Skills")]
    [SerializeField] private Transform forgottenSkillContent;
    [SerializeField] private GameObject forgottenSkillItemPrefab;
    
    [Header("New Skill Info")]
    [SerializeField] private GameObject newSkillPanel;
    [SerializeField] private TextMeshProUGUI newSkillNameText;
    [SerializeField] private TextMeshProUGUI newSkillDescriptionText;
    [SerializeField] private Button learnButton;
    
    private PlayerSkillManager skillManager;
    private SOSkillBase newSkill;
    private List<GameObject> activeSkillItems = new List<GameObject>();
    private List<GameObject> forgottenSkillItems = new List<GameObject>();

    public override void Initialization(UIData data)
    {
        if (data is SkillManageUIData skillData)
        {
            this.skillManager = skillData.skillManager;
            this.newSkill = skillData.newSkillToLearn;
        }
    }

    public override void Show(UIData data)
    {
        if (data is SkillManageUIData skillData)
        {
            this.skillManager = skillData.skillManager;
            this.newSkill = skillData.newSkillToLearn;
            
            // 새로운 스킬 정보 설정
            if (newSkill != null)
            {
                newSkillPanel.SetActive(true);
                newSkillNameText.text = newSkill.skillName;
                newSkillDescriptionText.text = newSkill.skillName; // 설명이 없으므로 스킬 이름으로 대체
                learnButton.interactable = !skillManager.IsSkillSlotsFull();
            }
            else
            {
                newSkillPanel.SetActive(false);
            }
            
            RefreshSkillLists();
            contents.SetActive(true);
            isShow = true;
        }
    }

    public override void Hide()
    {
        contents.SetActive(false);
        isShow = false;
    }

    private void RefreshSkillLists()
    {
        // 기존 아이템 정리
        ClearSkillItems();
        
        // 활성 스킬 목록 생성
        foreach (var skill in skillManager.GetActiveSkills())
        {
            CreateActiveSkillItem(skill);
        }
        
        // 잊은 스킬 목록 생성
        foreach (var skill in skillManager.GetForgottenSkills())
        {
            CreateForgottenSkillItem(skill);
        }
    }

    private void CreateActiveSkillItem(SOSkillBase skill)
    {
        GameObject item = Instantiate(activeSkillItemPrefab, activeSkillContent);
        activeSkillItems.Add(item);
        
        // 스킬 정보 설정
        item.GetComponentInChildren<TextMeshProUGUI>(true).text = skill.skillName;
        
        // 스킬 아이콘 설정
        if (skill.skillIcon != null && item.GetComponentInChildren<Image>() != null)
        {
            item.GetComponentInChildren<Image>().sprite = skill.skillIcon;
        }
        
        // 새로운 스킬이 있고 스킬 슬롯이 가득 찼을 때만 '잊기' 버튼 활성화
        Button forgetButton = item.GetComponentInChildren<Button>();
        if (forgetButton != null)
        {
            forgetButton.gameObject.SetActive(newSkill != null && skillManager.IsSkillSlotsFull());
            forgetButton.onClick.AddListener(() => OnForgetSkillClick(skill));
        }
    }

    private void CreateForgottenSkillItem(SOSkillBase skill)
    {
        GameObject item = Instantiate(forgottenSkillItemPrefab, forgottenSkillContent);
        forgottenSkillItems.Add(item);
        
        // 스킬 정보 설정
        item.GetComponentInChildren<TextMeshProUGUI>(true).text = skill.skillName;
        
        // 스킬 아이콘 설정
        if (skill.skillIcon != null && item.GetComponentInChildren<Image>() != null)
        {
            item.GetComponentInChildren<Image>().sprite = skill.skillIcon;
        }
        
        // '다시 배우기' 버튼 설정
        Button relearnButton = item.GetComponentInChildren<Button>();
        if (relearnButton != null)
        {
            relearnButton.interactable = !skillManager.IsSkillSlotsFull();
            relearnButton.onClick.AddListener(() => OnRelearnSkillClick(skill));
        }
    }

    private void ClearSkillItems()
    {
        foreach (var item in activeSkillItems)
        {
            Destroy(item);
        }
        activeSkillItems.Clear();
        
        foreach (var item in forgottenSkillItems)
        {
            Destroy(item);
        }
        forgottenSkillItems.Clear();
    }

    private void OnForgetSkillClick(SOSkillBase skillToForget)
    {
        if (newSkill != null)
        {
            skillManager.ForgetAndLearnSkill(skillToForget, newSkill);
            Hide();  // UI 닫기
        }
    }

    private void OnRelearnSkillClick(SOSkillBase skillToRelearn)
    {
        if (!skillManager.IsSkillSlotsFull())
        {
            skillManager.ReLearnForgottenSkill(skillToRelearn);
            RefreshSkillLists();
        }
    }

    public void OnLearnNewSkillClick()
    {
        if (newSkill != null && !skillManager.IsSkillSlotsFull())
        {
            skillManager.LearnNewSkill(newSkill);
            Hide();  // UI 닫기
        }
    }

    private void OnDestroy()
    {
        ClearSkillItems();
    }
} 