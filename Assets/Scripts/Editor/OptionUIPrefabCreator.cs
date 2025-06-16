using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class OptionUIPrefabCreator : EditorWindow
{
    [MenuItem("Tools/Create Option UI")]
    public static void CreateOptionUI()
    {
        // 메인 캔버스 생성
        GameObject canvasObj = new GameObject("OptionCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 옵션 패널 생성
        GameObject panelObj = new GameObject("OptionPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.2f, 0.2f);
        panelRect.anchorMax = new Vector2(0.8f, 0.8f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 제목 텍스트
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "옵션";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        // BGM 슬라이더
        CreateSlider("BGMSlider", "BGM 볼륨", panelObj.transform, new Vector2(0, 0.6f));

        // SE 슬라이더
        CreateSlider("SESlider", "효과음 볼륨", panelObj.transform, new Vector2(0, 0.4f));

        // 해상도 드롭다운
        GameObject resolutionObj = new GameObject("ResolutionDropdown");
        resolutionObj.transform.SetParent(panelObj.transform, false);
        TMP_Dropdown resolutionDropdown = resolutionObj.AddComponent<TMP_Dropdown>();
        RectTransform dropdownRect = resolutionObj.GetComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(0.2f, 0.2f);
        dropdownRect.anchorMax = new Vector2(0.8f, 0.3f);
        dropdownRect.offsetMin = Vector2.zero;
        dropdownRect.offsetMax = Vector2.zero;

        // OptionUI 컴포넌트 추가
        OptionUI optionUI = panelObj.AddComponent<OptionUI>();
        optionUI.contents = panelObj;
        optionUI.sortingGroup = canvasObj.AddComponent<SortingGroup>();

        // 프리팹으로 저장
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        PrefabUtility.SaveAsPrefabAsset(panelObj, "Assets/Prefabs/OptionUI.prefab");
        DestroyImmediate(canvasObj);
    }

    private static void CreateSlider(string name, string label, Transform parent, Vector2 anchorPosition)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);
        
        // 슬라이더 생성
        Slider slider = sliderObj.AddComponent<Slider>();
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.2f, anchorPosition.y);
        sliderRect.anchorMax = new Vector2(0.8f, anchorPosition.y + 0.1f);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        // 배경 생성
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObj.transform, false);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = new Vector2(0, -2);
        backgroundRect.offsetMax = new Vector2(0, 2);

        // Fill Area 생성
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(5, 0);
        fillAreaRect.offsetMax = new Vector2(-5, 0);

        // Fill 생성
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.6f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        // Handle 생성
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(sliderObj.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0, 0);
        handleRect.anchorMax = new Vector2(0, 1);
        handleRect.sizeDelta = new Vector2(20, 0);

        // 슬라이더 설정
        slider.targetGraphic = handleImage;
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.direction = Slider.Direction.LeftToRight;

        // 라벨 생성
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(sliderObj.transform, false);
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 24;
        labelText.alignment = TextAlignmentOptions.Left;
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0.2f, 0.5f);
        labelRect.offsetMin = new Vector2(-100, -15);
        labelRect.offsetMax = new Vector2(0, 15);
    }
} 