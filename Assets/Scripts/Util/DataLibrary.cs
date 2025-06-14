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

    Dictionary<int, SOBattleCharacter> characterData = new Dictionary<int, SOBattleCharacter>();
    Dictionary<int, SOItem> itemData = new Dictionary<int, SOItem>();

    Dictionary<int, BattleConversationData> battleConversationData = new Dictionary<int, BattleConversationData>();

    Dictionary<int, DialogData> dialogData = new Dictionary<int, DialogData>();

    Dictionary<string, Dictionary<int, Sprite>> dialogPortraitsdata = new Dictionary<string, Dictionary<int, Sprite>>();

    Dictionary<StatusEffectID, StatusEffectInfo> statusEffectData;

    Dictionary<int, SOSkillBase> skillData = new Dictionary<int, SOSkillBase>();

    Dictionary<string, MapEntity> mapData = new Dictionary<string, MapEntity>();

    Dictionary<string, GameObject> uiPrefabData = new Dictionary<string, GameObject>();

    Dictionary<string, CutsceneData> cutsceneData = new Dictionary<string, CutsceneData>();

    [SerializeField] GameObject loadingUIPrefab;
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
        // LoadingUI를 UIManager를 통해 표시
        var loadingUI = UIManager.instance.ShowUI<LoadingUI>(this.loadingUIPrefab, new LoadingUIData()
        {
            identifier = "LoadingUI",
            isAllowMultifle = false,
            task = UniTask.CompletedTask // 초기값
        });

        if (loadingUI != null)
        {
            float totalSteps = 9f; // 총 로딩 단계 수
            int currentStep = 0;

            loadingUI.ChangeText("악몽 꾸는중");
            loadingUI.UpdateProgress(currentStep / totalSteps);
            await LoadAllCharacterDataAsync();
            currentStep++;

            loadingUI.ChangeText("잠꼬대 하는중");
            loadingUI.UpdateProgress(currentStep / totalSteps);
            await LoadDialogDataAsync();
            currentStep++;

            loadingUI.ChangeText("가방 뒤지는중");
            loadingUI.UpdateProgress(currentStep / totalSteps);
            await LoadAllItemDataAsync();
            currentStep++;

            loadingUI.ChangeText("꿈에서 헤매는중");
            loadingUI.UpdateProgress(currentStep / totalSteps);
            await LoadAllMapDataAsync();
            currentStep++;

            loadingUI.ChangeText("꿈에서 본 소녀 찾는중");
            loadingUI.UpdateProgress(currentStep / totalSteps);
            await LoadAllPortraitsDataAsync();
            currentStep++;

            loadingUI.ChangeText("바다 보는중");
            loadingUI.UpdateProgress(currentStep / totalSteps);
            await LoadEffectDataAsync();
            currentStep++;

            loadingUI.ChangeText("꿈에서 하늘 나는중");
            loadingUI.UpdateProgress(currentStep / totalSteps);
            await LoadAllSkillDataAsync();
            currentStep++;

            loadingUI.ChangeText("꿈에서 깨는중");
            loadingUI.UpdateProgress(currentStep / totalSteps);
            await LoadAllUIDataAsync();
            currentStep++;

            loadingUI.ChangeText("꿈에서 본 장면 떠올리는중");
            loadingUI.UpdateProgress(currentStep / totalSteps);
            await LoadAllCutsceneDataAsync();
            currentStep++;

            loadingUI.ChangeText("완료!!");
            loadingUI.UpdateProgress(1.0f); // 100% 완료
            await UniTask.Delay(1000);

            // LoadingUI 숨기기
            UIManager.instance.HideUI("LoadingUI");
        }
    }

    #region Load

    // 주소값 해제를 위한 핸들 추적용 변수들
    private AsyncOperationHandle<TextAsset> conversationHandle;
    private AsyncOperationHandle<TextAsset> dialogHandle;
    private AsyncOperationHandle<TextAsset> effectHandle;
    private AsyncOperationHandle<IList<SOSkillBase>> skillHandle;
    private AsyncOperationHandle<IList<SOBattleCharacter>> characterHandle;
    private AsyncOperationHandle<IList<GameObject>> mapHandle;
    private AsyncOperationHandle<IList<Sprite>> portraitHandle;
    private AsyncOperationHandle<IList<GameObject>> uiHandle;

    private AsyncOperationHandle<IList<CutsceneData>> cutsceneHandle;

    // 기존 Load 메서드 내부에서 handle 저장 추가
    public async UniTask LoadConversationDataAsync()
    {
        conversationHandle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/ConversationData.csv");
        var asset = await conversationHandle.Task;
        this.battleConversationData = CSVReader.ReadConversationTable(asset);
    }

    public async UniTask LoadDialogDataAsync()
    {
        dialogHandle = Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/DialogDataTable.csv");
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
    public async UniTask LoadAllMapDataAsync()
    {
        mapHandle = Addressables.LoadAssetsAsync<GameObject>("MapPrefab", t_map =>
        {
            var t_mapComponent = t_map.GetComponent<MapEntity>();
            if (!this.mapData.ContainsKey(t_mapComponent.GetID()))
            {
                this.mapData.Add(t_mapComponent.GetID(), t_mapComponent);
            }
            else
            {
                LogUtil.Log($"중복된 맵 {t_mapComponent.GetID()}");
            }

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
            uiPrefabData[prefab.name] = prefab;
        });
        await uiHandle.Task;
    }

    public async UniTask LoadAllItemDataAsync()
    {
        var itemHandle = Addressables.LoadAssetsAsync<SOItem>("ItemSO", so =>
        {
            if (!itemData.ContainsKey(so.GetItemIndex()))
                itemData.Add(so.GetItemIndex(), so);
        });
        await itemHandle.Task;
    }

    public async UniTask LoadAllCutsceneDataAsync()
    {
        cutsceneHandle = Addressables.LoadAssetsAsync<CutsceneData>("CutsceneSO", so =>
        {
            cutsceneData.Add(so.id, so);
        });
        await cutsceneHandle.Task;
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
        if (cutsceneHandle.IsValid()) Addressables.Release(cutsceneHandle);

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

    public DialogData GetDialog(int _key) => this.dialogData[_key];
    public Dictionary<int, DialogData> GetDialogTable() => this.dialogData;

    public SOItem GetItemByIndex(int _index)
    {
        return this.itemData.TryGetValue(_index, out var t_item) ? t_item : null;
    }

    public Dictionary<string, CutsceneData> GetCutsceneTable() => this.cutsceneData;
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

    public int autoNextDialog;
}

public enum StatusCategory
{
    Debuff,
    Restriction,
    SpecialEffect,
    Panalty
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
    Spended = 101,
    Exhaustion = 102,
    CantAction = 103,
}
#endregion