using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField] Button loadButton;
    [SerializeField] GameObject startButton;

    void Start()
    {
        if (SaveGameManager.instance.DoesSaveExistAll())
        {
            loadButton.interactable = true;
        }
        else
        {
            loadButton.interactable = false;
        }
    }

    public void OnClickLoadButton()
    {
        UIManager.instance.ShowUI<SaveUI>(new SaveUIData()
        {
            identifier = "SaveUI",
            isAllowMultifle = false,
            isLoadOnly = true
        });
    }

    public async void OnClickStartButton()
    {
        // 새로운 세이브 데이터 생성
        SaveData t_newSaveData = new SaveData();
        
        // 기본 대화 데이터 초기화
        t_newSaveData.chatacterDialogs = new Dictionary<int, bool>();
        foreach (var t_dialog in DataLibrary.instance.GetDialogTable())
        {
            t_newSaveData.chatacterDialogs.Add(t_dialog.Key, false);
        }
        
        // 맵 아이템 데이터 초기화
        t_newSaveData.mapItems = new Dictionary<string, List<bool>>();

        t_newSaveData.currentMap = "입구";
        
        // 현재 세이브 데이터로 설정
        SaveGameManager.instance.SetCurrentSaveData(t_newSaveData);
        
        // 필드로 씬 전환
        await GameManager.instance.ChangeSceneMainToField();
        
    }
}
