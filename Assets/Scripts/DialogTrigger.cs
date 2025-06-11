using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] int dialogIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")        
        && SaveGameManager.instance.currentSaveData.chatacterDialogs[this.dialogIndex] == false)
        {
            ShowDialog();
        }
    }


    public void ShowDialog()
    {
        UIManager.instance.ShowUI<DialogUI>(new DialogUIData() { 
            identifier = "Dialog", 

            data = DataLibrary.instance.GetDialog(this.dialogIndex)
            });
    }
}
