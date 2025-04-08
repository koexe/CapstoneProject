using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI battleText;
    TurnSystem turnSystem;

    [SerializeField] BattleCharacterBase[] allyCharacters;
    [SerializeField] BattleCharacterBase[] enemyCharacters;

    [SerializeField] GameObject selectButtonGroup;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.turnSystem.SequenceAction();
        }
        this.turnSystem.UpdateTurn();
    }

    void InitializeTurnSystem()
    {
        this.turnSystem.Initialization();
        this.turnSystem.AddSequence(new InitializeSequence(this));
        this.turnSystem.AddSequence(new ChooseSequence(this, this.allyCharacters,
            () => ActiveSelectButtonGroup(true),
            () =>
            {
                ActiveSelectButtonGroup(false);
                GameStatics.instance.CameraController.SetTarget(null);
            }));
        this.turnSystem.AddSequence(new ExecuteSequence(this, TurnCheck()));
    }

    List<BattleCharacterBase> TurnCheck()
    {
        List<BattleCharacterBase> t_BattleOrder = new List<BattleCharacterBase>();

        foreach (var t_character in this.allyCharacters)
        {
            t_BattleOrder.Add(t_character);
        }
        foreach (var t_character in enemyCharacters)
        {
            t_BattleOrder.Add(t_character);
        }
        t_BattleOrder.Sort((a, b) => b.speed.CompareTo(a.speed));
        return t_BattleOrder;
    }

    public void ActiveSelectButtonGroup(bool _is) => this.selectButtonGroup.SetActive(_is);

    public void SelectPlayerAction()
    {

    }
}
