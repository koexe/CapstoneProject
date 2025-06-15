using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractModule : MonoBehaviour
{
    [SerializeField] KeyCode interactKey = KeyCode.F;
    [SerializeField] Vector3 interactArea = Vector3.one;
    [SerializeField] LayerMask interactableLayer;

    private void Start()
    {
        IngameInputManager.instance.AddInput(this.interactKey, IngameInputManager.InputEventType.Down, Interact);
    }

    void Interact()
    {
        if (GameManager.instance.GetGameState() == GameState.Field)
        {
            var t_result = Physics.OverlapBox(this.transform.position, this.interactArea, Quaternion.identity, this.interactableLayer);
            foreach (var t_interacable in t_result)
            {
                if (t_interacable.TryGetComponent<IInteractable>(out var t_component))
                {
                    t_component.ExecuteAction();
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(this.transform.position, this.interactArea);
    }
}
