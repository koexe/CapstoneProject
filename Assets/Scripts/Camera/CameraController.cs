using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform currentTarget;

    [SerializeField] Vector2 areaCenter = Vector2.zero;  // 제한 구역 중심
    [SerializeField] Vector2 areaSize = new Vector2(20f, 10f); // 제한 구역 크기

    [SerializeField] float followSpeed = 5f;

    Camera cam;

    private void Start()
    {
        cam = Camera.main;
        GameManager.instance.SetCamera(this);
    }

    public void SetTarget(Transform _target) => this.currentTarget = _target;

    private void LateUpdate()
    {
        Vector3 targetPosition = currentTarget != null ? currentTarget.position : Vector3.zero;
        targetPosition.y = transform.position.y;
        targetPosition.z -= 10.0f;

        // 부드럽게 따라가기
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // 카메라 화면 반 크기
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        // 제한 영역의 경계 계산
        float minX = areaCenter.x - areaSize.x * 0.5f + halfWidth;
        float maxX = areaCenter.x + areaSize.x * 0.5f - halfWidth;
        float minY = areaCenter.y - areaSize.y * 0.5f + halfHeight;
        float maxY = areaCenter.y + areaSize.y * 0.5f - halfHeight;

        // 카메라 중심 위치 클램프
        float clampedX = Mathf.Clamp(smoothPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(smoothPosition.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, smoothPosition.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(areaCenter.x, areaCenter.y, 0f);
        Vector3 size = new Vector3(areaSize.x, areaSize.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}