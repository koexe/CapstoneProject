using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MapMovePoint : MonoBehaviour
{
    [SerializeField] Collider coll;





    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        if (this.coll != null)
            Gizmos.DrawWireCube(this.coll.bounds.center, this.coll.bounds.size);

    }
}
