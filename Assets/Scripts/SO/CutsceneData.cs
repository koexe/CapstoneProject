using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CusceneData", menuName = "CusceneData")]
public class CutsceneData : ScriptableObject
{
    public AudioClip cutsceneBGM;
    public string id;  
    public List<CutsceneStep> steps;
}
 