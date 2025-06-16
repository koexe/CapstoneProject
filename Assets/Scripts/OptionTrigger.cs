using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionTrigger : MonoBehaviour
{
    public void OnClickOptionButton()
    {
        if (GameManager.instance.GetGameState() == GameState.Pause)
        {
            return;
        }
        else
        {
            UIManager.instance.ShowUI<OptionUI>(new UIData()
            {
                identifier = "OptionUI",
            });
        }

    }
}
