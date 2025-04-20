using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataLibrary : MonoBehaviour
{
    public static DataLibrary instance;

    CSVReader csvReader;

    Dictionary<int, SOMonsterBase> monsterDictionary = new Dictionary<int, SOMonsterBase>();

    Dictionary<int, BattleConversationData> battleConversationData = new Dictionary<int, BattleConversationData>();


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
        LoadAllTextData("TextAsset");
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
    public void LoadAllTextData(string label)
    {
        Addressables.LoadAssetsAsync<TextAsset>(label, t_data =>
        {
            this.battleConversationData = CSVReader.ReadConversationTable(t_data);

        }).Completed += OnLoadComplete;
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