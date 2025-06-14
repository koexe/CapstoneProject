using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraController : CameraController
{
    [SerializeField] private float maxDistanceFromCenter = 5f; // 중앙에서 최대 이동 가능 거리

    protected override void FixedUpdate()
    {
        Vector3 targetPosition;
        if (currentTarget != null)
        {
            // 타겟이 있을 경우, 중앙에서의 거리를 제한
            Vector3 directionToTarget = currentTarget.position - areaCenter;
            Vector2 horizontalDirection = new Vector2(directionToTarget.x, directionToTarget.z);
            
            // x,z 평면에서의 거리 제한
            if (horizontalDirection.magnitude > maxDistanceFromCenter)
            {
                horizontalDirection = horizontalDirection.normalized * maxDistanceFromCenter;
                directionToTarget.x = horizontalDirection.x;
                directionToTarget.z = horizontalDirection.y;
            }
            directionToTarget.y = this.offset.y;

            targetPosition = areaCenter + directionToTarget;
        }
        else
        {
            // 타겟이 없을 경우 중앙으로
            targetPosition = areaCenter;
        }

        targetPosition.y = this.basicOffsetY;
        targetPosition += offset;

        // 부드럽게 따라가기
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // 제한 영역 계산 (xz축만)
        float minX = areaCenter.x - areaSize.x * 0.5f;
        float maxX = areaCenter.x + areaSize.x * 0.5f;

        float minZ = areaCenter.z - areaSize.z * 0.5f;
        float maxZ = areaCenter.z + areaSize.z * 0.5f;

        // x, z 클램프
        float clampedX = Mathf.Clamp(smoothPosition.x, minX, maxX);
        float clampedZ = Mathf.Clamp(smoothPosition.z, minZ, maxZ);

        // y는 그대로
        transform.position = new Vector3(clampedX, smoothPosition.y, clampedZ);
    }
}
