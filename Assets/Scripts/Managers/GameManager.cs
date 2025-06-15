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
    }

    public async UniTask ChangeSceneMainToField()
    {
        await this.sceneLoadManager.LoadScene_Async("FieldScene");
        this.fieldManager.Initialization();
        ChangeGameState(GameState.Field);
        MapInitialization();
        MapManager.instance.OnChangeToFieldScene();
    }

    public async void ChangeSceneFieldToBattle(SOBattleCharacter[] enemys, int[] _levels, Action<bool> _onBattleEnd = null)
    {
        ChangeGameState(GameState.Battle);
        this.player.GetComponent<PlayerInputModule>().ShowEncounterUI();
        this.onChangeBattleSceneData.FieldToBattle(
            this.currentPlayer, this.currentNPC, enemys, _levels, _onBattleEnd);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        MapManager.instance.OnChangeToBattleScene();
        await this.sceneLoadManager.LoadScene_Async("BattleScene");
    }

    public async void ChangeSceneBattleToField()
    {
        ChangeGameState(GameState.Field);
        this.player.GetComponent<PlayerInputModule>().HideEncounterUI();
        await this.sceneLoadManager.LoadScene_Async("FieldScene");
        this.cameraController.SetTarget(this.player.transform);
        this.cameraController.SetPosition(this.player.transform.position);
        this.onChangeBattleSceneData.OnBattleEnd?.Invoke(true);
        this.onChangeBattleSceneData.Reset();
        MapManager.instance.OnChangeToFieldScene();
    }

    public void UpdatePlayerData(SOBattleCharacter _currentPlayer)
    {
        this.currentPlayer = _currentPlayer;
    }

    public void UpdateNPCData(SOBattleCharacter _currentNPC)
    {
        this.currentNPC = _currentNPC;
    }

    public async UniTask GameOver()
    {
        Destroy(this.player);   
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
        SOBattleCharacter[] enemys;
        public int[] enemyLevels;

        public Action<bool> OnBattleEnd;

        public void FieldToBattle(SOBattleCharacter _player, SOBattleCharacter _npc, SOBattleCharacter[] _enemys, int[] _levels, Action<bool> _onBattleEnd)
        {
            this.currentPlayer = _player;
            this.currentNPC = _npc;
            this.enemys = _enemys;
            this.enemyLevels = _levels;
            this.OnBattleEnd = _onBattleEnd;
        }
        public void BattleToField(SOBattleCharacter _player, SOBattleCharacter _npc)
        {
            this.currentPlayer = _player;
            this.currentNPC = _npc;
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