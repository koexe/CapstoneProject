using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldInteractable : MonoBehaviour, IInteractable
{
    [Header("상호작용 범위")]
    [SerializeField] Vector2 interactArea;
    [Header("현재 상호작용 가능한지")]
    [SerializeField] protected bool isCanInteract;
    [Header("상호작용 확인할 레이어")]
    [SerializeField] LayerMask interactLayerMask;
    [Header("상호작용 대기 시간(홀드)")]
    [SerializeField] float interactTime;
    [SerializeField] float currentInteractTime;

    public virtual void ExecuteAction()
    {
        if (this.isCanInteract)
        {
            if (this.interactTime != this.currentInteractTime)
            {
                this.currentInteractTime = Mathf.MoveTowards(this.currentInteractTime, this.interactTime, Time.fixedDeltaTime);
            }
            else
            {
                RealAction();
                this.currentInteractTime = 0f;
            }
        }
        return;
    }

    protected virtual void RealAction()
    {
        Debug.Log($"상호작용 실행   {this.name}");
        return;
    }


    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, this.interactArea);
    }
}
