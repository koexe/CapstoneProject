using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameState gameState;

    [SerializeField] GameStatics gameStatics;
    [SerializeField] DataLibrary dataLibrary;
    [SerializeField] GameObject player;
    [SerializeField] SceneLoadManager sceneLoadManager;
    [SerializeField] SaveGameManager saveGameManager;

    public GameState GetGameState() => this.gameState;
    public void SetGameState(GameState _gameState) => this.gameState = _gameState;    

    public GameObject GetPlayer()
    {
        return this.player;
    }

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
public enum GameState
{
    None,
    Menu,
    Loading,
    Ingame,
    Pause,
    Battle
}