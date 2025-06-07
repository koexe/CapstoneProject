using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIGroup : MonoBehaviour
{
    public void OnMapButtonClick()
    {
        UIManager.instance.ShowUI<MapUI>(new UIData()
        {
            identifier = "MapUI"
        });
        UIManager.instance.HideUI("InventoryUI");
        UIManager.instance.HideUI("SkillManageUI");
    }
    public void OnInventoryButtonClick()
    {
        UIManager.instance.ShowUI<InventoryUI>(new UIData()
        {
            identifier = "InventoryUI"
        });
        UIManager.instance.HideUI("MapUI");
        UIManager.instance.HideUI("SkillManageUI");
    }
    public void OnSkillButtonClick()
    {
        UIManager.instance.ShowUI<SkillManageUI>(new UIData()
        {
            identifier = "SkillManageUI"
        });
        UIManager.instance.HideUI("MapUI");
        UIManager.instance.HideUI("InventoryUI");
    }
}
