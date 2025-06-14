using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] protected Transform currentTarget;

    [SerializeField] protected Vector3 areaCenter = Vector3.zero;  // 제한 구역 중심
    [SerializeField] protected Vector3 areaSize = new Vector3(20f, 10f, 10f); // 제한 구역 크기

    public void SetAreaSize(Vector3 _areaCenter, Vector3 _areaSize)
    {
        this.areaCenter = _areaCenter;
        this.areaSize = _areaSize;
    }

    float viewDistance = 10f;

    [SerializeField] protected Vector3 offset = new Vector3(0f, 0f, -10f);

    [SerializeField] protected float basicOffsetY;

    [SerializeField] protected float followSpeed = 5f;

    Camera cam;

    private void Start()
    {
        cam = Camera.main;
        GameManager.instance.SetCamera(this);
    }
    public void SetPosition(Vector3 _position) => this.transform.position = new Vector3(_position.x, this.basicOffsetY, _position.z);
    public void SetTarget(Transform _target) => this.currentTarget = _target;


    protected virtual void FixedUpdate()
    {
        Vector3 targetPosition = currentTarget != null ? currentTarget.position : Vector3.zero;
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(areaCenter.x, areaCenter.y, areaCenter.z);
        Vector3 size = new Vector3(areaSize.x, areaSize.y, areaSize.z);
        Gizmos.DrawWireCube(center, size);
    }
}