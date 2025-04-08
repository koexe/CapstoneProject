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

    [SerializeField] SequenceState currentState = SequenceState.Initialize;
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

    enum SequenceState
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
    BattleCharacterBase[] players;
    int currentIndex;
    public ChooseSequence(BattleManager _battleManager, BattleCharacterBase[] _players, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {
        this.players = _players;
    }

    public override void SequenceUpdate()
    {
        base.SequenceUpdate();
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex += 1;
            if (currentIndex >= players.Length) currentIndex = 0;
            GameStatics.instance.CameraController.SetTarget(players[currentIndex].transform);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex -= 1;
            if (currentIndex < 0) currentIndex = 1;
            GameStatics.instance.CameraController.SetTarget(players[currentIndex].transform);
        }
    }

    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("플레이어 행동 선택");

    }
}

public class ExecuteSequence : TurnSequence
{
    public ExecuteSequence(BattleManager _battleManager, List<BattleCharacterBase> _battleCharacters, Action _beforeAction = null, Action _afterAction = null) : base(_battleManager, _beforeAction, _afterAction)
    {

    }

    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("각 행동 실행");

    }
}