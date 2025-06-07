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
        // 현재 세이브 데이터로 설정
        SaveGameManager.instance.SetCurrentSaveData(SaveGameManager.instance.NewSaveData());
        
        // 필드로 씬 전환
        await GameManager.instance.ChangeSceneMainToField();
        
    }
}
