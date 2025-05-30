using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] Image skillIcon;
    [SerializeField] Button skillButton;

    public void Initialization(BattleCharacterBase _character, SOSkillBase _skill, Action<BattleCharacterBase, SOSkillBase> _action)
    {
        this.skillName.text = _skill.skillName;
        this.skillIcon.sprite = _skill.skillIcon;
        this.skillButton.onClick.RemoveAllListeners();
        if (_character.GetMp() >= _skill.requireMp)
        {
            this.skillButton.interactable = true;
        }
        else
        {
            this.skillButton.interactable = false;
        }
        this.skillButton.interactable = false;
        this.skillButton.onClick.AddListener(() => _action.Invoke(_character, _skill));

    }
}
