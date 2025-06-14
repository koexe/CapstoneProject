using UnityEngine;
using Spine.Unity;

public class SpineOutlineEffect : MonoBehaviour
{
    [SerializeField] private Color outlineColor = Color.white;
    [SerializeField] private float outlineWidth = 0.1f;
    [SerializeField] private Material outlineMaterial;

    private SkeletonAnimation skeletonAnimation;
    private MeshRenderer[] meshRenderers;
    private Material[] originalMaterials;
    private Material[] outlineMaterials;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        if (skeletonAnimation == null)
        {
            Debug.LogError("SkeletonAnimation 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        // 외곽선 머티리얼이 없으면 생성
        if (outlineMaterial == null)
        {
            outlineMaterial = new Material(Shader.Find("Sprites/Default"));
            outlineMaterial.SetColor("_Color", outlineColor);
        }

        // MeshRenderer 컴포넌트들 가져오기
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[meshRenderers.Length];
        outlineMaterials = new Material[meshRenderers.Length];

        // 각 MeshRenderer의 원본 머티리얼 저장
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
            outlineMaterials[i] = new Material(outlineMaterial);
        }
    }

    public void ShowOutline(bool show)
    {
        if (meshRenderers == null) return;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (show)
            {
                // 외곽선 표시
                meshRenderers[i].material = outlineMaterials[i];
                // 외곽선 크기 조정
                meshRenderers[i].transform.localScale = Vector3.one * (1f + outlineWidth);
            }
            else
            {
                // 원본 머티리얼로 복원
                meshRenderers[i].material = originalMaterials[i];
                // 원본 크기로 복원
                meshRenderers[i].transform.localScale = Vector3.one;
            }
        }
    }

    private void OnDestroy()
    {
        // 생성된 머티리얼 정리
        if (outlineMaterials != null)
        {
            foreach (var material in outlineMaterials)
            {
                if (material != null)
                    Destroy(material);
            }
        }
    }
} 