using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurnSequence
{
    protected Action BeforeSequence;
    protected Action AfterSequence;
    protected BattleManager battleManager;

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


public class InitializeSequence : TurnSequence
{
    public InitializeSequence(BattleManager _battleManager, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {

    }

    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("초기화 진행중");

    }
}


public class ChooseSequence : TurnSequence
{
    enum ChooseState
    {

    }
    BattleCharacterBase[] players;
    int currentPlayerIndex;
    public ChooseSequence(BattleManager _battleManager, BattleCharacterBase[] _players, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.players = _players;
    }

    public override void SequenceUpdate()
    {
        base.SequenceUpdate();
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.currentPlayerIndex += 1;
            if (this.currentPlayerIndex >= this.players.Length) this.currentPlayerIndex = 0;
            GameStatics.instance.CameraController.SetTarget(this.players[currentPlayerIndex].transform);
            this.battleManager.SetSelectedCharacter(this.players[currentPlayerIndex]);

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.currentPlayerIndex -= 1;
            if (this.currentPlayerIndex < 0) this.currentPlayerIndex = 1;
            GameStatics.instance.CameraController.SetTarget(this.players[currentPlayerIndex].transform);
            this.battleManager.SetSelectedCharacter(this.players[currentPlayerIndex]);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            this.battleManager.HidePlayerAction();
        }
    }


    public BattleCharacterBase GetSelectedPlayer() => this.players[this.currentPlayerIndex];


    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("플레이어 행동 선택");
    }
}

public class ExecuteSequence : TurnSequence
{
    List<BattleCharacterBase> battleCharacters;
    public ExecuteSequence(BattleManager _battleManager, List<BattleCharacterBase> _battleCharacters, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.battleCharacters = _battleCharacters;

    }
    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.StartCoroutine(ActionCoroutine());
    }
    IEnumerator ActionCoroutine()
    {
        foreach(var t_battleCharacter in battleCharacters)
        {
            t_battleCharacter.StartAction();
            while(!t_battleCharacter.IsActionDone())
            {
                yield return CoroutineUtil.WaitForFixedUpdate;
            }

        }

        battleManager.NextSequence();

        yield break;
    }
}