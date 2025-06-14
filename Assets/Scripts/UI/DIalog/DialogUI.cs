using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : UIBase
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image[] portraits;
    [SerializeField] private GameObject nameBox;

    [SerializeField] private Button[] choiceBoxs;
    [SerializeField] private TextMeshProUGUI[] choiceTexts;


    private DialogData dialogData;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    private bool isTyping = false;
    private string fullText = "";

    public float typingSpeed = 0.05f;

    private int originalDialogIndexForCallback = -1;
    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
        }
        #endif
    }
    public override void Hide()
    {
        this.contents.SetActive(false);
        this.isShow = false;
        SaveGameManager.instance.currentSaveData.chatacterDialogs[this.dialogData.index] = true;
        GameManager.instance.SetGameState(GameState.Field);
    }
    public void ResetDialog()
    {
        this.currentLineIndex = 0;
        this.isTyping = false;
        this.fullText = "";
        this.dialogText.text = "";
    }

    public override void Initialization(UIData _data)
    {

    }

    /// <summary>
    /// 선택지가 선택되었을 때 호출되는 메서드
    /// </summary>
    /// <param name="choiceIndex">선택된 선택지의 인덱스</param>
    public void OnChoiceSelected(int choiceIndex)
    {
        if (dialogData == null || choiceIndex >= dialogData.choices.Length) return;

        // 모든 선택지 박스 비활성화
        HideAllChoices();

        // 현재 대화 인덱스 저장 (나중에 돌아오기 위해)
        int originalDialogIndex = dialogData.index;

        // 선택된 선택지의 다음 대화 인덱스 가져오기
        int nextDialogIndex = dialogData.choices[choiceIndex].Item2;

        // 다음 대화가 있는지 확인
        if (nextDialogIndex > 0 && nextDialogIndex < DataLibrary.instance.GetDialogTable().Count)
        {
            // 새로운 대화 데이터로 전환
            DialogData nextDialogData = DataLibrary.instance.GetDialog(nextDialogIndex);
            this.dialogData = nextDialogData;
            ResetDialog();
            ShowCurrentLine();

            // 선택지 대화 완료를 추적하기 위한 콜백 설정
            SetChoiceDialogCompleteCallback(originalDialogIndex);
        }
        else
        {
            // 다음 대화가 없으면 원래 대화로 돌아가서 모든 선택지 확인
            this.dialogData = DataLibrary.instance.GetDialog(originalDialogIndex);
            ResetDialog();
            this.currentLineIndex = this.dialogData.dialogs.Length; // 대화 끝으로 이동
            ShowCurrentLine();
        }
    }

    /// <summary>
    /// 선택지 대화 완료 콜백을 설정하는 메서드
    /// </summary>
    /// <param name="originalDialogIndex">원래 대화 인덱스</param>
    private void SetChoiceDialogCompleteCallback(int originalDialogIndex)
    {
        originalDialogIndexForCallback = originalDialogIndex;
    }

    /// <summary>
    /// 모든 선택지 박스를 숨기는 메서드
    /// </summary>
    private void HideAllChoices()
    {
        for (int i = 0; i < choiceBoxs.Length; i++)
        {
            if (choiceBoxs[i] != null)
            {
                choiceBoxs[i].gameObject.SetActive(false);
            }
        }
    }

    public override void Show(UIData _data)
    {
        DialogUIData t_dialogUIData = _data as DialogUIData;
        if (t_dialogUIData == null)
        {
            Debug.Log("Invalid DataType in DialogUI");
            return;
        }
        this.dialogData = t_dialogUIData.data;

        // 선택지 버튼 이벤트 설정
        for (int i = 0; i < choiceBoxs.Length; i++)
        {
            if (choiceBoxs[i] != null)
            {
                // 기존 리스너 제거
                choiceBoxs[i].onClick.RemoveAllListeners();

                // 새로운 리스너 추가 (클로저를 피하기 위해 로컬 변수 사용)
                int choiceIndex = i;
                choiceBoxs[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));

                // 초기에는 선택지 박스 비활성화
                choiceBoxs[i].gameObject.SetActive(false);
            }
        }

        // dialogData는 이미 Initialization에서 설정됨
        ResetDialog();
        this.contents.SetActive(true);
        this.isShow = true;
        if (SaveGameManager.instance.currentSaveData.chatacterDialogs[this.dialogData.index] == true)
        {
            this.currentLineIndex = this.dialogData.dialogs.Length - 1;
        }
        ShowCurrentLine();
        GameManager.instance.SetGameState(GameState.Pause);
    }

    void ShowCurrentLine()
    {
        if (this.dialogData == null || this.currentLineIndex >= this.dialogData.dialogs.Length)
        {
            if (this.dialogData.choices != null && this.dialogData.choices.Length > 0)
            {
                // 선택지 표시
                for (int i = 0; i < this.dialogData.choices.Length; i++)
                {
                    if (i < choiceBoxs.Length) // 배열 범위 체크
                    {
                        this.choiceBoxs[i].gameObject.SetActive(true);
                        this.choiceTexts[i].text = this.dialogData.choices[i].Item1;
                    }
                }
            }
            else if (this.dialogData.autoNextDialog != -1)
            {
                this.dialogData = DataLibrary.instance.GetDialog(this.dialogData.autoNextDialog);
                ResetDialog();
                ShowCurrentLine();
            }
            else
            {
                Hide();
            }
            return;
        }

        ShowPortraits(this.dialogData.characters[this.currentLineIndex]);
        var (t_speaker, t_line) = this.dialogData.dialogs[this.currentLineIndex];

        if (string.IsNullOrEmpty(t_speaker))
        {
            this.nameBox.SetActive(false);
        }
        else
        {
            this.nameBox.SetActive(true);
            this.nameText.text = GameStatics.GetName(t_speaker);
        }

        if (this.typingCoroutine != null)
            StopCoroutine(this.typingCoroutine);

        this.typingCoroutine = StartCoroutine(TypeLine(t_line));
    }

    IEnumerator TypeLine(string _line)
    {
        this.isTyping = true;
        this.fullText = _line;
        this.dialogText.text = "";

        for (int t_i = 0; t_i < _line.Length; t_i++)
        {
            this.dialogText.text += _line[t_i];
            yield return new WaitForSeconds(this.typingSpeed);
        }

        this.isTyping = false;
    }

    public void OnClickNext()
    {
        if (this.isTyping)
        {
            StopCoroutine(this.typingCoroutine);
            this.dialogText.text = this.fullText;
            this.isTyping = false;
        }
        else
        {
            this.currentLineIndex++;
            ShowCurrentLine();
        }
    }

    void ShowPortraits((string, int)[] _currentCharacters)
    {
        foreach (var t_img in this.portraits)
        {
            t_img.sprite = null;
            t_img.color = new Color(0, 0, 0, 0);
        }

        int t_count = _currentCharacters.Length;

        for (int i = 0; i < t_count; i++)
        {
            int t_slotIndex = (t_count == 1) ? 0 : (t_count == 2 ? (i == 0 ? 0 : 2) : i);

            var (t_charName, t_index) = _currentCharacters[i];
            Sprite t_portrait = DataLibrary.instance.GetPortrait(t_charName, t_index);

            if (t_portrait != null)
            {
                this.portraits[t_slotIndex].sprite = t_portrait;
                this.portraits[t_slotIndex].gameObject.SetActive(true);
                this.portraits[t_slotIndex].color = new Color(1, 1, 1, 1);
            }
        }
    }

    /// <summary>
    /// 마지막 대화인지 확인하는 메서드
    /// </summary>
    /// <returns>마지막 대화면 true, 아니면 false</returns>
    private bool IsLastDialog()
    {


        
        return this.currentLineIndex >= this.dialogData.dialogs.Length - 1;
    }
}

public class DialogUIData : UIData
{
    public DialogData data;
    public Action closeAction;
}
