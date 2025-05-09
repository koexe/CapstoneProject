using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    Dictionary<string, UIBase> currentUIObjects;
    Dictionary<string, GameObject> UIPrefabs;

    [SerializeField] Canvas canvas;

    static Dictionary<Type, string> staticKeys = new Dictionary<Type, string>
{
    { typeof(UIBase), "InventoryKey" },
    { typeof(SkillSelectUI), "SkillSelectUI" },
    { typeof(InventoryUI),"Inventory" },
    { typeof(DialogUI) ,"DialogUI" }

};

    private void Awake()
    {
        instance = this;
        this.currentUIObjects = new Dictionary<string, UIBase>();
        DontDestroyOnLoad(this);
        LoadAllUIs();
        if (this.canvas == null)
        {
            this.canvas = this.transform.GetComponent<Canvas>();
        }
        return;
    }

    public void HideUI(string _identifier)
    {
        if (this.currentUIObjects.ContainsKey(_identifier))
        {
            this.currentUIObjects[_identifier].Hide();
        }
        else
        {
            Debug.Log("No Such Name UI");
        }
    }


    public UIBase ShowUI<T>(UIData _data) where T : UIBase
    {
        if (this.currentUIObjects.ContainsKey(_data.identifier) && !_data.isAllowMultifle)
        {
            Debug.Log("Same UI Already Added In Screen");
            if (this.currentUIObjects[_data.identifier].isShow)
                this.currentUIObjects[_data.identifier].Hide();
            else
            {
                this.currentUIObjects[_data.identifier].Initialization(_data);
                this.currentUIObjects[_data.identifier].Show(_data);
            }

            return this.currentUIObjects[_data.identifier];
        }

        UIBase t_UIObject = GameObject.Instantiate(DataLibrary.instance.GetUI(staticKeys[typeof(T)])).GetComponent<UIBase>();
        t_UIObject.transform.SetParent(this.canvas.transform, false);
        t_UIObject.sortingGroup.sortingLayerName = "UIElements";
        if (_data.order == -1)
        {
            int t_MinOrder = 9999;
            foreach (var obj in this.canvas.transform.GetComponentsInChildren<SortingGroup>())
            {
                if (obj.sortingOrder < t_MinOrder)
                    t_MinOrder = obj.sortingOrder;
            }
            t_UIObject.sortingGroup.sortingOrder = t_MinOrder;
        }
        else
        {
            t_UIObject.sortingGroup.sortingOrder = _data.order;
        }
        this.currentUIObjects.Add(_data.identifier, t_UIObject);
        t_UIObject.Initialization(_data);
        t_UIObject.Show(_data);
        return t_UIObject;
    }
    void LoadAllUIs()
    {
        UIPrefabs = new Dictionary<string, GameObject>();
        GameObject[] t_prefabs = Resources.LoadAll<GameObject>("Prefabs/UIBases");
        foreach (var t_prefab in t_prefabs)
        {
            UIPrefabs.Add(t_prefab.name, t_prefab);
        }
    }
}

