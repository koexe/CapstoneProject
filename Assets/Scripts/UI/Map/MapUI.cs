using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : UIBase
{
    [SerializeField] Dictionary<string, MapIcon> icons = new Dictionary<string, MapIcon>();
    [SerializeField] GameObject playerMarker;

    public override void Hide()
    {
        this.contents.SetActive(false);
        this.isShow = false;
    }

    public override void Initialization(UIData data)
    {
        var t_icons = this.transform.GetComponentsInChildren<MapIcon>();

        foreach (var t_icon in t_icons)
        {
            if (t_icon.GetMapId() == null) continue;
            else if (this.icons.ContainsKey(t_icon.GetMapId()))
            {
                LogUtil.Log($"duplicated Mapping Entitiy! Check {t_icon.GetMapId()}");
            }
            else
            {
                this.icons.Add(t_icon.GetMapId(), t_icon);
            }
        }
    }

    public override void Show(UIData _data) 
    {
        this.contents.SetActive(true);
        this.isShow = true;
        if (this.icons.TryGetValue(MapManager.instance.currentMap.GetID(), out var t_icon))
        {
            this.playerMarker.transform.position = t_icon.transform.position;
        }
        else
        {
            Debug.Log($"No such Mapping {MapManager.instance.currentMap.GetID()}");
        }
    }
    private void Reset()
    {
        var t_icons = this.transform.GetComponentsInChildren<MapIcon>();

        foreach (var t_icon in t_icons)
        {
            if (t_icon.GetMapId() == null) continue;
            else if (this.icons.ContainsKey(t_icon.GetMapId()))
            {
                LogUtil.Log($"duplicated Mapping Entitiy! Check {t_icon.GetMapId()}");
            }
            else
            {
                this.icons.Add(t_icon.GetMapId(), t_icon);
            }
        }
    }
}
