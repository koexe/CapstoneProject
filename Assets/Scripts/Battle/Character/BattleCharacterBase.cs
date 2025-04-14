using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterBase : MonoBehaviour
{
    int hp;
    public int speed;
    [SerializeField] BattleManager battleManager;
    [SerializeField] SpriteRenderer skin;
    [SerializeField] Animator animator;
    [SerializeField] SkillBase[] skills;
    [SerializeField] SkillBase selectedSkill;

    [SerializeField] bool isActionDone = false;

    //public void SetActionDone(bool _actionDone) => this.isActionDone = _actionDone;

    public void SetActionDone() => this.isActionDone = true;

    public bool IsActionDone() => this.isActionDone;
    public SkillBase[] GetSkills() => this.skills;
    public void Initialization(BattleManager _battleManager)
    {
        this.battleManager = _battleManager;
        return;
    }
    public void SetSelectedSkill(SkillBase _skill)
    {
        this.selectedSkill = Instantiate(_skill);
    }
    public SkillBase GetSelectedSkill()
    {
        return this.selectedSkill;
    }

    public void StartAction()
    {
        this.selectedSkill.Execute(this);
        Invoke("SetActionDone", 3.0f);
    }
    

}
