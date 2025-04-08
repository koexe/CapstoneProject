using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothMoveButtonGroup : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform content;
    [SerializeField] List<Button> buttons;
    [SerializeField] float snapSpeed = 10f;
    [SerializeField] Vector3 selectedScale = Vector3.one * 1.2f;
    [SerializeField] Vector3 normalScale = Vector3.one;

    int currentIndex = 0;
    float[] buttonPositions;

    void Start()
    {
        InitButtonPositions();
        SetupButtonListeners();
        UpdateButtonScales();
    }

    void Update()
    {
        float target = buttonPositions[currentIndex];
        float newPos = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, target, Time.deltaTime * snapSpeed);
        scrollRect.horizontalNormalizedPosition = newPos;

        UpdateButtonScales();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveToIndex(currentIndex + 1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveToIndex(currentIndex - 1);
    }

    void InitButtonPositions()
    {
        int count = buttons.Count;
        buttonPositions = new float[count];

        if (count <= 1)
        {
            buttonPositions[0] = 0f;
            return;
        }

        float step = 1f / (count - 1);
        for (int i = 0; i < count; i++)
        {
            buttonPositions[i] = step * i;
        }
    }

    void SetupButtonListeners()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i; // 지역 변수 캡쳐
            buttons[i].onClick.AddListener(() =>
            {
                if (index != currentIndex)
                {
                    MoveToIndex(index);
                }
                else
                {
                    // 현재 선택된 버튼 클릭 시 → 실행
                    buttons[index].onClick.Invoke(); // 선택 확정
                }
            });
        }
    }

    void MoveToIndex(int newIndex)
    {
        int count = buttons.Count;

        if (newIndex < 0)
            newIndex = count - 1;
        else if (newIndex >= count)
            newIndex = 0;

        if (newIndex == currentIndex) return;

        currentIndex = newIndex;
        UpdateButtonScales();
    }

    void UpdateButtonScales()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform rt = buttons[i].GetComponent<RectTransform>();
            Vector3 targetScale = (i == currentIndex) ? selectedScale : normalScale;
            rt.localScale = Vector3.Lerp(rt.localScale, targetScale, Time.deltaTime * 10f);
        }
    }

    public void SelectCurrent()
    {
        buttons[currentIndex].onClick.Invoke();
    }
}