using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class DataLibrary : MonoBehaviour
{
    public static DataLibrary instance;

    CSVReader csvReader;

    Dictionary<int, SOMonsterBase> monsterDictionary = new Dictionary<int, SOMonsterBase>();

    Dictionary<int, BattleConversationData> battleConversationData = new Dictionary<int, BattleConversationData>();

    Dictionary<int, DialogData> dialogData = new Dictionary<int, DialogData>();

    Dictionary<string, Dictionary<int, Sprite>> dialogPortraits = new Dictionary<string, Dictionary<int, Sprite>>();


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
        LoadAllMonsterBase("MonsterSO");
        LoadConversation();
        LoadDialog();
        LoadAllPortraits();


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

    public void LoadAllMonsterBase(string label)
    {
        Addressables.LoadAssetsAsync<SOMonsterBase>(label, so =>
        {
            if (!monsterDictionary.ContainsKey(so.identifier))
            {
                monsterDictionary.Add(so.identifier, so);
                Debug.Log($"로드된: {so.name}");
            }
        }).Completed += OnLoadComplete;
    }
    public void LoadConversation()
    {
        Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/ConversationData.csv").Completed += handle =>
        {
            var asset = handle.Result;
            this.battleConversationData = CSVReader.ReadConversationTable(asset);
        };
    }

    public void LoadDialog()
    {
        Addressables.LoadAssetAsync<TextAsset>("Assets/TextAssets/DialogDataTable.csv").Completed += handle =>
        {
            var asset = handle.Result;
            this.dialogData = CSVReader.ReadDialogData(asset);
        };
    }

    public void LoadAllPortraits()
    {
        Addressables.LoadAssetsAsync<Sprite>("Portraits", t_sprite =>
        {
            var t_name = Regex.Split(t_sprite.name, "_");
            if (!dialogPortraits.ContainsKey(t_name[0]))
            {
                dialogPortraits.Add(t_name[0], new Dictionary<int, Sprite>());
                this.dialogPortraits[t_name[0]].Add(int.Parse(t_name[1]), t_sprite);
                Debug.Log($"로드된: {t_sprite.name}");
            }
            else
            {
                dialogPortraits[t_name[0]].Add(int.Parse(t_name[1]), t_sprite);
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

    public SOMonsterBase GetSOMonster(int key)
    {
        return monsterDictionary.TryGetValue(key, out var so) ? so : null;
    }

    public Sprite GetPortrait(string _key, int _emotion)
    {
        Debug.Log($"{_key}");
        if (this.dialogPortraits.TryGetValue(_key, out var t_dic))
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
}

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