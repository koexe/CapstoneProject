using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLibrary : MonoBehaviour
{
    public static DataLibrary instance;

    Dictionary<int, SOMonsterBase> monsterDictionary = new Dictionary<int, SOMonsterBase>();


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }



}
