using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    [SerializeField] private Image cutsceneImage;      // 일러스트 이미지
    [SerializeField] private Image fadeOverlay;        // 검정색 페이드용 오버레이
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private TextMeshProUGUI dialogueText;        // 대사 텍스트 컴포넌트
    [SerializeField] GameObject textBox;
    [SerializeField] private float typingSpeed = 0.05f; // 한 글자당 타이핑 속도

    [System.Serializable]
    public class CutsceneStep
    {
        public Sprite image;
        public float holdTime = 2f;
        [TextArea(3, 10)]
        public string dialogue;    // 대사 내용
        public bool useTypewriter = true; // 타이핑 효과 사용 여부
    }

    [SerializeField] private List<CutsceneStep> steps = new List<CutsceneStep>();

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        fadeOverlay.gameObject.SetActive(true);
        Color fadeColor = fadeOverlay.color;

        foreach (var step in steps)
        {
            cutsceneImage.sprite = step.image;
            // 페이드 아웃 (검은 화면 → 이미지)
            yield return Fade(1f, 0f);

            // 대사 표시
            if (!string.IsNullOrEmpty(step.dialogue))
            {
                textBox.SetActive(true);
                if (step.useTypewriter)
                {
                    yield return TypeDialogue(step.dialogue);
                }
                else
                {
                    dialogueText.text = step.dialogue;
                }
            }
            else
            {
                textBox.SetActive(false);
                dialogueText.text = "";
            }

            // 컷씬 이미지 유지
            yield return CoroutineUtil.WaitForSeconds(step.holdTime);

            // 대사 지우기
            dialogueText.text = "";

            // 페이드 인 (이미지 → 검은 화면)
            yield return Fade(0f, 1f);
        }

        // 컷씬 종료 후 오버레이 제거 (또는 다음 씬 등)
        fadeOverlay.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        float t = 0f;
        Color color = fadeOverlay.color;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            float a = Mathf.Lerp(fromAlpha, toAlpha, t);
            fadeOverlay.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }

        fadeOverlay.color = new Color(color.r, color.g, color.b, toAlpha);
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach (char letter in dialogue)
        {
            dialogueText.text += letter;
            yield return CoroutineUtil.WaitForSeconds(typingSpeed);
        }
    }
}