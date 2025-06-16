using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : UIBase
{
    public override void Initialization(UIData data)
    {

    }
    public override void Show(UIData data)
    {
        this.contents.SetActive(true);
        this.isShow = true;
    }
    public override void Hide()
    {
        this.contents.SetActive(false);
        this.isShow = false;
    }


    public void OnClickLoadButton()
    {
        UIManager.instance.ShowUI<SaveUI>(new SaveUIData()
        {
            identifier = "SaveUI",
            isAllowMultifle = false,
            isLoadOnly = true
        });
    }

    public void OnClickMainButton()
    {
        GameManager.instance.GameOver();
    }


}

public class GameOverUIData : UIData
{
}