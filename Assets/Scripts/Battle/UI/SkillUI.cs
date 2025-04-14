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

    public void Initialization(BattleCharacterBase _character, SkillBase _skill, Action<BattleCharacterBase, SkillBase> _action)
    {
        this.skillName.text = _skill.skillName;
        this.skillIcon.sprite = _skill.skillIcon;
        this.skillButton.onClick.AddListener(() => _action.Invoke(_character, _skill));

    }
}
