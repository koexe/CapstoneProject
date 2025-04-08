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
        this.currentSequence = turn.Dequeue();
        this.currentSequence.SequenceAction();
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
