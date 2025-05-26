using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveUI : UIBase
{
    [Header("Save Slot Prefab")]

    SaveUIData uiData;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform slotsContainer;

    SaveSlot[] saveSlots = new SaveSlot[9];
    int selectedSlot = -1;

    public override void Initialization(UIData _data)
    {
        if(_data is SaveUIData saveUIData)
        {
            uiData = saveUIData;
        }
        InitializeSaveSlots();
        UpdateAllSlots();
    }

    void InitializeSaveSlots()
    {
        // 기존 슬롯들 제거
        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }

        // 새로운 슬롯들 생성
        for (int i = 0; i < 9; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
            SaveSlot slot = slotObj.GetComponent<SaveSlot>();
            int slotNumber = i + 1;

            slot.Initialize(slotNumber,
                () => SelectSlot(slotNumber),
                () => SaveToSlot(slotNumber),
                () => LoadFromSlot(slotNumber),
                () => DeleteSlot(slotNumber),
                uiData.isLoadOnly
                );

            saveSlots[i] = slot;
        }
    }

    void UpdateAllSlots()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            UpdateSlotInfo(i + 1);
        }
    }

    void UpdateSlotInfo(int slotNumber)
    {
        SaveSlot slot = saveSlots[slotNumber - 1];
        bool exists = SaveGameManager.instance.DoesSaveExist(slotNumber);

        if (exists)
        {
            SaveData saveData = SaveGameManager.instance.GetSaveInfo(slotNumber);
            slot.UpdateInfo($"슬롯 {slotNumber}", "저장된 데이터 있음", true);
        }
        else
        {
            slot.UpdateInfo($"슬롯 {slotNumber}", "비어있음", false);
        }
    }
    void SelectSlot(int slotNumber)
    {
        selectedSlot = slotNumber;
        foreach (var slot in saveSlots)
        {
            slot.SetSelected(slot.SlotNumber == slotNumber);
        }
    }

    void SaveToSlot(int slotNumber)
    {
        SaveGameManager.instance.SavetoFile(slotNumber);
        UpdateSlotInfo(slotNumber);
    }

    void LoadFromSlot(int slotNumber)
    {
        if (SaveGameManager.instance.DoesSaveExist(slotNumber))
        {
            SaveGameManager.instance.LoadFromSlot(slotNumber);
            Hide();
        }
    }

    void DeleteSlot(int slotNumber)
    {
        SaveGameManager.instance.DeleteSave(slotNumber);
        UpdateSlotInfo(slotNumber);
    }

    public override void Show(UIData _data)
    {
        this.contents.SetActive(true);

        UpdateAllSlots();
    }

    public override void Hide()
    {
        this.contents.SetActive(false);
    }
}