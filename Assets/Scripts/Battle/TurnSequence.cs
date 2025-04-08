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
    public TurnSequence(BattleManager battleManager)
    {
        this.BeforeSequence = null;
        this.AfterSequence = null;

        this.currentState = SequenceState.BeforeAction;
        this.battleManager = battleManager;
    }

    public void AddBeforeSequence(Action _action)
    {
        BeforeSequence += _action;
    }

    public void AddAfterSequence(Action _action)
    {
        AfterSequence += _action;
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
    public InitializeSequence(BattleManager battleManager) : base(battleManager)
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
    public ChooseSequence(BattleManager battleManager) : base(battleManager)
    {

    }

    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("플레이어 행동 선택");

    }
}

public class ExecuteSequence : TurnSequence
{
    public ExecuteSequence(BattleManager battleManager) : base(battleManager)
    {

    }

    public override void SequenceAction()
    {
        base.SequenceAction();
        this.battleManager.ShowText("각 행동 실행");

    }
}