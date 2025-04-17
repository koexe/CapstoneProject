using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        SceneLoadManager.DontDestroyOnLoad(this);

        return;
    }
    public void LoadScene_Async(GameObject _LoadingUI, string _Scenename)
    {
        StopAllCoroutines();
        StartCoroutine(ChangeScene_Async(_LoadingUI, _Scenename));
        return;
    }


    IEnumerator ChangeScene_Async(GameObject _LoadingUI, string _SceneName)
    {
        AsyncOperation t_asyncOper = SceneManager.LoadSceneAsync(_SceneName, LoadSceneMode.Single);
        t_asyncOper.allowSceneActivation = false;
        while (!t_asyncOper.isDone)
        {
            // 로딩이 거의 완료되었는지 확인 (progress는 0.0f ~ 0.9f 사이)
            if (t_asyncOper.progress >= 0.9f)
            {
                // 추가적인 로딩 UI를 처리할 수 있음 (필요시)
                // 사용자가 특정 버튼을 누르거나, 특정 조건이 충족될 때 씬을 활성화할 수 있음

                // 여기에 대기 시간을 추가해도 됨
                yield return new WaitForSeconds(1.5f);

                // allowSceneActivation을 true로 설정하여 씬 활성화
                t_asyncOper.allowSceneActivation = true;
            }
            yield return null; // 매 프레임마다 대기
        }

        yield break;
    }
}
