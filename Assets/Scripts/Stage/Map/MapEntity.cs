using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MapEntity : MonoBehaviour
{
    [SerializeField] string mapIdentifier;
    [SerializeField] MapMovePoint[] mapPath;
    public MapMovePoint GetMapPoint(int _index) => this.mapPath[_index];
    public string GetID() => this.mapIdentifier;
    [SerializeField] Transform items;
    [SerializeField] Transform parallaxObject;
    [SerializeField] List<MapItem> itemEntitys;
    public int GetMapItemIndex(MapItem _item)
    {
        if (this.itemEntitys == null)
            this.itemEntitys = this.items.GetComponentsInChildren<MapItem>().ToList<MapItem>();
        return this.itemEntitys.FindIndex(x => x == _item);
    }
    public List<MapItem> GetItems()
    {
        return this.items.GetComponentsInChildren<MapItem>().ToList<MapItem>();
    }

    public void InitializeMap()
    {
        if (this.itemEntitys == null)
            this.itemEntitys = this.items.GetComponentsInChildren<MapItem>().ToList<MapItem>();
        List<bool> t_itemInfo = SaveGameManager.instance.currentSaveData.mapItems[this.mapIdentifier];
        if (t_itemInfo.Count != this.itemEntitys.Count)
        {
            LogUtil.Log("Not Matching Item Count With Savefile!");
        }
        for (int i = 0; i < this.itemEntitys.Count; i++)
        {
            this.itemEntitys[i].gameObject.SetActive(!t_itemInfo[i]);
        }
        foreach (var t_obj in this.parallaxObject.GetComponentsInChildren<ParallaxObject>())
        {
            t_obj.Initialization();
        }
    }


    [System.Serializable]
    public class MapPath
    {
        public Transform transform;
        public MapEntity linkedMap;
        public int linkedMapPoint;
    }

    public void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        if (this.itemEntitys == null)
        {
            this.itemEntitys = new List<MapItem>();
            foreach (var item in this.items.GetComponentsInChildren<MapItem>())
            {
                this.itemEntitys.Add(item);
            }
        }




    }

    public void Reset()
    {
        this.itemEntitys = new List<MapItem>();
        foreach (var item in this.items.GetComponentsInChildren<MapItem>())
        {
            this.itemEntitys.Add(item);
        }
    }
}
