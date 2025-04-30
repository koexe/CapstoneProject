using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacing : MonoBehaviour
{
    private Transform cameraTransform;
    private Quaternion initialRotation;

    private void Awake()
    {
        // 카메라를 한번만 찾아놓기
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
            cameraTransform = mainCamera.transform;
        else
            Debug.LogError("Main Camera not found!");

        // 이 오브젝트의 원래 회전값 저장 (바닥 기준)
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        // 오브젝트의 원래 회전값을 기준으로
        // 카메라의 y축 회전만 따라가도록 설정
        Vector3 cameraEuler = cameraTransform.eulerAngles;

        // 바닥은 눕혀지지 않게 → 카메라의 y축(수평) 방향만 반영
        Quaternion targetRotation = Quaternion.Euler(cameraEuler.x, 0f, 0f);

        transform.rotation = initialRotation * targetRotation;
    }
}