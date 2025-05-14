using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatics 
{
    public CameraController CameraController;

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

    public static void Swap<T>(List<T> _list, int _i, int _j)
    {
        (_list[_i], _list[_j]) = (_list[_j], _list[_i]);
    }

}

public static class LogUtil
{
    public static void Log(string _message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(_message);
#endif
    }

    public static void LogWarning(string _message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning(_message);
#endif
    }

    public static void LogError(string _message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogError(_message);
#endif
    }
}