using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryBlockWall : MonoBehaviour
{
    [SerializeField] CutsceneData checkCutsceneData;
    [SerializeField] int checkDialogIndex;
    [SerializeField] int blockDialogIndex;
    [SerializeField] bool isWatch;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (this.checkCutsceneData != null)
            {
                if (SaveGameManager.instance.currentSaveData.cutsceneIsShow[this.checkCutsceneData.id] != this.isWatch)
                {
                    this.gameObject.layer = LayerMask.NameToLayer("Wall");
                                        UIManager.instance.ShowUI<DialogUI>(new DialogUIData()
                    {
                        identifier = "DialogUI",
                        data = DataLibrary.instance.GetDialog(this.blockDialogIndex),
                    });
                }
            }
            else
            {
                if (SaveGameManager.instance.CheckStoryIs(this.checkDialogIndex) != this.isWatch)
                {
                    this.gameObject.layer = LayerMask.NameToLayer("Wall");
                    UIManager.instance.ShowUI<DialogUI>(new DialogUIData()
                    {
                        identifier = "DialogUI",
                        data = DataLibrary.instance.GetDialog(this.blockDialogIndex),
                    });
                }
                else
                {
                    this.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
        }
    }
}
