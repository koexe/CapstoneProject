using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance;

    [SerializeField] private Image fadePanel;
    [SerializeField] private Canvas fadeCanvas;
    private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // 페이드 패널이 없다면 생성
            if (this.fadePanel == null)
            {
                this.fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                this.fadeCanvas.sortingOrder = 999; // 최상단에 표시
                
                var t_fadeObj = new GameObject("FadePanel");
                t_fadeObj.transform.SetParent(this.fadeCanvas.transform);
                
                this.fadePanel = t_fadeObj.AddComponent<Image>();
                this.fadePanel.color = Color.black;
                this.fadePanel.raycastTarget = false;
                
                var t_rectTransform = this.fadePanel.GetComponent<RectTransform>();
                t_rectTransform.anchorMin = Vector2.zero;
                t_rectTransform.anchorMax = Vector2.one;
                t_rectTransform.sizeDelta = Vector2.zero;
                
                this.fadePanel.gameObject.SetActive(false);
            }
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void LoadScene_Async(string _sceneName)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScene_Async(_sceneName));
    }

    IEnumerator ChangeScene_Async(string _sceneName)
    {
        // 페이드 인
        this.fadePanel.gameObject.SetActive(true);
        float t_elapsed = 0f;
        
        while (t_elapsed < this.fadeDuration)
        {
            t_elapsed += Time.deltaTime;
            this.fadePanel.color = new Color(0, 0, 0, t_elapsed / this.fadeDuration);
            yield return null;
        }

        // 씬 로드
        AsyncOperation t_asyncLoad = SceneManager.LoadSceneAsync(_sceneName);
        t_asyncLoad.allowSceneActivation = false;

        while (t_asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        t_asyncLoad.allowSceneActivation = true;
        
        while (!t_asyncLoad.isDone)
        {
            yield return null;
        }

        // 페이드 아웃
        t_elapsed = this.fadeDuration;
        while (t_elapsed > 0)
        {
            t_elapsed -= Time.deltaTime;
            this.fadePanel.color = new Color(0, 0, 0, t_elapsed / this.fadeDuration);
            yield return null;
        }

        this.fadePanel.gameObject.SetActive(false);
    }
}
