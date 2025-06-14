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

    public void RemoveAlly(BattleCharacterBase character)
    {
        if (this.allyCharacters.Contains(character))
            this.allyCharacters.Remove(character);
        else
            return;
    }
    public void RemoveEnemy(BattleCharacterBase character)
    {
        if (this.enemyCharacters.Contains(character))
            this.enemyCharacters.Remove(character);
        else
            return;
    }

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

    public BattleUIManager(TextMeshProUGUI battleText, GameObject selectButtonGroup)
    {
        this.battleText = battleText;
        this.selectButtonGroup = selectButtonGroup;
    }

    public void ShowText(string text) => battleText.text = text;
    public void SetSelectButtonGroupActive(bool isActive) => selectButtonGroup.SetActive(isActive);
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI battleText;
    [SerializeField] TurnSystem turnSystem;

    [SerializeField] GameObject battleCharacterPrefab;
    [SerializeField] GameObject enemyCharacterPrefab;

    [SerializeField] GameObject selectButtonGroup;

    [SerializeField] BattleCharacterBase currentSelectedCharacter;


    [SerializeField] TurnSequence.BattleSequenceType currentSequenceType;

    [SerializeField] Transform[] enemyPositions;
    [SerializeField] Transform[] allyPositions;
    [SerializeField] Transform middlePoint;

    [SerializeField] int runStack = 0;
    [SerializeField] bool isUsedRunCommend;

    private BattleCharacterManager characterManager;
    private BattleUIManager uiManager;

    [SerializeField] List<BattleCharacterBase> playerDataContainer;
    [SerializeField] List<BattleCharacterBase> enemyDataContainer;


    [SerializeField] GameObject playerUIGroup;
    [SerializeField] GameObject npcUIGroup;


    [SerializeField] TextMeshProUGUI playerHp;
    [SerializeField] TextMeshProUGUI playerMp;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI npcHp;
    [SerializeField] TextMeshProUGUI npcMp;
    [SerializeField] TextMeshProUGUI npcName;


    // 배틀 종료 이벤트
    public event Action<bool> OnBattleEnd; // bool은 보스전 여부

    private void Awake()
    {
        characterManager = new BattleCharacterManager();
        uiManager = new BattleUIManager(battleText, selectButtonGroup);
    }

    public void SetCurrentSequenceType(TurnSequence.BattleSequenceType _type) => this.currentSequenceType = _type;

    public void SetSelectedCharacter(BattleCharacterBase _character) => this.currentSelectedCharacter = _character;

    public Transform GetMiddlePoint() => this.middlePoint;

    public Transform GetOriginTranform(BattleCharacterBase _battleCharacterBase)
    {
        var allies = characterManager.GetAllies();
        var enemies = characterManager.GetEnemies();
        if (allies.Contains(_battleCharacterBase))
        {
            int t_index = Array.IndexOf(allies, _battleCharacterBase);
            return this.allyPositions[t_index];
        }
        else
        {
            int t_index = Array.IndexOf(enemies, _battleCharacterBase);
            return this.enemyPositions[t_index];
        }
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
        var enemyLevels = battleSceneData.GetEnemyLevels();

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
                if (SaveGameManager.instance.GetCurrentSaveData().isMetLemo == false && t_allys[i].GetCharacterName() == "리모")
                {
                    this.npcUIGroup.SetActive(false);
                    break;
                }
                else
                {
                    this.npcUIGroup.SetActive(true);
                }


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
                ally.transform.position = allyPositions[i].position;
                characterManager.AddAlly(allyComponent);
                playerDataContainer.Add(allyComponent);
                allyComponent.PlayerInitialization(this, t_allys[i], i == 0 ? (playerHp, playerMp, playerName) : (npcHp, npcMp, npcName));
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
                    Debug.LogError($"[{enemy.name}] BattleCharacterBase 컴포넌트를 찾을 수 없습니다.");
                    Destroy(enemy);
                    continue;
                }

                if (enemyData[i] == null)
                {
                    Debug.LogError($"[{enemy.name}] enemyData[{i}]가 null입니다.");
                    Destroy(enemy);
                    continue;
                }

                enemyDataContainer.Add(enemyComponent);

                enemy.transform.position = enemyPositions[i].position;
                characterManager.AddEnemy(enemyComponent);
                enemyComponent.EnemyInitialization(this, enemyData[i], enemyLevels[i]);
            }

            turnSystem = new TurnSystem();
            InitializeTurnSystem();
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
                this.selectButtonGroup.gameObject.SetActive(true);
                this.currentSelectedCharacter = characterManager.GetAlly(0);
                GameManager.instance.GetCamera().SetTarget(this.currentSelectedCharacter.transform);
                SetChooseSequenceState(ChooseSequence.ChooseState.None);
            },
            () =>
            {
                SetAllyArrow(false);
                SetEnemyArrow(false);
                SetAllyShadow(false);
                SetEnemyShadow(false);
                GameManager.instance.GetCamera().SetTarget(null);
            }));
        this.turnSystem.AddSequence(new ExecuteSequence(this, TurnCheck(), _beforeAction: () => this.selectButtonGroup.gameObject.SetActive(false)));
        this.turnSystem.AddSequence(new SummarySequence(this, TurnCheck(), _afterAction: TurnEnd));
        this.turnSystem.AddSequence(new EndSequence(this));

        this.turnSystem.SequenceAction();
    }

    public void CharacterDie(BattleCharacterBase _character)
    {
        this.characterManager.RemoveAlly(_character);
        this.characterManager.RemoveEnemy(_character);
    }

    #region 기능
    #region public
    public void ShowText(string _text)
    {
        uiManager.ShowText(_text);
    }


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

    public bool IsAlly(BattleCharacterBase _character)
    {
        return this.characterManager.GetAllies().Contains(_character);
    }

    public bool IsEnemy(BattleCharacterBase _character)
    {
        return this.characterManager.GetEnemies().Contains(_character);
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
            var t_Skill = t_character.GetSkills()[Random.Range(0, t_character.GetSkills().Length)];
            if (t_Skill.attackRange == SOSkillBase.AttackRangeType.All)
            {
                t_character.SetSelectedSkill(t_Skill, characterManager.GetAllies());
                t_character.SetAction(CharacterActionType.Skill);
            }
            else
            {
                t_character.SetSelectedSkill(t_Skill, GetRandomElementAsArray(this.characterManager.GetAllies()));
                t_character.SetAction(CharacterActionType.Skill);
            }
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
        foreach (var t_enemy in this.enemyDataContainer)
        {
            if (t_enemy.GetBattleCharacter().GetCharacterName() == "Merla")
            {
                SaveGameManager.instance.GetCurrentSaveData().isCleardBoss = true;
            }
        }

        GameManager.instance.ChangeSceneBattleToField(this.playerDataContainer[0].GetBattleCharacter(), this.playerDataContainer[1].GetBattleCharacter());
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
            NextSequence();
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
                this.selectButtonGroup.gameObject.SetActive(false);
                UIManager.instance.ShowUI<SkillSelectUI>(new SkillUIData()
                {
                    battleManager = this,
                    battleCharacterBase = this.currentSelectedCharacter,
                    identifier = "BattleSkillUI",
                    skills = this.currentSelectedCharacter.GetSkills(),
                    isAllowMultifle = false,
                    order = 0,
                    action = UISelectSkill,
                    onHide = () =>
                    {
                        this.selectButtonGroup.gameObject.SetActive(true);
                    }
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
                _character.SetSelectedSkill(_skill, characterManager.GetAllies());
                CheckAllReady();
                break;
        }
    }


    public void SetChooseSequenceState(ChooseSequence.ChooseState _state)
    {
        var t_Sequence = this.turnSystem.GetCurrentSequence() as ChooseSequence;
        if (t_Sequence != null)
        {
            if (_state == ChooseSequence.ChooseState.None)
            {
                SetAllyArrow(true);
                SetEnemyArrow(false);
            }
            else if (_state == ChooseSequence.ChooseState.SelectEnemy)
            {
                SetAllyArrow(false);
                SetEnemyArrow(true);
            }
            else
            {
                SetAllyArrow(false);
                SetEnemyArrow(false);
            }
            t_Sequence.state = _state;
        }
    }

    public void SetAllyArrow(bool _is)
    {
        foreach (var t_character in characterManager.GetAllies())
        {
            t_character.SetArrow(_is);
        }
    }
    public void SetEnemyArrow(bool _is)
    {
        foreach (var t_character in characterManager.GetEnemies())
        {
            t_character.SetArrow(_is);
        }
    }
    public void SetEnemyShadow(bool _is)
    {
        foreach (var t_character in characterManager.GetEnemies())
        {
            t_character.SetShadow(_is);
        }
    }
    public void SetAllyShadow(bool _is)
    {
        foreach (var t_character in characterManager.GetAllies())
        {
            t_character.SetShadow(_is);
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
            ShowText($"{t_chracter.GetCharacterName()}{GameStatics.GetSubjectParticle(t_chracter.GetCharacterName())} {t_Exp} 의 경험치를 얻었다!");

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