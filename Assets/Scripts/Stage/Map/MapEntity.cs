using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEntity : MonoBehaviour
{
    [SerializeField] MapPath[] mapPath;
    
    public void MoveMap()
    {

    }




    [System.Serializable]
    public class MapPath
    {
        public MapMovePoint transform;
        public MapEntity linkedMap;
        public int linkedMapPoint;
    }
}
