using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Base", menuName = "Monster/Monster_Base")]
public class SOMonsterBase : ScriptableObject
{
    public int monsterIndentifier;
    public SOSkillBase[] skills;
    //스파인 데이터 추가?
}
