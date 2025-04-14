using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SkillSelectUI : UIBase
{
    [SerializeField] SkillUI[] skillUIs;
    public override void Hide()
    {
        this.contents.SetActive(false);
        this.isShow = false;
    }

    public override void Initialization(UIData data)
    {
        SkillUIData t_skillUIData = data as SkillUIData;
        if (t_skillUIData == null)
        {
            Debug.Log("Invalid DataType in SkillSelectUI");
            return;
        }
        for (int i = 0; i < skillUIs.Length; i++)
        {
            this.skillUIs[i].Initialization(t_skillUIData.battleCharacterBase, t_skillUIData.skills[i] , t_skillUIData.action);
        }
    }

    public override void Show(UIData _data)
    {
        this.contents.SetActive(true);
        this.isShow = true;
    }


}
public class SkillUIData : UIData
{
    public BattleCharacterBase battleCharacterBase;
    public SkillBase[] skills;
    public Action<BattleCharacterBase,SkillBase> action;
}