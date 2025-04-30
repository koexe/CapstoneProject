using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;

public class DataLibrary : MonoBehaviour
{
    public static DataLibrary instance;

    CSVReader csvReader;

    Dictionary<int, SOMonsterBase> monsterData = new Dictionary<int, SOMonsterBase>();

    Dictionary<int, BattleConversationData> battleConversationData = new Dictionary<int, BattleConversationData>();

    Dictionary<int, DialogData> dialogData = new Dictionary<int, DialogData>();

    Dictionary<string, Dictionary<int, Sprite>> dialogPortraitsdata = new Dictionary<string, Dictionary<int, Sprite>>();

    Dictionary<StatusEffectID, StatusEffectInfo> statusEffectData;

    Dictionary<int, SOSkillBase> skillData = new Dictionary<int, SOSkillBase>();


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        this.csvReader = new CSVReader();
        LoadAllMonsterData();
        LoadConversationData();
        LoadDialogData();
        LoadAllPortraitsData();
        LoadEffectData();
        LoadAllSkillData();

    }

    public async UniTask LoadAllDataAsync()
    {
        await LoadAllMonsterDataAsync();
        await LoadDialogDataAsync();
        await LoadAllPortraitsDataAsync();
        await LoadEffectDataAsync();
        await LoadAllSkillDataAsync();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.ShowUI<DialogUI>(new DialogUIData()
            {
                identifier = "DialogUI",
                data = this.dialogData[0]
            });
        }
    }

    #region Load
    public async UniTask LoadConversationDataAsync()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/ConversationData.csv");
        var asset = await handle.Task;
        this.battleConversationData = CSVReader.ReadConversationTable(asset);
    }

    public async UniTask LoadDialogDataAsync()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/DialogDataTable - 복사본.csv");
        var asset = await handle.Task;
        this.dialogData = CSVReader.ReadDialogData(asset);
    }

    public async UniTask LoadEffectDataAsync()
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/StatusTable.csv");
        var asset = await handle.Task;
        this.statusEffectData = CSVReader.ReadEffectData(asset);
    }

    public async UniTask LoadAllSkillDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<SOSkillBase>("SkillSO", so =>
        {
            if (!skillData.ContainsKey(so.skillIdentifier))
            {
                skillData.Add(so.skillIdentifier, so);
                Debug.Log($"로드된: {so.name}");
            }
        });
        await handle.Task;
        Debug.Log($"Skill 로드 완료: {handle.Result.Count}개");
    }

    public async UniTask LoadAllMonsterDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<SOMonsterBase>("MonsterSO", so =>
        {
            if (!monsterData.ContainsKey(so.identifier))
            {
                monsterData.Add(so.identifier, so);
                Debug.Log($"로드된: {so.name}");
            }
        });
        await handle.Task;
        Debug.Log($"Monster 로드 완료: {handle.Result.Count}개");
    }


    public async UniTask LoadAllPortraitsDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<Sprite>("Portraits", sprite =>
        {
            var parts = Regex.Split(sprite.name, "_");
            if (!dialogPortraitsdata.ContainsKey(parts[0]))
            {
                dialogPortraitsdata[parts[0]] = new Dictionary<int, Sprite>();
            }
            dialogPortraitsdata[parts[0]][int.Parse(parts[1])] = sprite;
            Debug.Log($"로드된: {sprite.name}");
        });
        await handle.Task;
        Debug.Log($"Portrait 로드 완료: {handle.Result.Count}개");
    }




    public void LoadAllMonsterData()
    {
        Addressables.LoadAssetsAsync<SOMonsterBase>("MonsterSO", so =>
        {
            if (!monsterData.ContainsKey(so.identifier))
            {
                monsterData.Add(so.identifier, so);
                Debug.Log($"로드된: {so.name}");
            }
        }).Completed += OnLoadComplete;
    }
    public void LoadConversationData()
    {
        Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/ConversationData.csv").Completed += handle =>
        {
            var asset = handle.Result;
            this.battleConversationData = CSVReader.ReadConversationTable(asset);
        };
    }

    public void LoadDialogData()
    {
        Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/DialogDataTable - 복사본.csv").Completed += handle =>
        {
            var asset = handle.Result;
            this.dialogData = CSVReader.ReadDialogData(asset);
        };
    }

    public void LoadEffectData()
    {
        Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/StatusTable.csv").Completed += handle =>
        {
            var asset = handle.Result;
            this.statusEffectData = CSVReader.ReadEffectData(asset);
        };
    }
    public void LoadAllPortraitsData()
    {
        Addressables.LoadAssetsAsync<Sprite>("Portraits", t_sprite =>
        {
            var t_name = Regex.Split(t_sprite.name, "_");
            if (!dialogPortraitsdata.ContainsKey(t_name[0]))
            {
                dialogPortraitsdata.Add(t_name[0], new Dictionary<int, Sprite>());
                this.dialogPortraitsdata[t_name[0]].Add(int.Parse(t_name[1]), t_sprite);
                Debug.Log($"로드된: {t_sprite.name}");
            }
            else
            {
                dialogPortraitsdata[t_name[0]].Add(int.Parse(t_name[1]), t_sprite);
                Debug.Log($"로드된: {t_sprite.name}");
            }
        }).Completed += OnLoadComplete;
    }

    private void OnLoadComplete<T>(AsyncOperationHandle<IList<T>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"총 로드 완료: {handle.Result.Count}개");

        }
        else
        {
            Debug.LogError("로딩 실패!");
        }
    }

    public void LoadAllSkillData()
    {
        Addressables.LoadAssetsAsync<SOSkillBase>("SkillSO", so =>
        {
            if (!monsterData.ContainsKey(so.skillIdentifier))
            {
                skillData.Add(so.skillIdentifier, so);
                Debug.Log($"로드된: {so.name }           {so.skillIdentifier}");
            }
        }).Completed += OnLoadComplete;
    }

    #endregion
    #region Get
    public SOMonsterBase GetSOMonster(int _key)
    {
        return this.monsterData.TryGetValue(_key, out var t_so) ? t_so : null;
    }
    public SOSkillBase GetSOSkill(int _key)
    {
        return this.skillData.TryGetValue(_key, out var t_so) ? t_so : null;
    }

    public Sprite GetPortrait(string _key, int _emotion)
    {
        Debug.Log($"{_key}");
        if (this.dialogPortraitsdata.TryGetValue(_key, out var t_dic))
        {
            if (t_dic.TryGetValue(_emotion, out var t_portrait))
            {
                return t_portrait;
            }
            else
            {
                Debug.Log($"No Such Portrait Emotion !!!! {_key}  {_emotion}");
                return null;
            }

        }
        else
        {
            Debug.Log($"No Such Portrait Name!!!! {_key}");
            return null;
        }
    }

    public StatusEffectInfo GetStateInfo(StatusEffectID _id)
    {
        if (this.statusEffectData.TryGetValue(_id, out var t_value))
        {
            return t_value;
        }
        else
        {
            Debug.Log($"no Such State!  {_id}");
            return null;
        }
    }
    #endregion
}

#region Class Type
public class BattleConversationData
{
    public int index;
    public string dialog;
    public string[] choices;
    public string[] result;
}

public class DialogData
{
    public int index;

    /// <summary>
    /// Character / Character Emotion Index / Dialog text
    /// </summary>
    public (string, string)[] dialogs;

    /// <summary>
    /// Condition Dialog / Is / Faild Text / Succesed 
    /// </summary>
    public (int, bool, string, int) condition;

    public (string, int)[][] characters;

    /// <summary>
    /// Choice Text / Choice Link Dialog
    /// </summary>
    public (string, int)[] choices;
}

public enum StatusCategory
{
    Debuff,
    Restriction,
    SpecialEffect
}

public class StatusEffectInfo
{
    public StatusEffectID id;
    public StatusCategory category;
    public string description;
    public int duration;
    public bool isStackable;
    public float activationChance;
    public int maxStack; // -1이면 스택 불가능
}
public enum StatusEffectID
{
    None = 0,
    Defence = 1,
    Bleed = 11,
    Corrosion = 12,
    Poison = 13,
    Blind = 14,
    DEFDown = 15,
    ATKDown = 16,
    Frail = 17,
    HealBlock = 18,
    SensoryLoss = 19,
    Decay = 20,
    HealDisable = 21,
    FocusLoss = 22,
    Bind = 23,
    Confusion = 24,
    MentalBreak = 25,
    Lethargy = 26,
    Stun = 27,
    WeakResistance = 28,
    Split = 29,
    SelfHarm = 30,
    MarkOfDoom = 31,
    PN001 = 101,
    PN002 = 102,
    PN003 = 103,
    PN004 = 104,
    PN005 = 105,
}
#endregion