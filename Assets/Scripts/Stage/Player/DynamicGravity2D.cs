using UnityEngine;

public class DynamicGravity2D : MonoBehaviour
{
    [SerializeField] private Collider coll3D;
    [SerializeField] private LayerMask wallMask;

    public Vector3 UpdateCheckWall(Vector3 _nextMovement)
    {
        var t_colls = Physics.OverlapBox(
            coll3D.bounds.center + _nextMovement,
            coll3D.bounds.extents,
            Quaternion.identity,
            wallMask
        );

        Vector3 t_pushValue = Vector3.zero;

        foreach (var t in t_colls)
        {
            Bounds t_bounds1 = coll3D.bounds;
            Bounds t_bounds2 = t.bounds;

            // XZ 기준으로만 고려
            Vector2 t_center1 = new Vector2(t_bounds1.center.x + _nextMovement.x, t_bounds1.center.z + _nextMovement.z);
            Vector2 t_size1 = new Vector2(t_bounds1.size.x, t_bounds1.size.z);

            Vector2 t_center2 = new Vector2(t_bounds2.center.x, t_bounds2.center.z);
            Vector2 t_size2 = new Vector2(t_bounds2.size.x, t_bounds2.size.z);

            Rect t_rect1 = new Rect(t_center1 - t_size1 * 0.5f, t_size1);
            Rect t_rect2 = new Rect(t_center2 - t_size2 * 0.5f, t_size2);

            Rect t_overlap = GetOverlapArea(t_rect1, t_rect2);
            if (t_overlap.width <= 0 || t_overlap.height <= 0) continue;

            if (t_overlap.width < t_overlap.height)
            {
                // x축으로 밀기
                if (t_overlap.center.x > t_bounds1.center.x)
                    t_pushValue.x -= t_overlap.width;
                else
                    t_pushValue.x += t_overlap.width;
            }
            else
            {
                // z축으로 밀기
                if (t_overlap.center.y > t_bounds1.center.z)
                    t_pushValue.z -= t_overlap.height;
                else
                    t_pushValue.z += t_overlap.height;
            }
        }

        return new Vector3(t_pushValue.x, 0f, t_pushValue.z);
    }

    private Rect GetOverlapArea(Rect _rect1, Rect _rect2)
    {
        float xMin = Mathf.Max(_rect1.xMin, _rect2.xMin);
        float xMax = Mathf.Min(_rect1.xMax, _rect2.xMax);
        float yMin = Mathf.Max(_rect1.yMin, _rect2.yMin);
        float yMax = Mathf.Min(_rect1.yMax, _rect2.yMax);

        if (xMax >= xMin && yMax >= yMin)
        {
            return new Rect(new Vector2(xMin, yMin), new Vector2(xMax - xMin, yMax - yMin));
        }
        else
        {
            return Rect.zero;
        }
    }

}

