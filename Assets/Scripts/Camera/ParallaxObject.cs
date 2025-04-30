using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Transform))]
public class ParallaxObject : MonoBehaviour
{
    [SerializeField, Tooltip("카메라 움직임 대비 이 오브젝트가 움직이는 비율. 1 = 동일하게 움직임, 0 = 고정")]
    private float parallaxFactor = 0.5f;

    private Vector3 initialPosition;
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
        initialPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 camDelta = cam.position - initialPosition;
        Vector3 newPosition = initialPosition + camDelta * parallaxFactor;
        transform.position = new Vector3(newPosition.x, transform.position.y, transform.position.z);
    }
}