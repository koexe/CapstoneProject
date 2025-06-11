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
    [SerializeField] Slider loadingBar;

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
        
        // 로딩 바 초기화
        if (this.loadingBar != null)
        {
            this.loadingBar.value = 0f;
        }
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

    public void UpdateProgress(float _progress)
    {
        if (this.loadingBar != null)
        {
            StartCoroutine(AnimateProgressBar(_progress));
        }
    }

    private IEnumerator AnimateProgressBar(float targetProgress)
    {
        float currentProgress = this.loadingBar.value;
        float duration = 0.5f; // 0.5초 동안 애니메이션
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            this.loadingBar.value = Mathf.Lerp(currentProgress, targetProgress, normalizedTime);
            yield return null;
        }

        this.loadingBar.value = targetProgress;
    }

    async void WaitLoading()
    {
        await this.currentData.task;
    }

}

public class LoadingUIData : UIData
{
    public UniTask task;
}
