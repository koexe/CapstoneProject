using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupUI : UIBase
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button yesButton;
    [SerializeField] TextMeshProUGUI yesButtonText;
    [SerializeField] Button noButton;
    [SerializeField] TextMeshProUGUI noButtonText;

    PopupUIData popupData;

    public override void Initialization(UIData _data)
    {
        if (_data is PopupUIData data)
        {
            popupData = data;
            titleText.text = data.title;
            messageText.text = data.message;
            yesButtonText.text = data.yesButtonText;
            noButtonText.text = data.noButtonText;

            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();

            yesButton.onClick.AddListener(() => 
            {
                popupData.onYesClicked?.Invoke();
                Hide();
            });

            noButton.onClick.AddListener(() => 
            {
                popupData.onNoClicked?.Invoke();
                Hide();
            });
        }
    }

    public override void Show(UIData _data)
    {
        base.data = _data;
        contents.SetActive(true);
        isShow = true;
    }

    public override void Hide()
    {
        contents.SetActive(false);
        isShow = false;
        base.data?.onHide?.Invoke();
    }
} 