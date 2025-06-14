using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private int dialogIndex;  // 대화 데이터의 인덱스

    [SerializeField] CutsceneData cutsceneData;

    public void ExecuteAction()
    {
        if (this.cutsceneData != null)
        {
            UIManager.instance.ShowUI<CutSceneUI>(new CutSceneUIData()
            {
                identifier = "CutSceneUI",
                cutsceneID = this.cutsceneData.id,
                step = this.cutsceneData.steps,
            });
        }
        else
        {
            // 대화 데이터 가져오기
            DialogData dialogData = DataLibrary.instance.GetDialog(this.dialogIndex);

            if (dialogData != null)
            {
                if(dialogIndex == 10)
                {
                    SaveGameManager.instance.GetCurrentSaveData().isMetLemo = true;
                }



                // DialogUI 표시
                UIManager.instance.ShowUI<DialogUI>(new DialogUIData()
                {
                    identifier = "DialogUI",
                    data = dialogData
                });
            }
            else
            {
                Debug.LogWarning($"대화 데이터를 찾을 수 없습니다. Index: {this.dialogIndex}");
            }
        }
    }
}