using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameState gameState;

    [SerializeField] DataLibrary dataLibrary;
    [SerializeField] GameObject player;
    [SerializeField] SceneLoadManager sceneLoadManager;
    [SerializeField] SaveGameManager saveGameManager;
    [SerializeField] MainScene mainScene;

    [SerializeField] MapManager mapManager;

    [SerializeField] SOBattleCharacter currentPlayer;
    [SerializeField] SOBattleCharacter currentNPC;

    [SerializeField] CameraController cameraController;

    [SerializeField] FieldManager fieldManager;
    public CameraController GetCamera() => this.cameraController;
    public void SetCamera(CameraController _controller) => this.cameraController = _controller;

    BattleData onChangeBattleSceneData = new BattleData();

    public BattleData GetBattleSceneData() => this.onChangeBattleSceneData;

    public GameState GetGameState() => this.gameState;
    public void SetGameState(GameState _gameState) => this.gameState = _gameState;

    public void SetMainScene(MainScene _mainScene) => this.mainScene = _mainScene;

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
        ChangeGameState(GameState.Loading);
        await this.dataLibrary.Initialization();
        await this.saveGameManager.Initialization();

        // DataLibrary 로딩 완료 후 MainScene 초기화
        if (this.mainScene != null)
        {
            this.mainScene.Initialization();
        }

        ChangeGameState(GameState.Main);

    }

    public void MapInitialization()
    {
        this.mapManager.Initialization();
        this.currentPlayer = DataLibrary.instance.GetSOCharacter(1);
        this.currentNPC = DataLibrary.instance.GetSOCharacter(2);
        this.onChangeBattleSceneData.currentPlayerHp = this.currentPlayer.GetStatus().GetHp();
        this.onChangeBattleSceneData.currentNPCHp = this.currentNPC.GetStatus().GetHp();
    }

    public async UniTask ChangeSceneMainToField()
    {
        await this.sceneLoadManager.LoadScene_Async("FieldScene");
        this.fieldManager.Initialization();
        ChangeGameState(GameState.Field);
        MapInitialization();
        MapManager.instance.OnChangeToFieldScene();
    }

    public async void ChangeSceneFieldToBattle(SOBattleCharacter[] enemys, int[] _levels)
    {
        ChangeGameState(GameState.Battle);
        this.player.GetComponent<PlayerInputModule>().ShowEncounterUI();
        this.onChangeBattleSceneData.FieldToBattle(this.currentPlayer, this.currentNPC, enemys, this.player.transform.position, _levels);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        MapManager.instance.OnChangeToBattleScene();
        await this.sceneLoadManager.LoadScene_Async("BattleScene");
    }

    public async void ChangeSceneBattleToField(SOBattleCharacter _currentPlayer, SOBattleCharacter _currentNpc)
    {
        ChangeGameState(GameState.Field);
        this.currentNPC = _currentNpc;
        this.currentPlayer = _currentPlayer;
        this.player.transform.position = this.onChangeBattleSceneData.position;
        this.onChangeBattleSceneData.Reset();
        await this.sceneLoadManager.LoadScene_Async("FieldScene");
        MapManager.instance.OnChangeToFieldScene();
        this.cameraController.SetTarget(this.player.transform);
    }

    public async UniTask GameOver()
    {

        this.mapManager.Detialization();
        await this.sceneLoadManager.LoadScene_Async("MainScene");
    }

    public void ChangeGameState(GameState _gameState)
    {
        switch (this.gameState)
        {
            case GameState.None:
                this.gameState = _gameState;
                break;
            case GameState.Main:
                if (_gameState != GameState.Field)
                {
                    LogUtil.Log("not Allowed Change GameState");
                }
                else
                {
                    this.gameState = _gameState;
                }
                break;
            case GameState.Loading:
                if (_gameState != GameState.Main)
                {
                    LogUtil.Log("not Allowed Change GameState");
                }
                else
                {
                    this.gameState = _gameState;
                }
                break;
            case GameState.Field:
                this.gameState = _gameState;
                break;
            case GameState.Pause:
                this.gameState = _gameState;
                break;
            case GameState.Battle:
                this.gameState = _gameState;
                break;
        }
    }


    public class BattleData
    {
        [SerializeField] SOBattleCharacter currentPlayer;
        [SerializeField] SOBattleCharacter currentNPC;
        public int currentPlayerHp;
        public int currentNPCHp;
        SOBattleCharacter[] enemys;
        public Vector3 position;
        public int[] enemyLevels;

        public void FieldToBattle(SOBattleCharacter _player, SOBattleCharacter _npc, SOBattleCharacter[] _enemys, Vector3 _position, int[] _levels)
        {
            this.currentPlayer = _player;
            this.currentNPC = _npc;
            this.enemys = _enemys;
            this.position = _position;
            this.enemyLevels = _levels;
        }
        public void BattleToField(SOBattleCharacter _player, SOBattleCharacter _npc, int _playerHp, int _npcHp)
        {
            this.currentPlayer = _player;
            this.currentNPC = _npc;
            this.currentNPCHp = _npcHp;
            this.currentPlayerHp = _playerHp;
        }

        public void Reset()
        {
            this.currentPlayer = null;
            this.currentNPC = null;
            this.enemys = null;
        }
        public SOBattleCharacter[] GetEnemys() => this.enemys;
        public int[] GetEnemyLevels() => this.enemyLevels;
        public SOBattleCharacter GetPlayerData() => this.currentPlayer;
        public SOBattleCharacter GetNPCData() => this.currentNPC;

        public int GetPlayerHp() => this.currentPlayerHp;

        public int GetNpcHp() => this.currentNPCHp;
    }
    public void SetPlayer(GameObject _player)
    {
        this.player = _player;
    }

}
public enum GameState
{
    None,
    Main,
    Loading,
    Field,
    Pause,
    Battle
}