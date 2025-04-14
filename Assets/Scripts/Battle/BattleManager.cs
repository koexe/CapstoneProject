using System.Collections;
using System.Collections.Generic;
using Spine.Unity.Examples;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI battleText;
    TurnSystem turnSystem;

    [SerializeField] BattleCharacterBase[] allyCharacters;
    [SerializeField] BattleCharacterBase[] enemyCharacters;

    [SerializeField] GameObject selectButtonGroup;

    [SerializeField] BattleCharacterBase currentSelectedCharacter;

    public void SetSelectedCharacter(BattleCharacterBase _character) => this.currentSelectedCharacter = _character;
    public enum PlayerActionType
    {
        Attack,
        Talk,
        Item,
        Run,
    }

    private void Awake()
    {
        turnSystem = new TurnSystem();
        InitializeTurnSystem();
        this.turnSystem.SequenceAction();
        foreach (var t_character in this.allyCharacters)
        {
            t_character.Initialization(this);
        }
        foreach (var t_character in this.enemyCharacters)
        {
            t_character.Initialization(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextSequence();
        }
        this.turnSystem.UpdateTurn();
    }

    public void NextSequence()
    {
        if (this.turnSystem.SequenceAction())
        {
            return;
        }
        else
        {
            NewTurn();
            this.turnSystem.SequenceAction();
        }
    }

    void InitializeTurnSystem()
    {
        this.turnSystem.Initialization();
        NewTurn();
    }

    void NewTurn()
    {
        this.turnSystem.AddSequence(new InitializeSequence(this));
        this.turnSystem.AddSequence(new ChooseSequence(this, this.allyCharacters,
            () =>
            {
                ActiveSelectButtonGroup(true);
                this.currentSelectedCharacter = this.allyCharacters[0];
                GameStatics.instance.CameraController.SetTarget(this.currentSelectedCharacter.transform);
            },
            () =>
            {
                ActiveSelectButtonGroup(false);
                GameStatics.instance.CameraController.SetTarget(null);
            }));
        this.turnSystem.AddSequence(new ExecuteSequence(this, TurnCheck()));
    }

    #region 기능
    #region public
    public void ShowText(string _text)
    {
        this.battleText.text = _text;
    }

    public void ActiveSelectButtonGroup(bool _is) => this.selectButtonGroup.SetActive(_is);

    public void HidePlayerAction()
    {
        UIManager.instance.HideUI("BattleSkillUI");
    }

    public void SelectPlayerAction(int _type)
    {
        switch ((PlayerActionType)_type)
        {
            case PlayerActionType.Attack:
            case PlayerActionType.Talk:
            case PlayerActionType.Item:
            case PlayerActionType.Run:
                UIManager.instance.ShowUI<SkillSelectUI>(new SkillUIData()
                {
                    battleCharacterBase = this.currentSelectedCharacter,
                    identifier = "BattleSkillUI",
                    skills = this.currentSelectedCharacter.GetSkills(),
                    isAllowMultifle = false,
                    order = 0,
                    action = UISelectSkill
                });
                break;
        }
    }
    void UISelectSkill(BattleCharacterBase _character, SkillBase _skill)
    {
        _character.SetSelectedSkill(_skill);
        UIManager.instance.HideUI("BattleSkillUI");
        CheckAllReady();
        switch(_skill.attackRange)
        {
            case SkillBase.AttackRangeType.All:
            case SkillBase.AttackRangeType.Random:
                break;
            case SkillBase.AttackRangeType.Select:
                ShowText("적 선택");
                
                break;
            case SkillBase.AttackRangeType.Ally:
                break;
        }
    }

    public void CheckAllReady()
    {
        bool t_isAllReady = true;
        foreach (var t_character in this.allyCharacters)
        {
            if (t_character.GetSelectedSkill() == null)
            {
                t_isAllReady = false;
                break;
            }
        }

        if (t_isAllReady == true)
        {
            EnemyAttackSelect();
        }
        else
        {
            return;
        }
    }

    public void StartAction(List<BattleCharacterBase> battleCharacters, int index)
    {
        if (index >= battleCharacters.Count)
            return;
        battleCharacters[index].StartAction();

    }
    #endregion

    void EnemyAttackSelect()
    {
        foreach (var t_character in this.enemyCharacters)
        {
            //t_character.SetSelectedSkill(t_character.GetSkills()[Random.Range(0, t_character.GetSkills().Length)]);
            t_character.SetSelectedSkill(t_character.GetSkills()[0]);
        }
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
    #endregion
}
