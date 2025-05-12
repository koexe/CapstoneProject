using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameState gameState;

    [SerializeField] GameStatics gameStatics;
    [SerializeField] DataLibrary dataLibrary;
    [SerializeField] GameObject player;


    [SerializeField] SceneLoadManager sceneLoadManager;
    [SerializeField] SaveGameManager saveGameManager;


    [SerializeField] PlayerData playerData;

    OnChangeBattleSceneData onChangeBattleSceneData = new OnChangeBattleSceneData();

    public OnChangeBattleSceneData GetBattleSceneData() => this.onChangeBattleSceneData;

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
        Initialization();
    }

    public async void Initialization()
    {
        await this.dataLibrary.Initialization();
        this.playerData = new PlayerData();
        this.playerData.currentPlayer = DataLibrary.instance.GetSOCharacter(1);
        this.playerData.currentNPC = DataLibrary.instance.GetSOCharacter(2);
    }


    public void ChangeSceneToBattle(SOBattleCharacter[] enemys)
    {
        this.onChangeBattleSceneData.Set(this.playerData, enemys);
        this.sceneLoadManager.LoadScene_Async("BattleScene");
        MapManager.instance.OnChangeToBattleScene();
    }


    public class OnChangeBattleSceneData
    {
        PlayerData playerData;
        SOBattleCharacter[] enemys;

        public void Set(PlayerData _player, SOBattleCharacter[] _enemys)
        {
            this.playerData = _player;
            this.enemys = _enemys;
        }
        public SOBattleCharacter[] GetEnemys() => this.enemys;
        public PlayerData GetPlayerData() => this.playerData;
    }

    public class PlayerData
    {
        public SOBattleCharacter currentPlayer;
        public SOBattleCharacter currentNPC;
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