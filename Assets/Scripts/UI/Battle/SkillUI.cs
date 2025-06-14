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
    [SerializeField] TextMeshProUGUI skillMp;
    [SerializeField] TextMeshProUGUI skillRace;
    [SerializeField] TextMeshProUGUI skillRange;

    public void Initialization(BattleCharacterBase _character, SOSkillBase _skill, Action<BattleCharacterBase, SOSkillBase> _action)
    {
        this.skillName.text = _skill.skillName;
        this.skillIcon.sprite = _skill.skillIcon;
        this.skillButton.onClick.RemoveAllListeners();

        this.skillMp.text = $"Mp:{_skill.requireMp}";
        this.skillRace.text = GameStatics.GetRaceText(_skill.attackRaceType);
        if(_skill.attackRange == SOSkillBase.AttackRangeType.All)
        {
            this.skillRange.text = "전체";
        }
        else if (_skill.attackRange == SOSkillBase.AttackRangeType.Ally)
        {
            this.skillRange.text = "아군";
        }
        else
        {
            this.skillRange.text = $"단일";
        }


        if (_character.GetMp() >= _skill.requireMp)
        {
            this.skillButton.interactable = true;
        }
        else
        {
            this.skillButton.interactable = false;
        }
        this.skillButton.onClick.AddListener(() => _action.Invoke(_character, _skill));

    }
}
