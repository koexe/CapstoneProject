using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataLibrary : MonoBehaviour
{
    public static DataLibrary instance;

    CSVReader csvReader;

    Dictionary<int, SOMonsterBase> monsterDictionary = new Dictionary<int, SOMonsterBase>();

    Dictionary<int, BattleConversationData> battleConversationData = new Dictionary<int, BattleConversationData>();

    Dictionary<int, DialogData> dialogData = new Dictionary<int, DialogData>();


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

    private void OnLoadComplete<T> (AsyncOperationHandle<IList<T>> handle)
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
    public (string, int, string)[] dialogs;

    /// <summary>
    /// Condition Dialog / Is / Faild Text / Succesed 
    /// </summary>
    public (int, bool, string , int) condition;
    
    /// <summary>
    /// Choice Text / Choice Link Dialog
    /// </summary>
    public (string, int)[] choices;
}