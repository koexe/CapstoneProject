using Cysharp.Threading.Tasks;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    public MapEntity currentMap;

#if UNITY_EDITOR
    [SerializeField] MapEntity startMap;
    [SerializeField] bool isDebugingMap;
#endif

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }


    public void Initialization()
    {
#if UNITY_EDITOR
        if (this.isDebugingMap)
            LoadMap(this.startMap.GetID());
        else
            LoadMap(SaveGameManager.instance.GetCurrentSaveData().currentMap);
#else
        LoadMap(SaveGameManager.instance.GetCurrentSaveData().currentMap);
#endif
    }

    public void Detialization()
    {
        Destroy(this.currentMap.gameObject);
        this.currentMap = null;
    }

    public void LoadMap(string _mapName)
    {
        this.currentMap = Instantiate(DataLibrary.instance.GetMap(_mapName), this.transform);
        this.currentMap.InitializeMap();
        SaveGameManager.instance.GetCurrentSaveData().currentMap = _mapName;
    }



    public void MoveMap(MapEntity.MapPath _path)
    {
        var t_player = GameManager.instance.GetPlayer();
        t_player.transform.SetParent(null);
        Destroy(this.currentMap.gameObject);

        this.currentMap = Instantiate(_path.linkedMap, this.transform);
        t_player.transform.SetParent(this.currentMap.transform);

        SaveGameManager.instance.GetCurrentSaveData().currentMap = this.currentMap.GetID();

        t_player.transform.position = this.currentMap.GetMapPoint(_path.linkedMapPoint).GetSpawnPoint().position;

        GameManager.instance.GetCamera().SetPosition(t_player.transform.position);

        this.currentMap.InitializeMap(); 


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
        GameManager.instance.GetPlayer().SetActive(true);
    }
    public void OnChangeToBattleScene()
    {
        this.currentMap.gameObject.SetActive(false);
        GameManager.instance.GetPlayer().SetActive(false);
    }
}
