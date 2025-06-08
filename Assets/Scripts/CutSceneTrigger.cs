using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] List<CutsceneStep> cutsceneSteps = new List<CutsceneStep>();
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            UIManager.instance.ShowUI<CutSceneUI>(
                new CutSceneUIData
                {
                    identifier = "CutScene",
                    step = this.cutsceneSteps,
                    isAllowMultifle = false,
                });
        }
    }
}
