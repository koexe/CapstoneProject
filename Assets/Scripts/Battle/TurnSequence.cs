using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading;

[System.Serializable]
public class TurnSequence
{
    public enum BattleSequenceType
    {
        InitializeSequence,
        ChooseSequence,
        ExecuteSequence,
        SummarySequence,
    }

    protected Action BeforeSequence;
    protected Action AfterSequence;
    protected BattleManager battleManager;
    protected BattleSequenceType sequenceType;



    public SequenceState currentState = SequenceState.Initialize;
    public TurnSequence(BattleManager _battleManager, Action _beforeAction = null, Action _afterAction = null)
    {
        this.BeforeSequence = null;
        this.AfterSequence = null;

        this.currentState = SequenceState.BeforeAction;
        this.battleManager = _battleManager;

        this.BeforeSequence += _beforeAction;
        this.AfterSequence += _afterAction;

    }
    public virtual void SequenceUpdate()
    {

    }

    public virtual void SequenceAction()
    {
        this.currentState = SequenceState.InAction;

    }

    public void ExecuteBeforeAction()
    {
        BeforeSequence?.Invoke();
        this.currentState = SequenceState.InAction;
    }
    public void ExecuteAfterAction()
    {
        AfterSequence?.Invoke();
        this.currentState = SequenceState.Done;
    }

    public enum SequenceState
    {
        Initialize,
        BeforeAction,
        InAction,
        AfterAction,
        Done,
    }
}

[System.Serializable]
public class InitializeSequence : TurnSequence
{

    public InitializeSequence(BattleManager _battleManager, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.sequenceType = BattleSequenceType.InitializeSequence;

        this.battleManager.SetCurrentSequenceType(this.sequenceType);
    }

    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("초기화 진행중");

    }
}

[System.Serializable]
public class ChooseSequence : TurnSequence
{
    public enum ChooseState
    {
        SelectSkill,
        SelectEnemy,
        None,
    }
    BattleCharacterBase[] players;
    BattleCharacterBase[] enemys;

    public ChooseState state;

    int currentPlayerIndex;

    int currentEnemyIndex;
    public ChooseSequence(BattleManager _battleManager, BattleCharacterBase[] _players, BattleCharacterBase[] _enemys, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.players = _players;
        this.sequenceType = BattleSequenceType.ChooseSequence;
        this.state = ChooseState.None;
        this.enemys = _enemys;
        this.battleManager.SetCurrentSequenceType(this.sequenceType);
    }

    public override void SequenceUpdate()
    {
        base.SequenceUpdate();
        if (this.state == ChooseState.None)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.currentPlayerIndex += 1;
                if (this.currentPlayerIndex >= this.players.Length) this.currentPlayerIndex = 0;
                GameStatics.instance.CameraController.SetTarget(this.players[this.currentPlayerIndex].transform);
                this.battleManager.SetSelectedCharacter(this.players[this.currentPlayerIndex]);

            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.currentPlayerIndex -= 1;
                if (this.currentPlayerIndex < 0) this.currentPlayerIndex = 1;
                GameStatics.instance.CameraController.SetTarget(this.players[this.currentPlayerIndex].transform);
                this.battleManager.SetSelectedCharacter(this.players[this.currentPlayerIndex]);
            }


            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.TryGetComponent<BattleCharacterBase>(out var t_character))
                    {
                        if (this.players.Contains(t_character))
                        {
                            this.currentPlayerIndex = System.Array.IndexOf(this.players, t_character);
                            GameStatics.instance.CameraController.SetTarget(hit.transform);
                            this.battleManager.SetSelectedCharacter(t_character);
                        }
                    }
                }
            }
        }
        else if (this.state == ChooseState.SelectSkill)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.battleManager.HidePlayerAction();
                this.state = ChooseState.None;
            }
        }
        else if (this.state == ChooseState.SelectEnemy)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.currentEnemyIndex += 1;
                if (this.currentEnemyIndex >= this.enemys.Length) this.currentEnemyIndex = 0;
                GameStatics.instance.CameraController.SetTarget(this.enemys[this.currentEnemyIndex].transform);
                this.battleManager.SetSelectedCharacter(this.enemys[this.currentEnemyIndex]);

            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.currentEnemyIndex -= 1;
                if (this.currentEnemyIndex < 0) this.currentEnemyIndex = this.enemys.Length - 1;
                GameStatics.instance.CameraController.SetTarget(this.enemys[this.currentEnemyIndex].transform);
                this.battleManager.SetSelectedCharacter(this.enemys[this.currentEnemyIndex]);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.players[this.currentPlayerIndex].GetSelectedSkill().target = new BattleCharacterBase[] { this.enemys[this.currentEnemyIndex] };
                GameStatics.instance.CameraController.SetTarget(null);
                this.state = ChooseState.None;
                this.battleManager.CheckAllReady();
            }
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.TryGetComponent<BattleCharacterBase>(out var t_character))
                    {
                        if (this.enemys.Contains(t_character))
                        {
                            GameStatics.instance.CameraController.SetTarget(hit.transform);

                            this.currentEnemyIndex = System.Array.IndexOf(this.players, t_character);

                            this.players[this.currentPlayerIndex].GetSelectedSkill().target = new BattleCharacterBase[] { t_character };
                            GameStatics.instance.CameraController.SetTarget(null);
                            this.state = ChooseState.None;
                            this.battleManager.CheckAllReady();
                        }

                    }
                }
            }
        }

    }

    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("플레이어 행동 선택");
    }
}
[System.Serializable]
public class ExecuteSequence : TurnSequence
{
    List<BattleCharacterBase> battleCharacters;
    public ExecuteSequence(BattleManager _battleManager, List<BattleCharacterBase> _battleCharacters, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.battleCharacters = _battleCharacters;

        this.sequenceType = BattleSequenceType.ChooseSequence;

        this.battleManager.SetCurrentSequenceType(this.sequenceType);
    }
    public override void SequenceAction()
    {
        base.SequenceAction();
        ActionTask();
    }
    public async void ActionTask()
    {
        for (int i = 0; i < battleCharacters.Count; i++)
        {
            if (battleCharacters[i].GetAction() == CharacterActionType.Defence)
            {
                GameStatics.Swap(battleCharacters, i, 0);
            }
        }

        foreach (var t_battleCharacter in battleCharacters)
        {
            await t_battleCharacter.StartAction();
        }

        battleManager.NextSequence();

    }
}

[System.Serializable]
public class SummarySequence : TurnSequence
{
    List<BattleCharacterBase> battleCharacters;
    public SummarySequence(BattleManager _battleManager, List<BattleCharacterBase> _battleCharacters, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.battleCharacters = _battleCharacters;
        this.sequenceType = BattleSequenceType.SummarySequence;

        this.battleManager.SetCurrentSequenceType(this.sequenceType);
    }
    public override void SequenceAction()
    {
        base.SequenceAction();
        SummaryTask();
    }
    async void SummaryTask()
    {
        foreach (var t_battleCharacter in battleCharacters)
        {
            await t_battleCharacter.Summary();
        }

        battleManager.NextSequence();


    }
}