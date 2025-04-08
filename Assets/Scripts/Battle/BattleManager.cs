using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI battleText;
    TurnSystem turnSystem;

    public void ShowText(string _text)
    {
        this.battleText.text = _text;
    }
    private void Awake()
    {
        turnSystem = new TurnSystem();
        InitializeTurnSystem();
        this.turnSystem.SequenceAction();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            this.turnSystem.SequenceAction();
        }

    }

    void InitializeTurnSystem()
    {
        this.turnSystem.Initialization();
        this.turnSystem.AddSequence(new InitializeSequence(this));
        this.turnSystem.AddSequence(new ChooseSequence(this));
        this.turnSystem.AddSequence(new ExecuteSequence(this));
    }
}
