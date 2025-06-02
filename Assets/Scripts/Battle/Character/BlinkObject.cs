using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float blinkTime = 1f;        // 깜빡임 주기
    [SerializeField] float minAlpha = 0.2f;       // 최소 투명도
    [SerializeField] float maxAlpha = 1f;         // 최대 투명도

    private float currentTime = 0f;

    void FixedUpdate()
    {
        currentTime += Time.fixedDeltaTime;
        
        // Sin 함수를 사용하여 0~1 사이의 값을 생성
        float alpha = (Mathf.Sin(currentTime * blinkTime * Mathf.PI) + 1f) * 0.5f;
        
        // minAlpha와 maxAlpha 사이의 값으로 변환
        alpha = Mathf.Lerp(minAlpha, maxAlpha, alpha);
        
        // 현재 색상을 가져와서 알파값만 변경d
        Color currentColor = spriteRenderer.color;
        currentColor.a = alpha;
        spriteRenderer.color = currentColor;
    }
}
