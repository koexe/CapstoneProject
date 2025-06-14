using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] GameObject bossObject;
    [SerializeField] GameObject memoryStone;

    [SerializeField] int bossClearDialogIndex;

    [SerializeField] SOBattleCharacter boss;

    [SerializeField] CutsceneData cutsceneData;


    private void OnEnable()
    {
        if (SaveGameManager.instance.GetCurrentSaveData().isCleardBoss)
        {
            this.bossObject.SetActive(false);
            this.memoryStone.SetActive(true);
        }
        else
        {
            this.memoryStone.SetActive(false);
            this.bossObject.SetActive(true);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (SaveGameManager.instance.GetCurrentSaveData().isCleardBoss)
        {
            if (other.CompareTag("Player"))
            {
                
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                GameManager.instance.ChangeSceneFieldToBattle(
                    new SOBattleCharacter[] { boss }, new int[] { 10 }, (bool _isCleard) =>
                    {
                        if (_isCleard)
                        {
                            UIManager.instance.ShowUI<DialogUI>(new DialogUIData()
                            {   
                                identifier = "DialogUI",
                                data = DataLibrary.instance.GetDialog(this.bossClearDialogIndex),
                            });
                        }
                        else
                        {
                            return;
                        }
                    });
            }
        }
    }
}
