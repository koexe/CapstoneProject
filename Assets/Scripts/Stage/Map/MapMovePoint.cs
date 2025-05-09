using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MapMovePoint : MonoBehaviour
{
    [SerializeField] MapEntity.MapPath path;
    const float waitTime = 1.0f;
    [SerializeField] float currentWaitTime;
    [SerializeField] Collider coll;

    private void Start()
    {
        this.currentWaitTime = waitTime;
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerInputModule>(out var t_moveModule))
        {
            if (this.currentWaitTime == 0f)
            {
                MapManager.instance.MoveMap(this.path);
                this.currentWaitTime = waitTime;
            }
            else
            {
                this.currentWaitTime = Mathf.MoveTowards(this.currentWaitTime, 0f, Time.fixedDeltaTime);
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        if (this.coll != null)
            Gizmos.DrawWireCube(this.coll.bounds.center, this.coll.bounds.size);

    }
}
