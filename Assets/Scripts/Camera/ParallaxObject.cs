using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Transform))]
public class ParallaxObject : MonoBehaviour
{
    [SerializeField, Tooltip("카메라 움직임 대비 이 오브젝트가 움직이는 비율. 1 = 동일하게 움직임, 0 = 고정")]
    private float parallaxFactor = 0.5f;

    [SerializeField]
    bool isApplyZaxis = true;
    bool isActavated = false;

    [SerializeField] Vector3 initialPosition;
    [SerializeField] Vector3 initialCameraPosition;
    private Transform cam;

    public void Initialization()
    {
        cam = Camera.main.transform;
        initialPosition = transform.position;
        initialCameraPosition = Vector3.zero;
        this.isActavated = true;

    }

    private void LateUpdate()
    {
        if (!this.isActavated)
        {
            return;
        }
        Vector3 camDelta = cam.position - initialCameraPosition;
        Vector3 newPosition = initialPosition + camDelta * parallaxFactor;
        transform.position = new Vector3(newPosition.x, transform.position.y, this.isApplyZaxis ? newPosition.z : this.transform.position.z); 
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        this.initialPosition = this.transform.position;
    }
}