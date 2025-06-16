using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField] CutsceneData cutsceneData;
    [SerializeField] UnityEvent onCutsceneEnd;
    public void ShowCutscene()
    {
        UIManager.instance.ShowUI<CutSceneUI>(
            new CutSceneUIData
            {
                identifier = "CutScene",
                step = this.cutsceneData.steps,
                isAllowMultifle = false,
                cutsceneID = this.cutsceneData.id,
                onHide = this.onCutsceneEnd.Invoke,
                audio = this.cutsceneData.cutsceneBGM,
            });
    }
}
