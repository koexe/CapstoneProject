using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem
{
    Queue<TurnSequence> turn;
    TurnSequence currentSequence;


    public void Initialization()
    {
        this.turn = new Queue<TurnSequence>();
    }
    public void SequenceAction()
    {
        if (this.currentSequence != null)
            this.currentSequence.ExecuteAfterAction();


        this.currentSequence = turn.Dequeue();
        this.currentSequence.ExecuteBeforeAction();


        this.currentSequence.SequenceAction();
    }

    public void UpdateTurn()
    {
        this.currentSequence.SequenceUpdate();
    }

    public void InitializeTurnSystem()
    {

    }

    public void AddSequence(TurnSequence turnSequence)
    {
        this.turn.Enqueue(turnSequence);
        return;
    }


}
