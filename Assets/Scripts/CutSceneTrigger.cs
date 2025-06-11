using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] CutsceneData cutsceneData;
    public void ShowCutscene()
    {
        UIManager.instance.ShowUI<CutSceneUI>(
            new CutSceneUIData
            {
                identifier = "CutScene",
                step = this.cutsceneData.steps,
                isAllowMultifle = false,
                cutsceneID = this.cutsceneData.id,
            });
    }
}
