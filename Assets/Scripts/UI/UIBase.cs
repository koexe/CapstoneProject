using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class UIBase : MonoBehaviour
{
    UIData data;

    [SerializeField] protected GameObject contents;
    public abstract void Initialization(UIData data);
    public abstract void Show(UIData _data);

    public abstract void Hide();
    public SortingGroup sortingGroup;
    public bool isShow;

}


public class UIData
{
    public string identifier;
    public int order;
    public bool isAllowMultifle;
}
 