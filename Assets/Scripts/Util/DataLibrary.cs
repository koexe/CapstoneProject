using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class DataLibrary : MonoBehaviour
{
    public static DataLibrary instance;

    [SerializeField] Image coverImage;
    [SerializeField] TextMeshProUGUI loadingInfoText;

    Dictionary<int, SOBattleCharacter> characterData = new Dictionary<int, SOBattleCharacter>();

    Dictionary<int, BattleConversationData> battleConversationData = new Dictionary<int, BattleConversationData>();

    Dictionary<int, DialogData> dialogData = new Dictionary<int, DialogData>();

    Dictionary<string, Dictionary<int, Sprite>> dialogPortraitsdata = new Dictionary<string, Dictionary<int, Sprite>>();

    Dictionary<StatusEffectID, StatusEffectInfo> statusEffectData;

    Dictionary<int, SOSkillBase> skillData = new Dictionary<int, SOSkillBase>();

    Dictionary<string, MapEntity> mapData;

    Dictionary<string, GameObject> uiPrefabData = new Dictionary<string, GameObject>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public async UniTask Initialization()
    {
        await LoadAllDataAsync();
        UnloadAllData();
    }

    public async UniTask LoadAllDataAsync()
    {
        this.coverImage.gameObject.SetActive(true);
        this.loadingInfoText.gameObject.SetActive(true);
        this.loadingInfoText.text = "Load Monster Data";
        await LoadAllCharacterDataAsync();
        this.loadingInfoText.text = "Load Dialog Data";
        await LoadDialogDataAsync();
        this.loadingInfoText.text = "Load Portraits Data";
        await LoadAllPortraitsDataAsync();
        this.loadingInfoText.text = "Load Effect Data";
        await LoadEffectDataAsync();
        this.loadingInfoText.text = "Load Skill Data";
        await LoadAllSkillDataAsync();
        this.loadingInfoText.text = "Load UI Data";
        await LoadAllUIDataAsync();
        this.loadingInfoText.text = "All Loading Done!!";
        await UniTask.Delay(1000);
        this.coverImage.gameObject.SetActive(false);
        this.loadingInfoText.gameObject.SetActive(false);
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
    //public async UniTask LoadConversationDataAsync()
    //{
    //    var handle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/ConversationData.csv");
    //    var asset = await handle.Task;
    //    this.battleConversationData = CSVReader.ReadConversationTable(asset);
    //}

    //public async UniTask LoadDialogDataAsync()
    //{
    //    var handle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/DialogDataTable - 복사본.csv");
    //    var asset = await handle.Task;
    //    this.dialogData = CSVReader.ReadDialogData(asset);
    //}

    //public async UniTask LoadEffectDataAsync()
    //{
    //    var handle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/StatusTable.csv");
    //    var asset = await handle.Task;
    //    this.statusEffectData = CSVReader.ReadEffectData(asset);
    //}

    //public async UniTask LoadAllSkillDataAsync()
    //{
    //    var handle = Addressables.LoadAssetsAsync<SOSkillBase>("SkillSO", so =>
    //    {
    //        if (!skillData.ContainsKey(so.skillIdentifier))
    //        {
    //            skillData.Add(so.skillIdentifier, so);
    //            Debug.Log($"로드된: {so.name}");
    //        }
    //    });
    //    await handle.Task;
    //    Debug.Log($"Skill 로드 완료: {handle.Result.Count}개");
    //}

    //public async UniTask LoadAllCharacterDataAsync()
    //{
    //    var handle = Addressables.LoadAssetsAsync<SOBattleCharacter>("MonsterSO", so =>
    //    {
    //        if (!characterData.ContainsKey(so.identifier))
    //        {
    //            characterData.Add(so.identifier, so);
    //            Debug.Log($"로드된: {so.name}");
    //        }
    //    });
    //    await handle.Task;
    //    Debug.Log($"Monster 로드 완료: {handle.Result.Count}개");
    //}


    //public async UniTask LoadAllPortraitsDataAsync()
    //{
    //    var handle = Addressables.LoadAssetsAsync<Sprite>("Portraits", sprite =>
    //    {
    //        var parts = Regex.Split(sprite.name, "_");
    //        if (!dialogPortraitsdata.ContainsKey(parts[0]))
    //        {
    //            dialogPortraitsdata[parts[0]] = new Dictionary<int, Sprite>();
    //        }
    //        dialogPortraitsdata[parts[0]][int.Parse(parts[1])] = sprite;
    //        Debug.Log($"로드된: {sprite.name}");
    //    });
    //    await handle.Task;
    //    Debug.Log($"Portrait 로드 완료: {handle.Result.Count}개");
    //}

    //public async UniTask LoadAllUIDataAsync()
    //{
    //    var handle = Addressables.LoadAssetsAsync<GameObject>("UIPrefab", t_Prefab =>
    //    {
    //        this.uiPrefabData.Add(t_Prefab.name, t_Prefab);

    //    });
    //    await handle.Task;
    //    Debug.Log($"Portrait 로드 완료: {handle.Result.Count}개");
    //}

    // 주소값 해제를 위한 핸들 추적용 변수들
    private AsyncOperationHandle<TextAsset> conversationHandle;
    private AsyncOperationHandle<TextAsset> dialogHandle;
    private AsyncOperationHandle<TextAsset> effectHandle;
    private AsyncOperationHandle<IList<SOSkillBase>> skillHandle;
    private AsyncOperationHandle<IList<SOBattleCharacter>> characterHandle;
    private AsyncOperationHandle<IList<Sprite>> portraitHandle;
    private AsyncOperationHandle<IList<GameObject>> uiHandle;

    // 기존 Load 메서드 내부에서 handle 저장 추가
    public async UniTask LoadConversationDataAsync()
    {
        conversationHandle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/ConversationData.csv");
        var asset = await conversationHandle.Task;
        this.battleConversationData = CSVReader.ReadConversationTable(asset);
    }

    public async UniTask LoadDialogDataAsync()
    {
        dialogHandle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/DialogDataTable - 복사본.csv");
        var asset = await dialogHandle.Task;
        this.dialogData = CSVReader.ReadDialogData(asset);
    }

    public async UniTask LoadEffectDataAsync()
    {
        effectHandle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/StatusTable.csv");
        var asset = await effectHandle.Task;
        this.statusEffectData = CSVReader.ReadEffectData(asset);
    }

    public async UniTask LoadAllSkillDataAsync()
    {
        skillHandle = Addressables.LoadAssetsAsync<SOSkillBase>("SkillSO", so =>
        {
            if (!skillData.ContainsKey(so.skillIdentifier))
                skillData.Add(so.skillIdentifier, so);
        });
        await skillHandle.Task;
    }

    public async UniTask LoadAllCharacterDataAsync()
    {
        characterHandle = Addressables.LoadAssetsAsync<SOBattleCharacter>("CharacterSO", so =>
        {
            if (!characterData.ContainsKey(so.GetIdentifier()))
                characterData.Add(so.GetIdentifier(), so);
        });
        await characterHandle.Task;
    }

    public async UniTask LoadAllPortraitsDataAsync()
    {
        portraitHandle = Addressables.LoadAssetsAsync<Sprite>("Portraits", sprite =>
        {
            var parts = Regex.Split(sprite.name, "_");
            if (!dialogPortraitsdata.ContainsKey(parts[0]))
                dialogPortraitsdata[parts[0]] = new Dictionary<int, Sprite>();
            dialogPortraitsdata[parts[0]][int.Parse(parts[1])] = sprite;
        });
        await portraitHandle.Task;
    }

    public async UniTask LoadAllUIDataAsync()
    {
        uiHandle = Addressables.LoadAssetsAsync<GameObject>("UIPrefab", prefab =>
        {
            Debug.Log(prefab.name);
            uiPrefabData[prefab.name] = prefab;
        });
        await uiHandle.Task;
    }

    public void UnloadAllData()
    {
        if (conversationHandle.IsValid()) Addressables.Release(conversationHandle);
        if (dialogHandle.IsValid()) Addressables.Release(dialogHandle);
        if (effectHandle.IsValid()) Addressables.Release(effectHandle);
        if (skillHandle.IsValid()) Addressables.Release(skillHandle);
        if (characterHandle.IsValid()) Addressables.Release(characterHandle);
        if (portraitHandle.IsValid()) Addressables.Release(portraitHandle);
        if (uiHandle.IsValid()) Addressables.Release(uiHandle);

        Debug.Log(" Addressables 리소스 전부 언로드 완료!");
    }

    #endregion
    #region Get
    public SOBattleCharacter GetSOCharacter(int _key)
    {
        return this.characterData.TryGetValue(_key, out var t_so) ? t_so : null;
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
    public GameObject GetUI(string _key)
    {
        if (this.uiPrefabData.TryGetValue(_key, out var t_ui))
        {
            return t_ui;
        }
        else
        {
            LogUtil.Log($"no Such UI {_key}");
            return null;
        }
    }
    public MapEntity GetMap(string _key) => this.mapData[_key];
    public List<MapEntity> GetMapAll() => this.mapData.Values.ToList();
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