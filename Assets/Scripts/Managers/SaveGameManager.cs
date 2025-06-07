using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using System;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager instance;
    const string fileNameFormat = "SaveData_{0}"; // 슬롯 번호를 포함한 파일 이름 형식
    const int MAX_SLOTS = 9; // 최대 슬롯 수

    SaveData saveInFile;
    public SaveData currentSaveData;
    public SaveData GetCurrentSaveData() => this.currentSaveData;
    public void SetCurrentSaveData(SaveData _data) => this.currentSaveData = _data;

    public bool isSaveDebug;
    public int currentSlot = 1; // 현재 선택된 슬롯

    SaveData previousSaveData { get; set; }

    private float gameStartTime;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        gameStartTime = Time.time;
    }

    private void Update()
    {
        if (currentSaveData != null)
        {
            currentSaveData.playTime = Time.time - gameStartTime;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SavetoFile(currentSlot);
        }
#endif
    }

    public void SavetoFile(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > MAX_SLOTS)
        {
            Debug.LogError($"잘못된 슬롯 번호입니다. (1-{MAX_SLOTS} 사이의 값이어야 합니다)");
            return;
        }

        // 저장하기 전에 임시 리스트 초기화
        this.currentSaveData.itemNames.Clear();

        // 아이템 정보 저장
        foreach (var item in this.currentSaveData.items.Values)
        {
            this.currentSaveData.itemNames.Add(new SaveItemMinimal(item.GetItemIndex(), item.amount));
        }

        // 저장 시간 업데이트
        this.currentSaveData.saveDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        try
        {
            // 파일에 저장
            string fileName = string.Format(fileNameFormat, slotNumber);
            SaveToJsonFile<SaveData>(this.currentSaveData, fileName);
            this.saveInFile = this.currentSaveData;
            Debug.Log($"게임 데이터가 슬롯 {slotNumber}에 성공적으로 저장되었습니다.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"저장 중 오류 발생: {e.Message}");
        }
    }

    public void LoadFromSlot(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > MAX_SLOTS)
        {
            Debug.LogError($"잘못된 슬롯 번호입니다. (1-{MAX_SLOTS} 사이의 값이어야 합니다)");
            return;
        }

        currentSlot = slotNumber;
        LoadToFile(slotNumber);
    }

    void LoadToFile(int slotNumber)
    {
        try
        {
            string fileName = string.Format(fileNameFormat, slotNumber);
            this.saveInFile = LoadFromJson<SaveData>(fileName);

            if (this.saveInFile == null)
            {
                Debug.Log($"슬롯 {slotNumber}에 새로운 세이브 데이터를 생성합니다.");
                this.saveInFile = new SaveData();
                this.currentSaveData = this.saveInFile;
                return;
            }

            // 아이템 데이터 초기화 및 로드
            this.saveInFile.items.Clear();
            foreach (var itemMinimal in this.saveInFile.itemNames)
            {
                SOItem item = DataLibrary.instance.GetItemByIndex(itemMinimal.index);
                if (item != null)
                {
                    SaveItem saveItem = new SaveItem(item, itemMinimal.amount);
                    this.saveInFile.items.Add(itemMinimal.index, saveItem);
                }
            }

            // 현재 세이브 데이터 설정
            this.currentSaveData = this.saveInFile;
            Debug.Log($"슬롯 {slotNumber}의 게임 데이터를 성공적으로 불러왔습니다.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"로드 중 오류 발생: {e.Message}");
            // 오류 발생 시 새로운 세이브 데이터 생성
            this.saveInFile = new SaveData();
            this.currentSaveData = this.saveInFile;
        }
    }

    public bool DoesSaveExistAll()
    {
        for (int i = 1; i <= MAX_SLOTS; i++)
        {
            if (DoesSaveExist(i))
            {
                return true;
            }
        }
        return false;
    }

    public bool DoesSaveExist(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > MAX_SLOTS) return false;
        string fileName = string.Format(fileNameFormat, slotNumber);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        return File.Exists(path);
    }

    public SaveData GetSaveInfo(int slotNumber)
    {
        if (!DoesSaveExist(slotNumber)) return null;
        string fileName = string.Format(fileNameFormat, slotNumber);
        return LoadFromJson<SaveData>(fileName);
    }

    public void DeleteSave(int slotNumber)
    {
        if (slotNumber < 1 || slotNumber > MAX_SLOTS) return;
        string fileName = string.Format(fileNameFormat, slotNumber);
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"슬롯 {slotNumber}의 세이브 데이터가 삭제되었습니다.");
        }
    }

    public async UniTask Initialization()
    {
        if (isSaveDebug == true)
        {
            this.saveInFile = new SaveData();
            this.saveInFile.chatacterDialogs = new Dictionary<int, bool>();
            foreach (var t_dialog in DataLibrary.instance.GetDialogTable())
            {
                this.saveInFile.chatacterDialogs.Add(t_dialog.Key, false);
            }
            this.saveInFile.mapItems = new Dictionary<string, List<bool>>();

            this.currentSaveData = this.saveInFile;
        }
        else
        {
            LoadFromSlot(currentSlot);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        return;
    }

    public SaveData NewSaveData()
    {
        var t_saveData = new SaveData();
        t_saveData.items = new Dictionary<int, SaveItem>();
        t_saveData.itemNames = new List<SaveItemMinimal>();
        t_saveData.chatacterDialogs = new Dictionary<int, bool>();
        foreach (var t_dialog in DataLibrary.instance.GetDialogTable())
        {
            t_saveData.chatacterDialogs.Add(t_dialog.Key, false);
        }

        t_saveData.mapItems = new Dictionary<string, List<bool>>();
        foreach (var map in DataLibrary.instance.GetMapAll())
        {
            t_saveData.mapItems.Add(map.GetID(), new List<bool>());
            foreach (var item in map.GetItems())
            {
                t_saveData.mapItems[map.GetID()].Add(false);
            }
        }
        t_saveData.currentMap = "입구";
        return t_saveData;
    }

    public void SaveToJsonFile<T>(T data, string fileName)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        string path = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllText(path, json);

        Debug.Log($"Data saved to {path}");
    }

    public T LoadFromJson<T>(string _fileName) where T : class
    {
        string path = Path.Combine(Application.persistentDataPath, _fileName);
        Debug.Log(path);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            T t_result = JsonConvert.DeserializeObject<T>(json);

            Debug.Log("Data loaded successfully!");
            return t_result;
        }
        else
        {
            Debug.LogWarning("File not found!");
            return null;
        }
    }

    public void DeleteItem(int _item)
    {
        var t_items = SaveGameManager.instance.currentSaveData.items;

        if (t_items.TryGetValue(_item, out var t_item))
        {
            this.currentSaveData.items.Remove(_item);
        }
        else
        {
            LogUtil.Log("No Such Item");
        }
    }

    public bool CheckStoryIs(int _index)
    {
        return this.currentSaveData.chatacterDialogs[_index];
    }

    public void ResetSave()
    {
        this.currentSaveData = this.saveInFile;
    }
}

[System.Serializable]
public class SaveData
{
    public string currentMap;
    public Dictionary<int, SaveItem> items;
    public Dictionary<int, bool> chatacterDialogs;
    public List<SaveItemMinimal> itemNames;
    public Dictionary<string, List<bool>> mapItems;

    // 시간 정보 추가
    public float playTime;        // 플레이 시간 (초 단위)
    public string saveDateTime;   // 저장 시간

    public SaveData()
    {
        this.items = new Dictionary<int, SaveItem>();
        this.itemNames = new List<SaveItemMinimal>();
        this.chatacterDialogs = new Dictionary<int, bool>();
        this.playTime = 0f;
        this.saveDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[System.Serializable]
public class SaveItem
{
    SOItem item;
    public int amount;

    public int GetItemIndex() => item.GetItemIndex();
    public Sprite GetItemImage() => item.GetItemImage();
    public string GetItemName() => item.GetItemName();
    public string GetDescription() => item.GetItemDescription();
    public int GetAmount() => amount;

    public SOItem GetSOItem() => item;

    public void SetItem(SOItem item) => this.item = item;
    public void SetAmount(int amount) => this.amount = amount;

    public SaveItem(SOItem _item, int _amount)
    {
        this.item = _item;
        this.amount = _amount;
    }


}
[System.Serializable]
public class SaveItemMinimal
{
    public int index;
    public int amount;

    public SaveItemMinimal(int _index, int _amount)
    {
        this.index = _index;
        this.amount = _amount;
    }
}
