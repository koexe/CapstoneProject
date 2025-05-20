using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public MapEntity currentMap;

#if UNITY_EDITOR
    [SerializeField] MapEntity startMap;
#endif

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
#if UNITY_EDITOR
        this.currentMap = Instantiate(startMap, this.transform);
#endif
    }

    public void MoveMap(MapEntity.MapPath _path)
    {
        Destroy(this.currentMap.gameObject);
        this.currentMap = Instantiate(_path.linkedMap, this.transform);

        var t_player = GameManager.instance.GetPlayer();
        t_player.transform.SetParent(this.currentMap.transform);



        t_player.transform.position = this.currentMap.GetMapPoint(_path.linkedMapPoint).transform.position;
    }
    public int GetItem(MapItem _mapitem)
    {
        int t_index = this.currentMap.GetMapItemIndex(_mapitem);
        if (t_index < 0)
        {
            LogUtil.Log("No Such item!!!");
            return -1;
        }
        else
        {
            return t_index;
        }
    }


    public void OnChangeToFieldScene()
    {
        this.currentMap.gameObject.SetActive(true);
    }
    public void OnChangeToBattleScene()
    {
        this.currentMap.gameObject.SetActive(false);
    }
}
