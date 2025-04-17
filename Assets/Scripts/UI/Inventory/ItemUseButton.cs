using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUseButton : UIBase
{
    [SerializeField] Button useButton;
    [SerializeField] Button trashButton;

    public Button GetUseButton() => this.useButton;
    public Button GetTrashButton() => this.trashButton;

    public override void Initialization(UIData data)
    {
        ItemUseData t_ItemUseData = data as ItemUseData;
        if (t_ItemUseData == null)
        {
            Debug.Log("Invalid DataType in InventoryUI");
            return;
        }
        this.useButton.onClick.RemoveAllListeners();
        this.useButton.onClick.AddListener(() => t_ItemUseData.action?.Invoke());
    }

    public override void Show(UIData _data)
    {

    }

    public override void Hide()
    {

    }

    class ItemUseData : UIData
    {
        public Action action;
    }
}
