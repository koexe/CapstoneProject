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

        DontDestroyOnLoad(instance);
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
}
