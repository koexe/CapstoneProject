using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI battleText;
    [SerializeField] TurnSystem turnSystem;

    [SerializeField] GameObject battleCharacterPrefab;
    [SerializeField] GameObject enemyCharacterPrefab;

    [SerializeField] BattleCharacterBase[] allyCharacters;
    [SerializeField] BattleCharacterBase[] enemyCharacters;

    [SerializeField] GameObject selectButtonGroup;

    [SerializeField] BattleCharacterBase currentSelectedCharacter;

    [SerializeField] GameObject nextButton;

    [SerializeField] TurnSequence.BattleSequenceType currentSequenceType;

    [SerializeField] Transform[] enemyPositions;
    [SerializeField] Transform[] allyPositions;
    [SerializeField] Transform middlePoint;

    [SerializeField] int runStack = 0;
    [SerializeField] bool isUsedRunCommend;

    public void SetCurrentSequenceType(TurnSequence.BattleSequenceType _type) => this.currentSequenceType = _type;

    public void SetSelectedCharacter(BattleCharacterBase _character) => this.currentSelectedCharacter = _character;

    public Transform GetMiddlePoint() => this.middlePoint;

    public Transform GetPlayerTranform(BattleCharacterBase _battleCharacterBase)
    {
        int t_index = Array.FindIndex(this.allyCharacters, x => x == _battleCharacterBase); // → 2
        return this.allyPositions[t_index];
    }

#if UNITY_EDITOR
    [SerializeField] bool _isDebug;
#endif


    private void Start()
    {
        Initialization();
    }
    public void Initialization()
    {
        SOBattleCharacter[] t_allys = new SOBattleCharacter[2]
        {
            GameManager.instance.GetBattleSceneData().GetPlayerData(),
            GameManager.instance.GetBattleSceneData().GetNPCData()
        };
        SOBattleCharacter[] t_enemys = GameManager.instance.GetBattleSceneData().GetEnemys();

        this.allyCharacters = new BattleCharacterBase[t_allys.Length];
        this.enemyCharacters = new BattleCharacterBase[t_enemys.Length];

        for (int i = 0; i < t_allys.Length; i++)
        {
            this.allyCharacters[i] = Instantiate(this.battleCharacterPrefab).GetComponent<BattleCharacterBase>();
            this.allyCharacters[i].Initialization(this, t_allys[i]);
            this.allyCharacters[i].transform.position = this.allyPositions[i].position;

        }
        for (int i = 0; i < t_enemys.Length; i++)
        {
            this.enemyCharacters[i] = Instantiate(this.enemyCharacterPrefab).GetComponent<BattleCharacterBase>();
            this.enemyCharacters[i].Initialization(this, t_enemys[i]);
            this.enemyCharacters[i].transform.position = this.enemyPositions[i].position;
        }

        turnSystem = new TurnSystem();
        InitializeTurnSystem();
        this.turnSystem.SequenceAction();
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
        foreach (var t_char in this.allyCharacters)
        {
            t_char.SetAction(CharacterActionType.None);
        }
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
                GameManager.instance.GetCamera().SetTarget(this.currentSelectedCharacter.transform);
            },
            () =>
            {
                ActiveSelectButtonGroup(false);
                GameManager.instance.GetCamera().SetTarget(null);
            }));
        this.turnSystem.AddSequence(new ExecuteSequence(this, TurnCheck()));
        this.turnSystem.AddSequence(new SummarySequence(this, TurnCheck(), _afterAction: TurnEnd));
        this.turnSystem.AddSequence(new EndSequence(this));
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
    public bool IsAllyAllDie()
    {
        foreach (var t_ally in this.allyCharacters)
        {
            if (!t_ally.IsDie())
                return false;
        }
        return true;
    }

    public bool IsEnemyAllDie()
    {
        foreach (var t_enemy in this.enemyCharacters)
        {
            if (!t_enemy.IsDie())
                return false;
        }
        return true;
    }

    public async UniTask RunCheck()
    {

        float finalChance = 0f;

        float baseChance = 0.5f; // 50%
        float t_allyLevel = 0f;
        foreach (var t_ally in this.allyCharacters)
        {
            t_allyLevel += t_ally.GetStatus().GetLevel();
        }
        t_allyLevel = t_allyLevel / this.allyCharacters.Length;
        float t_enemyLevel = 0f;
        foreach (var t_enemy in this.enemyCharacters)
        {
            t_enemyLevel += t_enemy.GetStatus().GetLevel();
        }
        t_enemyLevel /= this.enemyCharacters.Length;


        float levelDiff = t_allyLevel - t_enemyLevel;
        float levelModifier = 0f;

        // 레벨 차이 보정
        if (levelDiff >= 3)
            levelModifier = 0.15f;
        else if (levelDiff >= 1)
            levelModifier = 0.05f;
        else if (levelDiff <= -3)
            levelModifier = -0.25f;
        else if (levelDiff <= -1)
            levelModifier = -0.15f;

        // 연속 시도 페널티
        int penaltyCount = Mathf.Max(0, this.runStack);
        float penalty = penaltyCount * -0.15f;

        finalChance = baseChance + levelModifier + penalty;
        finalChance = Mathf.Clamp01(finalChance); // 0% ~ 100%

        this.runStack += 1;
        this.isUsedRunCommend = true;

        bool success = UnityEngine.Random.value < finalChance;
        if (success)
        {
            ShowText("도망에 성공했다!");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            ChangeToFieldScene();
            await UniTask.Delay(TimeSpan.FromSeconds(10000f));
        }
        else
        {
            ShowText("도망에 실패했다!");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
        }
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
            if (!t_character.IsDie())
                t_BattleOrder.Add(t_character);
        }
        foreach (var t_character in enemyCharacters)
        {
            if (!t_character.IsDie())
                t_BattleOrder.Add(t_character);
        }
        t_BattleOrder.Sort((a, b) => b.GetStat(StatType.Spd).CompareTo(a.GetStat(StatType.Spd)));


        return t_BattleOrder;
    }

    T[] GetRandomElementAsArray<T>(T[] sourceArray)
    {
        if (sourceArray == null || sourceArray.Length == 0)
            return new T[0]; // 빈 배열 반환

        int randomIndex = Random.Range(0, sourceArray.Length); // UnityEngine.Random
        return new T[] { sourceArray[randomIndex] };
    }

    void TurnEnd()
    {
        if (!this.isUsedRunCommend)
            this.runStack = 0;
        this.isUsedRunCommend = false;
    }


    public void ChangeToFieldScene()
    {
        GameManager.instance.ChangeSceneToField(this.allyCharacters[0].GetBattleCharacter(), this.allyCharacters[1].GetBattleCharacter());
    }



    #region ChooseSequence
    public void CheckAllReady()
    {
        GameManager.instance.GetCamera().SetTarget(null);
        ShowText("대기중");
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
                CheckAllReady();
                break;
            case SOSkillBase.AttackRangeType.Random:
                _character.SetSelectedSkill(_skill, GetRandomElementAsArray<BattleCharacterBase>(this.enemyCharacters));
                SetChooseSequenceState(ChooseSequence.ChooseState.None);
                CheckAllReady();
                break;
            case SOSkillBase.AttackRangeType.Select:
                GameManager.instance.GetCamera().SetTarget(null);
                _character.SetSelectedSkill(_skill, null);
                ShowText("적 선택");
                SetChooseSequenceState(ChooseSequence.ChooseState.SelectEnemy);
                break;
            case SOSkillBase.AttackRangeType.Ally:
                ShowText("아군 선택");
                CheckAllReady();
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


    public async UniTask GainExp(int _exp)
    {
        List<BattleCharacterBase> t_allyCharacter = new List<BattleCharacterBase>();
        foreach (var t_character in this.allyCharacters)
        {
            if (!t_character.IsDie())
                t_allyCharacter.Add(t_character);
        }
        foreach (var t_chracter in t_allyCharacter)
        {
            int t_Exp = _exp / t_allyCharacter.Count;
            ShowText($"{t_chracter.GetCharacterName()} 이 {t_Exp} 의 경험치를 얻었다!");

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            t_chracter.GainExp(t_Exp);
            while (t_chracter.CheckLevelup())
            {
                ShowText($"{t_chracter.GetCharacterName()} 의 레벨이 {t_chracter.GetStatus().GetLevel()}로 올랐다!");
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                t_chracter.LevelUp_UpdateStat();
            }

        }
    }
    #endregion
    #endregion
}
public enum CharacterActionType
{
    None = 0,
    Attack = 1,
    Defence = 5,
    Talk = 2,
    Skill = 6,
    Item = 3,
    Run = 4,
}