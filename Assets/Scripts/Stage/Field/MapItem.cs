using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour, IInteractable
{
    [SerializeField] SOItem item;
    [SerializeField] SpriteRenderer spriteRenderer;

    public bool isGeted = false;


    public void Start()
    {
        if (this.item != null)
            this.spriteRenderer.sprite = this.item.GetItemImage();
    }

    public void ExecuteAction()
    {
        this.item.GetItem();
        SaveGameManager.instance.currentSaveData.mapItems[MapManager.instance.currentMap.GetID()][MapManager.instance.GetItem(this)] = true;
        Destroy(this.gameObject);
    }
}
