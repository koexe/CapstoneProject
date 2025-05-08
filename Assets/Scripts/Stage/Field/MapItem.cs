using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour, IInteractable
{
    [SerializeField] SOItem item;
    [SerializeField] SpriteRenderer spriteRenderer;


    public void Start()
    {
        if (this.item != null)
            this.spriteRenderer.sprite = this.item.GetItemImage();
    }

    public void ExecuteAction()
    {
        this.item.GetItem();
        Destroy(this.gameObject);
    }
}
