using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameStatics gameStatics;
    [SerializeField] SceneLoadManager sceneLoadManager;
    [SerializeField] SaveGameManager saveGameManager;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    


    class OnChangeBattleSceneData
    {
        int[] enemys;
        
    }

    class PlayerData
    {
        SOSkillBase[] skillBases;
        SOMonsterBase currentNPC;
    }
}
