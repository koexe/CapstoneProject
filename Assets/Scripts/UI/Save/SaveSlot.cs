using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI slotNumberText;
    [SerializeField] TextMeshProUGUI saveInfoText;
    [SerializeField] Button slotButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button loadButton;
    [SerializeField] Button deleteButton;
    [SerializeField] Image selectedBorder;

    public int SlotNumber { get; private set; }
    
    Action onSelect;
    Action onSave;
    Action onLoad;
    Action onDelete;

    public void Initialize(int slotNumber, Action onSelect, Action onSave, Action onLoad, Action onDelete , bool _isLoadOnly = false)
    {
        this.SlotNumber = slotNumber;
        this.onSelect = onSelect;
        this.onSave = onSave;
        this.onLoad = onLoad;
        this.onDelete = onDelete;

        slotButton.onClick.AddListener(() => this.onSelect?.Invoke());
        saveButton.onClick.AddListener(ShowSavePopup);
        loadButton.onClick.AddListener(ShowLoadPopup);
        deleteButton.onClick.AddListener(ShowDeletePopup);

        if(_isLoadOnly)
        {
            saveButton.gameObject.SetActive(false);
        }
        else
        {
            saveButton.gameObject.SetActive(true);
        }

        SetSelected(false);
    }

    void ShowSavePopup()
    {
        UIManager.instance.ShowUI<PopupUI>(new PopupUIData()
        {
            identifier = $"SavePopup_{SlotNumber}",
            title = "저장",
            message = $"슬롯 {SlotNumber}에 저장하시겠습니까?",
            yesButtonText = "저장",
            noButtonText = "취소",
            onYesClicked = () => this.onSave?.Invoke()
        });
    }

    void ShowLoadPopup()
    {
        UIManager.instance.ShowUI<PopupUI>(new PopupUIData()
        {
            identifier = $"LoadPopup_{SlotNumber}",
            title = "불러오기",
            message = $"슬롯 {SlotNumber}의 데이터를 불러오시겠습니까?",
            yesButtonText = "불러오기",
            noButtonText = "취소",
            onYesClicked = () => this.onLoad?.Invoke()
        });
    }

    void ShowDeletePopup()
    {
        UIManager.instance.ShowUI<PopupUI>(new PopupUIData()
        {
            identifier = $"DeletePopup_{SlotNumber}",
            title = "삭제",
            message = $"슬롯 {SlotNumber}의 데이터를 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다.",
            yesButtonText = "삭제",
            noButtonText = "취소",
            onYesClicked = () => this.onDelete?.Invoke()
        });
    }

    public void UpdateInfo(string slotText, string infoText, bool hasData)
    {
        slotNumberText.text = slotText;
        
        if (hasData)
        {
            SaveData saveData = SaveGameManager.instance.GetSaveInfo(SlotNumber);
            float hours = Mathf.Floor(saveData.playTime / 3600);
            float minutes = Mathf.Floor((saveData.playTime % 3600) / 60);
            float seconds = Mathf.Floor(saveData.playTime % 60);
            
            string playTimeStr = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            saveInfoText.text = $"플레이 시간: {playTimeStr}\n저장 시간: {saveData.saveDateTime}";
        }
        else
        {
            saveInfoText.text = "비어있음";
        }
        
        loadButton.interactable = hasData;
        deleteButton.interactable = hasData;
    }

    public void SetSelected(bool selected)
    {
        selectedBorder.gameObject.SetActive(selected);
        saveButton.interactable = selected;
    }
} 