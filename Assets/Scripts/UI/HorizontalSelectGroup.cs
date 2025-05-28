using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System;

public class HorizontalSelectGroup : MonoBehaviour
{
    [SerializeField] List<SelectableButton> buttons;
    [SerializeField] List<RectTransform> points;
    [SerializeField] float spacing;
    [SerializeField] RectTransform middlePoint;

    [SerializeField] SelectableButton selectedButton;

    [SerializeField] float moveSmoothTime;

    Coroutine moveCoroutine;

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].transform.localPosition = new Vector3(i * spacing, 0, 0);
        }
    }

    public void OnButtonClick(int index)
    {
        if (moveCoroutine != null) return;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i == index)
            {
                if (selectedButton == buttons[i])
                {
                    buttons[i].onClick.Invoke();
                }
                else
                {
                    buttons[i].GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    selectedButton = buttons[i];
                }
            }
            else
            {
                buttons[i].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
        }
    }

    IEnumerator MoveButtons()
    {
        float t_timer = 0f;
        while(t_timer != moveSmoothTime)
        {
            t_timer += Time.deltaTime;
            yield return CoroutineUtil.WaitForFixedUpdate;
            float t_progress = t_timer / moveSmoothTime;
            foreach(var button in buttons)
            {

                var targetX = points[button.currentIndex].localPosition.x;
                button.transform.localPosition = Vector3.Lerp(button.transform.localPosition, new Vector3(targetX, 0, 0), t_progress);
            }
        }
    }

}

[Serializable]
public class SelectableButton : MonoBehaviour
{
    public Button button;
    public UnityEvent onClick;
    public int currentIndex;
}