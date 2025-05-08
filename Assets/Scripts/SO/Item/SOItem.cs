using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "Item/item")]
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
        var t_items = SaveGameManager.instance.currentSaveData.items;

        if (t_items.TryGetValue(this.itemIndex, out var t_item))
        {
            t_item.amount += 1;
        }
        else
        {
            t_items[this.itemIndex] = new SaveItem(this, 1);
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
