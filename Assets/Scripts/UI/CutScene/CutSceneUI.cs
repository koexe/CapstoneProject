using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneUI : UIBase
{
    [SerializeField] private Image cutsceneImage;      // 일러스트 이미지
    [SerializeField] private Image fadeOverlay;        // 검정색 페이드용 오버레이
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private TextMeshProUGUI dialogueText;        // 대사 텍스트 컴포넌트
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject textBox;
    [SerializeField] GameObject nameBox;
    [SerializeField] private float typingSpeed = 0.05f; // 한 글자당 타이핑 속도
    [SerializeField] CutSceneUIData uiData;

    [SerializeField] private List<CutsceneStep> steps = new List<CutsceneStep>();

    //private void Start()
    //{
    //    StartCoroutine(PlayCutscene());
    //}

    private IEnumerator PlayCutscene()
    {
        fadeOverlay.gameObject.SetActive(true);
        Color fadeColor = fadeOverlay.color;

        foreach (var step in steps)
        {
            cutsceneImage.sprite = step.image;

            // 페이드 아웃 (검은 화면 → 이미지)
            yield return Fade(1f, 0f);

            if (!string.IsNullOrEmpty(step.dialogue))
            {
                textBox.SetActive(true);

                // 대사를 줄 단위로 분할
                string[] t_lines = step.dialogue.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (string t_line in t_lines)
                {
                    var t_cols = Regex.Split(t_line, "\\*");
                    string t_text;

                    if (t_cols.Length == 2)
                    {
                        this.nameBox.SetActive(true);
                        this.nameText.text = t_cols[0];
                        t_text = t_cols[1];
                    }
                    else
                    {
                        this.nameBox.SetActive(false);
                        t_text = t_cols[0];
                    }

                    dialogueText.text = "";

                    if (step.useTypewriter)
                    {
                        yield return TypeDialogue(t_text);
                    }
                    else
                    {
                        dialogueText.text = t_line;
                    }

                    // 클릭 대기
                    yield return WaitForClick();
                }
            }
            else
            {
                textBox.SetActive(false);
                dialogueText.text = "";
            }

            textBox.SetActive(false);
            nameBox.SetActive(false);   
            yield return WaitForClick();
            yield return CoroutineUtil.WaitForSeconds(step.holdTime);

            dialogueText.text = "";

            // 페이드 인 (이미지 → 검은 화면)
            yield return Fade(0f, 1f);
        }

        Hide();
        fadeOverlay.gameObject.SetActive(false);
    }
    private IEnumerator WaitForClick()
    {
        // 입력이 들어올 때까지 대기
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
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

    public override void Initialization(UIData _data)
    {
        this.contents.SetActive(true);
        if (_data is CutSceneUIData saveUIData)
        {
            uiData = saveUIData;
        }
        else
        {
            LogUtil.Log("Invalid DataType in CutsceneUI");
            Hide();
            return;
        }

        this.steps = this.uiData.step;
    }

    public override void Show(UIData _data)
    {
        if (_data is CutSceneUIData cutSceneData)
        {
            uiData = cutSceneData;
        }
        else
        {
            LogUtil.Log("Invalid DataType in CutsceneUI");
            Hide();
            return;
        }

        if (SaveGameManager.instance.GetCurrentSaveData().cutsceneIsShow[uiData.cutsceneID])
        {
            Hide();
            return;
        }
        else
        {
            SaveGameManager.instance.GetCurrentSaveData().cutsceneIsShow[uiData.cutsceneID] = true;
            GameManager.instance.ChangeGameState(GameState.Pause);
            this.steps = this.uiData.step;
            StartCoroutine(PlayCutscene());
            this.contents.SetActive(true);
            this.isShow = true;
        }



    }

    public override void Hide()
    {
        this.contents.SetActive(false);
        this.isShow = false;
        GameManager.instance.ChangeGameState(GameState.Field);
        StopAllCoroutines();
    }
}

[System.Serializable]
public class CutsceneStep
{
    public Sprite image;
    public float holdTime = 2f;
    [TextArea(3, 10)]
    public string dialogue;    // 대사 내용
    public bool useTypewriter = true; // 타이핑 효과 사용 여부
}

public class CutSceneUIData : UIData
{
    public string cutsceneID;
    public List<CutsceneStep> step;
}