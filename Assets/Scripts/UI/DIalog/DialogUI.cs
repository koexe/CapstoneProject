using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : UIBase
{
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image[] portraits;

    [SerializeField] GameObject nameBox;

    DialogData dialogData;

    private int currentLineIndex = 0;
    private Coroutine typingCoroutine;

    private bool isTyping = false;
    private string fullText = "";

    public float typingSpeed = 0.05f;
    public override void Hide()
    {

    }

    public override void Initialization(UIData data)
    {
        DialogUIData t_dialogUIData = data as DialogUIData;
        if (t_dialogUIData == null)
        {
            Debug.Log("Invalid DataType in DialogUI");
            return;
        }
        this.dialogData = t_dialogUIData.data;


    }

    public override void Show(UIData _data)
    {
        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (dialogData == null || currentLineIndex >= dialogData.dialogs.Length)
        {
            Debug.Log("��� ����");
            return;
        }
        ShowPortraits(dialogData.characters[currentLineIndex]);
        var (speaker, line) = dialogData.dialogs[currentLineIndex];

        if (string.IsNullOrEmpty(speaker))
        {
            nameBox.SetActive(false);
        }
        else
        {
            nameBox.SetActive(true);
            nameText.text = speaker;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        fullText = line;
        dialogText.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            dialogText.text += line[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void OnClickNext()
    {
        if (isTyping)
        {
            // Ÿ���� �� �� ��ü ���
            StopCoroutine(typingCoroutine);
            dialogText.text = fullText;
            isTyping = false;
        }
        else
        {
            // ���� �ٷ� �Ѿ
            currentLineIndex++;
            ShowCurrentLine();
        }
    }
    void ShowPortraits((string, int)[] currentCharacters)
    {
        // ���� �� ����
        foreach (var img in portraits)
        {
            img.gameObject.SetActive(false);
        }

        int count = currentCharacters.Length;

        // ĳ���� ���� ���� ��ġ ����
        for (int i = 0; i < count; i++)
        {
            int slotIndex = (count == 1) ? 1 : (count == 2 ? (i == 0 ? 0 : 2) : i); // 1�� = ���, 2�� = �¿�, 3�� = ���߿�

            var (t_charName, t_index) = currentCharacters[i];
            Sprite portrait = DataLibrary.instance.GetPortrait(t_charName, t_index);

            if (portrait != null)
            {
                portraits[slotIndex].sprite = portrait;
                portraits[slotIndex].gameObject.SetActive(true);
            }
        }
    }
}

public class DialogUIData : UIData
{
    public DialogData data;
    public Action closeAction;
}