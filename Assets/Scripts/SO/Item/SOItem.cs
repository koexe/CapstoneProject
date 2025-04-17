using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_",menuName ="Item/item")]
public class SOItem : ScriptableObject
{
    [SerializeField] protected int itemIndex;
    [SerializeField] protected string itemName;
    [SerializeField] protected string itemDescription;
    [SerializeField] protected bool isUsable;
    [SerializeField] protected Sprite itemImage;


    public virtual void UseItem()
    {
        CheckItemAmount();
        return;
    }

    public virtual void GetItem()
    {
        if (SaveGameManager.instance.currentSaveData.items.Any(x => this.itemIndex == x.GetItemIndex()))
        {
            int t_Index = SaveGameManager.instance.currentSaveData.items.FindIndex(x => this.itemIndex == x.GetItemIndex());
            SaveGameManager.instance.currentSaveData.items[t_Index].amount += 1;
        }
        else
        {
            SaveItem t_saveItem = new SaveItem(this,1);
            SaveGameManager.instance.currentSaveData.items.Add(t_saveItem);
        }
        return;
    }

    protected void CheckItemAmount()
    {

        return;
    }


    public Sprite GetItemImage() => this.itemImage;
    public string GetItemName() => this.itemName;
    public string GetItemDescription() => this.itemDescription;
    public int GetItemIndex() => this.itemIndex;
    public bool IsUsable() => this.isUsable;
}
