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

    private DialogData dialogData;
    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    private bool isTyping = false;
    private string fullText = "";

    public float typingSpeed = 0.05f;

    public override void Hide()
    {
        this.contents.SetActive(false);
        this.isShow = false;
        SaveGameManager.instance.currentSaveData.chatacterDialogs[this.dialogData.index] = true;
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
        DialogUIData t_dialogUIData = _data as DialogUIData;
        if (t_dialogUIData == null)
        {
            Debug.Log("Invalid DataType in DialogUI");
            return;
        }
        this.dialogData = t_dialogUIData.data;
    }

    public override void Show(UIData _data)
    {
        ResetDialog();
        this.contents.SetActive(true);
        this.isShow = true;
        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (this.dialogData == null || this.currentLineIndex >= this.dialogData.dialogs.Length)
        {
            Debug.Log("다이얼로그 종료");
            Hide();
            return;
        }
        if(SaveGameManager.instance.currentSaveData.chatacterDialogs[this.dialogData.index] == true)
        {
            this.currentLineIndex = this.dialogData.dialogs.Length - 1;
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
            this.nameText.text = t_speaker;
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
}

public class DialogUIData : UIData
{
    public DialogData data;
    public Action closeAction;
}
