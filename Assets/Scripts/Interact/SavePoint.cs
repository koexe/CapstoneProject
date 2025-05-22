using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    public void ExecuteAction()
    {
        // SaveUI 표시
        UIManager.instance.ShowUI<SaveUI>(new SaveUIData()
        {
            identifier = "SaveUI",
            isAllowMultifle = false,
            order = 100
        });
    }
} 