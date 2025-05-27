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

    [SerializeField] MapManager mapManager;

    [SerializeField] SOBattleCharacter currentPlayer;
    [SerializeField] SOBattleCharacter currentNPC;

    [SerializeField] CameraController cameraController;

    [SerializeField] FieldManager fieldManager;
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
        await this.saveGameManager.Initialization();
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
        this.MapInitialization();
        MapManager.instance.OnChangeToFieldScene();
    }


    public async void ChangeSceneFieldToBattle(SOBattleCharacter[] enemys, int[] _levels)
    {
        this.onChangeBattleSceneData.Set(this.currentPlayer, this.currentNPC, enemys, this.player.transform.position, _levels);
        await this.sceneLoadManager.LoadScene_Async("BattleScene");
        MapManager.instance.OnChangeToBattleScene();
    }
    public async void ChangeSceneBattleToField(SOBattleCharacter _currentPlayer, SOBattleCharacter _currentNpc)
    {
        this.currentNPC = _currentNpc;
        this.currentPlayer = _currentPlayer;
        this.player.transform.position = this.onChangeBattleSceneData.position;
        this.onChangeBattleSceneData.Reset();
        await this.sceneLoadManager.LoadScene_Async("FieldScene");
        MapManager.instance.OnChangeToFieldScene();
        this.cameraController.SetTarget(this.player.transform);
    }

    public async void LoadSaveData()
    {
        this.mapManager.LoadMap(this.saveGameManager.GetCurrentSaveData().currentMap);
        await this.sceneLoadManager.LoadScene_Async("FieldScene");
        this.fieldManager.Initialization();
        this.MapInitialization();
        MapManager.instance.OnChangeToFieldScene();
    }

    public class OnChangeBattleSceneData
    {
        [SerializeField] SOBattleCharacter currentPlayer;
        [SerializeField] SOBattleCharacter currentNPC;
        SOBattleCharacter[] enemys;
        public Vector3 position;
        public int[] enemyLevels;

        public void Set(SOBattleCharacter _player, SOBattleCharacter _npc, SOBattleCharacter[] _enemys, Vector3 _position, int[] _levels)
        {
            this.currentPlayer = _player;
            this.currentNPC = _npc;
            this.enemys = _enemys;
            this.position = _position;
            this.enemyLevels = _levels;
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
    Ingame,
    Pause,
    Battle
}