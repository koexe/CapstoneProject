using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Spine.Unity.Examples;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI battleText;
    [SerializeField] TurnSystem turnSystem;

    [SerializeField] BattleCharacterBase[] allyCharacters;
    [SerializeField] BattleCharacterBase[] enemyCharacters;

    [SerializeField] GameObject selectButtonGroup;

    [SerializeField] BattleCharacterBase currentSelectedCharacter;

    [SerializeField] GameObject nextButton;

    [SerializeField] TurnSequence.BattleSequenceType currentSequenceType;

    public void SetCurrentSequenceType(TurnSequence.BattleSequenceType _type) => this.currentSequenceType = _type;

    public void SetSelectedCharacter(BattleCharacterBase _character) => this.currentSelectedCharacter = _character;

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
        foreach(var t_char in this.allyCharacters)
        {
            t_char.SetAction(CharacterActionType.None);
        }
        //foreach (var t_char in this.enemyCharacters)
        //{
        //    t_char.SetAction(CharacterActionType.None);
        //}
        this.turnSystem.AddSequence(new InitializeSequence(this, _beforeAction: () =>
        {
            foreach (var t_character in this.allyCharacters)
                t_character.ResetSelectedSkill();
            foreach (var t_character in this.enemyCharacters)
                t_character.ResetSelectedSkill();
        }));
        this.turnSystem.AddSequence(new ChooseSequence(this, this.allyCharacters, this.enemyCharacters,
            () =>
            {
                ActiveSelectButtonGroup(true);
                this.nextButton.SetActive(false);
                this.currentSelectedCharacter = this.allyCharacters[0];
                GameStatics.instance.CameraController.SetTarget(this.currentSelectedCharacter.transform);
            },
            () =>
            {
                ActiveSelectButtonGroup(false);
                GameStatics.instance.CameraController.SetTarget(null);
            }));
        this.turnSystem.AddSequence(new ExecuteSequence(this, TurnCheck()));
        this.turnSystem.AddSequence(new SummarySequence(this, TurnCheck()));
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


    #endregion

    void EnemyAttackSelect()
    {
        foreach (var t_character in this.enemyCharacters)
        {
            t_character.SetSelectedSkill(t_character.GetSkills()[0], GetRandomElementAsArray<BattleCharacterBase>(this.allyCharacters));
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

    T[] GetRandomElementAsArray<T>(T[] sourceArray)
    {
        if (sourceArray == null || sourceArray.Length == 0)
            return new T[0]; // 빈 배열 반환

        int randomIndex = Random.Range(0, sourceArray.Length); // UnityEngine.Random
        return new T[] { sourceArray[randomIndex] };
    }

    #region ChooseSequence
    public void CheckAllReady()
    {
        GameStatics.instance.CameraController.SetTarget(null);

        var t_Sequence = this.turnSystem.GetCurrentSequence() as ChooseSequence;
        if (t_Sequence != null)
        {
            t_Sequence.state = ChooseSequence.ChooseState.None;
        }
        bool t_isAllReady = true;
        foreach (var t_character in this.allyCharacters)
        {
            if (t_character.IsReady() == false)
            {
                t_isAllReady = false;
                break;
            }
        }

        if (t_isAllReady == true)
        {
            EnemyAttackSelect();
            this.nextButton.SetActive(true);
        }
        else
        {
            return;
        }
    }

   

    public void SelectPlayerAction(int _type)
    {
        var t_Sequence = this.turnSystem.GetCurrentSequence() as ChooseSequence;
        if (t_Sequence != null)
        {
            t_Sequence.state = ChooseSequence.ChooseState.SelectSkill;
        }

        switch ((CharacterActionType)_type)
        {
            case CharacterActionType.Attack:
                this.currentSelectedCharacter.SetAction(CharacterActionType.Skill);
                UIManager.instance.ShowUI<SkillSelectUI>(new SkillUIData()
                {
                    battleCharacterBase = this.currentSelectedCharacter,
                    identifier = "BattleSkillUI",
                    onHide = () => CheckAllReady(),
                    skills = this.currentSelectedCharacter.GetSkills(),
                    isAllowMultifle = false,
                    order = 0,
                    action = UISelectSkill
                });
                break;
            case CharacterActionType.Talk:
                this.currentSelectedCharacter.SetAction(CharacterActionType.Talk);
                CheckAllReady();
                break;
            case CharacterActionType.Item:
                this.currentSelectedCharacter.SetAction(CharacterActionType.Item);
                CheckAllReady();
                break;
            case CharacterActionType.Run:
                this.currentSelectedCharacter.SetAction(CharacterActionType.Run);
                CheckAllReady();
                break;
        }
    }
    void UISelectSkill(BattleCharacterBase _character, SOSkillBase _skill)
    {
        UIManager.instance.HideUI("BattleSkillUI");

        switch (_skill.attackRange)
        {
            case SOSkillBase.AttackRangeType.All:
                _character.SetSelectedSkill(_skill, enemyCharacters);
                SetChooseSequenceState(ChooseSequence.ChooseState.None);
                break;
            case SOSkillBase.AttackRangeType.Random:
                _character.SetSelectedSkill(_skill, GetRandomElementAsArray<BattleCharacterBase>(this.enemyCharacters));
                SetChooseSequenceState(ChooseSequence.ChooseState.None);
                break;
            case SOSkillBase.AttackRangeType.Select:
                GameStatics.instance.CameraController.SetTarget(null);
                _character.SetSelectedSkill(_skill, null);
                ShowText("적 선택");
                SetChooseSequenceState(ChooseSequence.ChooseState.SelectEnemy);
                break;
            case SOSkillBase.AttackRangeType.Ally:
                ShowText("아군 선택");
                break;
        }
    }


    void SetChooseSequenceState(ChooseSequence.ChooseState _state)
    {
        var t_Sequence = this.turnSystem.GetCurrentSequence() as ChooseSequence;
        if (t_Sequence != null)
        {
            t_Sequence.state = _state;
        }
    }
    #endregion
    #endregion
}
public enum CharacterActionType
{
    None,
    Attack,
    Defence,
    Talk,
    Skill,
    Item,
    Run,
}