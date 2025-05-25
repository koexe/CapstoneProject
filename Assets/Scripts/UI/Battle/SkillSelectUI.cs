using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillSelectUI : UIBase
{
    [SerializeField] Button[] actionButtons;

    [Header("Skill UIs")]
    [SerializeField] SkillUI[] skillUIs;

    [SerializeField] GameObject actionSelects;
    [SerializeField] GameObject skillSelects;

    SkillUIData skillUIData;
    public override void Hide()
    {
        this.contents.SetActive(false);
        this.isShow = false;
        this.skillUIData.onHide?.Invoke();
    }

    public override void Initialization(UIData _data)
    {
    }

    public override void Show(UIData _data)
    {

        SkillUIData t_skillUIData = _data as SkillUIData;
        if (t_skillUIData == null)
        {
            Debug.Log("Invalid DataType in SkillSelectUI");
            return;
        }
        for (int i = 0; i < skillUIs.Length; i++)
        {
            this.skillUIData = t_skillUIData;
            this.skillUIs[i].Initialization(t_skillUIData.battleCharacterBase, t_skillUIData.skills[i], t_skillUIData.action);
        }
        this.contents.SetActive(true);
        this.actionSelects.SetActive(true);
        this.skillSelects.SetActive(false);
        this.isShow = true;

    }

    public void BaseAttack()
    {
        LogUtil.Log("기본공격 예정");
        this.skillUIData.battleCharacterBase.SetAction(CharacterActionType.Attack);
        this.skillUIData.action(this.skillUIData.battleCharacterBase, Instantiate(DataLibrary.instance.GetSOSkill(1)));
        UIManager.instance.HideUI("BattleSkillUI");
    }
    public void Defence()
    {
        LogUtil.Log("방어 예정");
        this.skillUIData.battleCharacterBase.SetAction(CharacterActionType.Defence);
        this.skillUIData.battleManager.CheckAllReady();
        UIManager.instance.HideUI("BattleSkillUI");
    }
    public void Skill()
    {
        this.actionSelects.SetActive(false);
        this.skillSelects.SetActive(true);
    }
    public void Talant()
    {
        LogUtil.Log("궁극기 구현 예정");
        UIManager.instance.HideUI("BattleSkillUI");
    }
}
public class SkillUIData : UIData
{
    public BattleCharacterBase battleCharacterBase;
    public BattleManager battleManager;
    public SOSkillBase[] skills;
    public Action<BattleCharacterBase, SOSkillBase> action;
}