using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryBlockWall : MonoBehaviour
{
    [SerializeField] int checkDialogIndex;
    [SerializeField] int faildDialogIndex;
    [SerializeField] bool isWatch;
    [SerializeField] Collider blockCollider;
    [SerializeField] LayerMask blockMask;
    [SerializeField] LayerMask defaultMask;

    private void OnTriggerEnter(Collider other)
    {
        if (SaveGameManager.instance.CheckStoryIs(this.checkDialogIndex) != this.isWatch)
        {
            this.gameObject.layer = this.blockMask;
            
        }
        else 
        {
            this.gameObject.layer = this.defaultMask;
        }
    }
}
