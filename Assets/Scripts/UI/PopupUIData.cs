using System;

public class PopupUIData : UIData
{
    public string title;
    public string message;
    public string yesButtonText = "예";
    public string noButtonText = "아니오";
    public Action onYesClicked;
    public Action onNoClicked;
} 