using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleCharacterManager
{
    private List<BattleCharacterBase> allyCharacters = new List<BattleCharacterBase>();
    private List<BattleCharacterBase> enemyCharacters = new List<BattleCharacterBase>();
    
    public BattleCharacterBase[] GetAllies() => allyCharacters.ToArray();
    public BattleCharacterBase[] GetEnemies() => enemyCharacters.ToArray();
    
    public void AddAlly(BattleCharacterBase character) => allyCharacters.Add(character);
    public void AddEnemy(BattleCharacterBase character) => enemyCharacters.Add(character);
    
    public bool IsAllyAllDie()
    {
        foreach (var ally in allyCharacters)
        {
            if (!ally.IsDie()) return false;
        }
        return true;
    }
    
    public bool IsEnemyAllDie()
    {
        foreach (var enemy in enemyCharacters)
        {
            if (!enemy.IsDie()) return false;
        }
        return true;
    }
    
    public void Clear()
    {
        allyCharacters.Clear();
        enemyCharacters.Clear();
    }

    public int AllyCount => allyCharacters.Count;
    public int EnemyCount => enemyCharacters.Count;
    
    public BattleCharacterBase GetAlly(int index) => allyCharacters[index];
}

public class BattleUIManager
{
    private TextMeshProUGUI battleText;
    private GameObject selectButtonGroup;
    private GameObject nextButton;
    
    public BattleUIManager(TextMeshProUGUI battleText, GameObject selectButtonGroup, GameObject nextButton)
    {
        this.battleText = battleText;
        this.selectButtonGroup = selectButtonGroup;
        this.nextButton = nextButton;
    }
    
    public void ShowText(string text) => battleText.text = text;
    public void SetSelectButtonGroupActive(bool isActive) => selectButtonGroup.SetActive(isActive);
    public void SetNextButtonActive(bool isActive) => nextButton.SetActive(isActive);
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI battleText;
    [SerializeField] TurnSystem turnSystem;

    [SerializeField] GameObject battleCharacterPrefab;
    [SerializeField] GameObject enemyCharacterPrefab;

    [SerializeField] GameObject selectButtonGroup;

    [SerializeField] BattleCharacterBase currentSelectedCharacter;

    [SerializeField] GameObject nextButton;

    [SerializeField] TurnSequence.BattleSequenceType currentSequenceType;

    [SerializeField] Transform[] enemyPositions;
    [SerializeField] Transform[] allyPositions;
    [SerializeField] Transform middlePoint;

    [SerializeField] int runStack = 0;
    [SerializeField] bool isUsedRunCommend;

    private BattleCharacterManager characterManager;
    private BattleUIManager uiManager;

    private void Awake()
    {
        characterManager = new BattleCharacterManager();
        uiManager = new BattleUIManager(battleText, selectButtonGroup, nextButton);
    }

    public void SetCurrentSequenceType(TurnSequence.BattleSequenceType _type) => this.currentSequenceType = _type;

    public void SetSelectedCharacter(BattleCharacterBase _character) => this.currentSelectedCharacter = _character;

    public Transform GetMiddlePoint() => this.middlePoint;

    public Transform GetPlayerTranform(BattleCharacterBase _battleCharacterBase)
    {
        var allies = characterManager.GetAllies();
        int t_index = Array.IndexOf(allies, _battleCharacterBase);
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
        characterManager.Clear();
        
        var battleSceneData = GameManager.instance.GetBattleSceneData();
        if (battleSceneData == null)
        {
            Debug.LogError("BattleSceneData가 null입니다. 전투를 초기화할 수 없습니다.");
            return;
        }

        var playerData = battleSceneData.GetPlayerData();
        var npcData = battleSceneData.GetNPCData();
        var enemyData = battleSceneData.GetEnemys();

        if (playerData == null || npcData == null)
        {
            Debug.LogError("플레이어 또는 NPC 데이터가 null입니다.");
            return;
        }

        if (enemyData == null)
        {
            Debug.LogError("적 데이터가 null입니다.");
            return;
        }

        SOBattleCharacter[] t_allys = new SOBattleCharacter[2] { playerData, npcData };

        try
        {
            for (int i = 0; i < t_allys.Length; i++)
            {
                if (t_allys[i] == null)
                {
                    Debug.LogError($"아군 데이터 {i}가 null입니다.");
                    continue;
                }

                var ally = Instantiate(battleCharacterPrefab);
                if (ally == null)
                {
                    Debug.LogError("battleCharacterPrefab이 null입니다.");
                    continue;
                }

                var allyComponent = ally.GetComponent<BattleCharacterBase>();
                if (allyComponent == null)
                {
                    Debug.LogError("BattleCharacterBase 컴포넌트를 찾을 수 없습니다.");
                    Destroy(ally);
                    continue;
                }

                allyComponent.Initialization(this, t_allys[i]);
                ally.transform.position = allyPositions[i].position;
                characterManager.AddAlly(allyComponent);
            }
            
            for (int i = 0; i < enemyData.Length; i++)
            {
                if (enemyData[i] == null)
                {
                    Debug.LogError($"적 데이터 {i}가 null입니다.");
                    continue;
                }

                var enemy = Instantiate(enemyCharacterPrefab);
                if (enemy == null)
                {
                    Debug.LogError("enemyCharacterPrefab이 null입니다.");
                    continue;
                }

                var enemyComponent = enemy.GetComponent<BattleCharacterBase>();
                if (enemyComponent == null)
                {
                    Debug.LogError("BattleCharacterBase 컴포넌트를 찾을 수 없습니다.");
                    Destroy(enemy);
                    continue;
                }

                enemyComponent.Initialization(this, enemyData[i]);
                enemy.transform.position = enemyPositions[i].position;
                characterManager.AddEnemy(enemyComponent);
            }

            turnSystem = new TurnSystem();
            InitializeTurnSystem();
            this.turnSystem.SequenceAction();
        }
        catch (Exception e)
        {
            Debug.LogError($"전투 초기화 중 오류 발생: {e.Message}\n{e.StackTrace}");
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
        foreach (var t_char in characterManager.GetAllies())
        {
            t_char.SetAction(CharacterActionType.None);
        }
        this.turnSystem.AddSequence(new InitializeSequence(this, _beforeAction: () =>
        {
            foreach (var t_character in characterManager.GetAllies())
                t_character.ResetSelectedSkill();
            foreach (var t_character in characterManager.GetEnemies())
                t_character.ResetSelectedSkill();
        }));
        this.turnSystem.AddSequence(new ChooseSequence(this, characterManager.GetAllies(), characterManager.GetEnemies(),
            () =>
            {
                ActiveSelectButtonGroup(true);
                this.nextButton.SetActive(false);
                this.currentSelectedCharacter = characterManager.GetAlly(0);
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
        uiManager.ShowText(_text);
    }

    public void ActiveSelectButtonGroup(bool _is) => uiManager.SetSelectButtonGroupActive(_is);

    public void HidePlayerAction()
    {
        UIManager.instance.HideUI("BattleSkillUI");
    }
    public bool IsAllyAllDie()
    {
        return characterManager.IsAllyAllDie();
    }

    public bool IsEnemyAllDie()
    {
        return characterManager.IsEnemyAllDie();
    }

    public async UniTask RunCheck()
    {

        float finalChance = 0f;

        float baseChance = 0.5f; // 50%
        float t_allyLevel = 0f;
        foreach (var t_ally in characterManager.GetAllies())
        {
            t_allyLevel += t_ally.GetStatus().GetLevel();
        }
        t_allyLevel = t_allyLevel / characterManager.AllyCount;
        float t_enemyLevel = 0f;
        foreach (var t_enemy in characterManager.GetEnemies())
        {
            t_enemyLevel += t_enemy.GetStatus().GetLevel();
        }
        t_enemyLevel /= characterManager.EnemyCount;


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
        foreach (var t_character in characterManager.GetEnemies())
        {
            t_character.SetSelectedSkill(t_character.GetSkills()[0], characterManager.GetAllies());
        }
    }

    List<BattleCharacterBase> TurnCheck()
    {
        List<BattleCharacterBase> t_BattleOrder = new List<BattleCharacterBase>();

        foreach (var t_character in characterManager.GetAllies())
        {
            if (!t_character.IsDie())
                t_BattleOrder.Add(t_character);
        }
        foreach (var t_character in characterManager.GetEnemies())
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
        GameManager.instance.ChangeSceneToField(characterManager.GetAllies()[0].GetBattleCharacter(), characterManager.GetAllies()[1].GetBattleCharacter());
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
        foreach (var t_character in characterManager.GetAllies())
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
            uiManager.SetNextButtonActive(true);
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
                _character.SetSelectedSkill(_skill, characterManager.GetEnemies());
                SetChooseSequenceState(ChooseSequence.ChooseState.None);
                CheckAllReady();
                break;
            case SOSkillBase.AttackRangeType.Random:
                _character.SetSelectedSkill(_skill, GetRandomElementAsArray<BattleCharacterBase>(characterManager.GetEnemies()));
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
        foreach (var t_character in characterManager.GetAllies())
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