using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : UIBase
{
    [SerializeField] List<SaveItem> items;

    [SerializeField] Transform slotTransform;

    [SerializeField] ExplainArea explainArea;

    [SerializeField] GameObject button;

    public ExplainArea GetExplainArea() => this.explainArea;
    public GameObject GetButton() => this.button;

    public void Sort()
    {
        this.items.Sort((x, y) => x.GetItemIndex().CompareTo(y.GetItemIndex()));
        Refresh();
        return;
    }

    public void Refresh()
    {
        int index = 0;
        foreach (InventoryItemSlot slot in this.slotTransform.GetComponentsInChildren<InventoryItemSlot>())
        {
            if (this.items.Count <= index)
                break;
            else
                slot.SetItem(this.items[index]);
            index++;
        }
        foreach (InventoryItemSlot slot in this.slotTransform.GetComponentsInChildren<InventoryItemSlot>())
        {
            slot.Initialization();
            slot.SetInventoryUI(this);
        }
        return;
    }

    public override void Initialization(UIData data)
    {
        InventoryUIData t_skillUIData = data as InventoryUIData;
        if (t_skillUIData == null)
        {
            Debug.Log("Invalid DataType in InventoryUI");
            return;
        }

        this.items = SaveGameManager.instance.GetCurrentSaveData().items.Values.ToList();
        Refresh();
        GameManager.instance.SetGameState(GameState.Pause);

    }

    public override void Show(UIData _data)
    {
        this.contents.SetActive(true);
        this.items = SaveGameManager.instance.GetCurrentSaveData().items.Values.ToList();
        Refresh();
        GameManager.instance.SetGameState(GameState.Pause);
    }

    public override void Hide()
    {
        this.contents.SetActive(false);
        var t_currentSaveData = SaveGameManager.instance.GetCurrentSaveData();
        t_currentSaveData.items = null;

        Dictionary<int, SaveItem> t_itemDictionary = new Dictionary<int, SaveItem>();
        foreach (var t_item in this.items)
        {
            t_itemDictionary.Add(t_item.GetItemIndex(), t_item);
        }
        t_currentSaveData.items = t_itemDictionary;
        SaveGameManager.instance.SetCurrentSaveData(t_currentSaveData); 
        return;
    }



}
public class InventoryUIData : UIData
{

}