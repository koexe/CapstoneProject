using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : UIBase
{
    LoadingUIData currentData;

    [SerializeField] Image backGround;
    [SerializeField] TextMeshProUGUI loadingText;

    public override void Hide()
    {
        this.contents.SetActive(false);
        this.isShow = false;
    }

    public override void Initialization(UIData _data)
    {
        LoadingUIData t_dialogUIData = _data as LoadingUIData;
        if (t_dialogUIData == null)
        {
            Debug.Log("Invalid DataType in DialogUI");
            return;
        }
        this.currentData = t_dialogUIData;
        if (this.currentData.isTextRequire)
            this.loadingText.gameObject.SetActive(false);
    }

    public override void Show(UIData _data)
    {
        this.isShow = true;
        this.contents.SetActive(true);
        WaitLoading();
    }


    public void ChangeText(string _text)
    {
        this.loadingText.text = _text;
        return;
    }

    async void WaitLoading()
    {
        await this.currentData.task;
        while(this.backGround.color.a != 0)
        {
            Color t_currentColor = this.backGround.color;
            float t_currentAlpha = this.backGround.color.a;
            t_currentAlpha = Mathf.MoveTowards(t_currentAlpha, 0, Time.deltaTime);
            t_currentColor.a = t_currentAlpha;
            this.backGround.color = t_currentColor;

        }
    }

}

class LoadingUIData : UIData
{
    public UniTask task;
    public bool isTextRequire;
}
