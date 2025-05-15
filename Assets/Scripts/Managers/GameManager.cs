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

    [SerializeField] SOBattleCharacter currentPlayer;
    [SerializeField] SOBattleCharacter currentNPC;

    [SerializeField] CameraController cameraController;
    public CameraController GetCamera() => this.cameraController;
    public void SetCamera(CameraController _controller) => this.cameraController = _controller;

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
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Initialization();
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

    }

    public async void Initialization()
    {
        await this.dataLibrary.Initialization();
        this.currentPlayer = DataLibrary.instance.GetSOCharacter(1);
        this.currentNPC = DataLibrary.instance.GetSOCharacter(2);
    }


    public void ChangeSceneToBattle(SOBattleCharacter[] enemys)
    {
        this.onChangeBattleSceneData.Set(this.currentPlayer, this.currentNPC, enemys);
        this.sceneLoadManager.LoadScene_Async("BattleScene");
        MapManager.instance.OnChangeToBattleScene();
    }
    public void ChangeSceneToField(SOBattleCharacter _currentPlayer, SOBattleCharacter _currentNpc)
    {
        this.currentNPC = _currentNpc;
        this.currentPlayer = _currentPlayer;
        this.onChangeBattleSceneData.Reset();
        this.sceneLoadManager.LoadScene_Async("FieldScene");
        MapManager.instance.OnChangeToFieldScene();
    }

    public class OnChangeBattleSceneData
    {
        [SerializeField] SOBattleCharacter currentPlayer;
        [SerializeField] SOBattleCharacter currentNPC;
        SOBattleCharacter[] enemys;

        public void Set(SOBattleCharacter _player, SOBattleCharacter _npc, SOBattleCharacter[] _enemys)
        {
            this.currentPlayer = _player;
            this.currentNPC = _npc;
            this.enemys = _enemys;
        }
        public void Reset()
        {
            this.currentPlayer = null;
            this.currentNPC = null;
            this.enemys = null;
        }
        public SOBattleCharacter[] GetEnemys() => this.enemys;
        public SOBattleCharacter GetPlayerData() => this.currentPlayer;
        public SOBattleCharacter GetNPCData() => this.currentNPC;
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