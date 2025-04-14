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
    public bool SequenceAction()
    {
        if (this.currentSequence != null)
            this.currentSequence.ExecuteAfterAction();

        if(this.turn.Count != 0)
        {
            this.currentSequence = turn.Dequeue();

            this.currentSequence.ExecuteBeforeAction();

            this.currentSequence.SequenceAction();
            return true;
        }
        else
        {
            return false;
        }

    }

    public void UpdateTurn()
    {
        this.currentSequence.SequenceUpdate();
    }
    public void AddSequence(TurnSequence turnSequence)
    {
        this.turn.Enqueue(turnSequence);
        return;
    }
}
