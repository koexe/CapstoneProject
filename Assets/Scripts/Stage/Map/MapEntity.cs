using System.Collections.Generic;
using UnityEngine;

public class MapEntity : MonoBehaviour
{
    [SerializeField] string mapIdentifier;
    [SerializeField] MapMovePoint[] mapPath;
    public MapMovePoint GetMapPoint(int _index) => this.mapPath[_index];
    public string GetID() => this.mapIdentifier;
    [SerializeField] Transform items;
    [SerializeField] List<MapItem> itemEntitys;

    public int GetMapItemIndex(MapItem _item)
    {
        return this.itemEntitys.FindIndex(x => x == _item);
    }
    public List<MapItem> GetItems() => this.itemEntitys;


    [System.Serializable]
    public class MapPath
    {
        public Transform transform;
        public MapEntity linkedMap;
        public int linkedMapPoint;
    }

    public void OnDrawGizmos()
    {
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
