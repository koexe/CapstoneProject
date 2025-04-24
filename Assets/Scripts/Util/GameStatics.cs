using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatics : MonoBehaviour
{
    public static GameStatics instance;

    public CameraController CameraController;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public class BattleData
    {

    }

    public static int GetRaceCompatibility(RaceType _typeFrom, RaceType _typeTo)
    {

        int t_count = System.Enum.GetValues(typeof(RaceType)).Length;

        int t_attackerIndex = (int)_typeFrom;
        int t_defenderIndex = (int)_typeTo;

        int t_strongAgainst = (t_attackerIndex + 1) % t_count;
        int t_weakAgainst = (t_attackerIndex - 1 + t_count) % t_count;

        if (t_defenderIndex == t_strongAgainst) return 1;  // 유리
        else if (t_defenderIndex == t_weakAgainst) return -1; // 불리
        else return 0; // 중립


    }
}

