using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] Canvas canvas;
    [SerializeField] Transform uiRoot; // UI들이 생성될 부모 Transform

    private Dictionary<string, UIBase> activeUIs = new Dictionary<string, UIBase>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (canvas == null)
                canvas = GetComponent<Canvas>();
            if (uiRoot == null)
                uiRoot = canvas.transform;

            return;
        }
        Destroy(gameObject);
    }

    public void HideAllUI()
    {
        foreach (var ui in activeUIs)
        {
            Destroy(ui.Value.gameObject);
        }
        this.activeUIs.Clear();
    }

    public void HideUI(string identifier)
    {
        if (activeUIs.TryGetValue(identifier, out var ui))
        {
            ui.Hide();
        }
    }
    public T ShowUI<T>(GameObject _preafab ,UIData data) where T : UIBase
    {
        // 이미 존재하는 UI 체크
        if (activeUIs.TryGetValue(data.identifier, out var existingUI))
        {
            if (!data.isAllowMultifle)
            {
                if (existingUI.isShow)
                {
                    existingUI.Hide();
                }
                else
                {
                    existingUI.Show(data);
                }
                return existingUI as T;
            }
        }

        // UI 프리팹 가져오기
        GameObject uiPrefab = _preafab;
        if (uiPrefab == null)
        {
            Debug.LogError($"UI 프리팹을 찾을 수 없습니다: {typeof(T).Name}");
            return null;
        }

        // UI 인스턴스 생성
        T uiInstance = Instantiate(uiPrefab, uiRoot).GetComponent<T>();
        if (uiInstance == null)
        {
            Debug.LogError($"UI 컴포넌트를 찾을 수 없습니다: {typeof(T).Name}");
            return null;
        }

        uiInstance.sortingGroup.sortingLayerName = "UIElements";

        // 정렬 순서 설정
        if (data.order == -1)
        {
            int minOrder = 9999;
            foreach (var sortingGroup in uiRoot.GetComponentsInChildren<SortingGroup>())
            {
                if (sortingGroup.sortingOrder < minOrder)
                    minOrder = sortingGroup.sortingOrder;
            }
            uiInstance.sortingGroup.sortingOrder = minOrder;
        }
        else
        {
            uiInstance.sortingGroup.sortingOrder = data.order;
        }

        // UI 초기화 및 표시
        activeUIs[data.identifier] = uiInstance;
        uiInstance.Initialization(data);
        uiInstance.Show(data);

        return uiInstance;
    }

    public T ShowUI<T>(UIData data) where T : UIBase
    {
        // 이미 존재하는 UI 체크
        if (activeUIs.TryGetValue(data.identifier, out var existingUI))
        {
            if (!data.isAllowMultifle)
            {
                if (existingUI.isShow)
                {
                    existingUI.Hide();
                }
                else
                {
                    existingUI.Show(data);
                }
                return existingUI as T;
            }
        }

        // UI 프리팹 가져오기
        GameObject uiPrefab = DataLibrary.instance.GetUI(typeof(T).Name);
        if (uiPrefab == null)
        {
            Debug.LogError($"UI 프리팹을 찾을 수 없습니다: {typeof(T).Name}");
            return null;
        }

        // UI 인스턴스 생성
        T uiInstance = Instantiate(uiPrefab, uiRoot).GetComponent<T>();
        if (uiInstance == null)
        {
            Debug.LogError($"UI 컴포넌트를 찾을 수 없습니다: {typeof(T).Name}");
            return null;
        }

        uiInstance.sortingGroup.sortingLayerName = "UIElements";

        // 정렬 순서 설정
        if (data.order == -1)
        {
            int minOrder = 9999;
            foreach (var sortingGroup in uiRoot.GetComponentsInChildren<SortingGroup>())
            {
                if (sortingGroup.sortingOrder < minOrder)
                    minOrder = sortingGroup.sortingOrder;
            }
            uiInstance.sortingGroup.sortingOrder = minOrder;
        }
        else
        {
            uiInstance.sortingGroup.sortingOrder = data.order;
        }

        // UI 초기화 및 표시
        activeUIs[data.identifier] = uiInstance;
        uiInstance.Initialization(data);
        uiInstance.Show(data);

        return uiInstance;
    }

    public void CleanupInactiveUIs()
    {
        var inactiveUIs = new List<string>();
        foreach (var kvp in activeUIs)
        {
            if (!kvp.Value.isShow)
            {
                inactiveUIs.Add(kvp.Key);
            }
        }

        foreach (var key in inactiveUIs)
        {
            if (activeUIs.TryGetValue(key, out var ui))
            {
                Destroy(ui.gameObject);
                activeUIs.Remove(key);
            }
        }
    }

    private void OnDestroy()
    {
        activeUIs.Clear();
    }
}

