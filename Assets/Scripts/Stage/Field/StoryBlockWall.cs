using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryBlockWall : MonoBehaviour
{
    [SerializeField] int checkDialogIndex;
    [SerializeField] int blockDialogIndex;
    [SerializeField] bool isWatch;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
