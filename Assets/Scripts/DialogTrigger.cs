using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] int dialogIndex;
    [SerializeField] CutsceneData cutsceneData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (this.cutsceneData != null)
            {
                ShowDialog();
            }
            else if (SaveGameManager.instance.currentSaveData.chatacterDialogs[this.dialogIndex] == false)
            {
                ShowDialog();
            }
            else
                return;
        }
    }


    public void ShowDialog()
    {
        if (this.cutsceneData != null)
        {
            UIManager.instance.ShowUI<CutSceneUI>(new CutSceneUIData()
            {
                identifier = "Cutscene",
                step = this.cutsceneData.steps,
                cutsceneID = this.cutsceneData.id
            });
        }
        else
        {
            UIManager.instance.ShowUI<DialogUI>(new DialogUIData()
            {
                identifier = "Dialog",

                data = DataLibrary.instance.GetDialog(this.dialogIndex)
            });
        }
    }
}
