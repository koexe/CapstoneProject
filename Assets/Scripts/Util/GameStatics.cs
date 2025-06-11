using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatics 
{
    public CameraController CameraController;

    public static int GetRaceCompatibility(RaceType _typeFrom, RaceType _typeTo)
    {

        int t_count = System.Enum.GetValues(typeof(RaceType)).Length;

        int t_attackerIndex = (int)_typeFrom;
        int t_defenderIndex = (int)_typeTo;

        int t_strongAgainst = (t_attackerIndex + 1) % t_count;
        int t_weakAgainst = (t_attackerIndex - 1 + t_count) % t_count;

        if (t_defenderIndex == t_strongAgainst) return 1;  // 유리
        else if (t_defenderIndex == t_weakAgainst) return -1; // 불리
        else return 0; // 중립
    }

    public static void Swap<T>(List<T> _list, int _i, int _j)
    {
        (_list[_i], _list[_j]) = (_list[_j], _list[_i]);
    }

    public static string GetStatusText(StatusEffectID _id)
    {
        switch (_id)
        {
            case StatusEffectID.Bleed:
                return "출혈";
            case StatusEffectID.Corrosion:
                return "침식";
            case StatusEffectID.Poison:
                return "독";
            case StatusEffectID.Blind:
                return "실명";
            case StatusEffectID.DEFDown:
                return "방어력 감소";
            case StatusEffectID.ATKDown:
                return "공격력 감소";
            case StatusEffectID.HealBlock:
                return "재생 차단";
            case StatusEffectID.SensoryLoss:
                return "감각 상실";
            case StatusEffectID.Decay:
                return "쇠퇴";
            case StatusEffectID.HealDisable:
                return "회복 불가";
            case StatusEffectID.FocusLoss:
                return "집중력 저하";
            case StatusEffectID.Bind:
                return "속박";
            case StatusEffectID.Confusion:
                return "혼란";
            case StatusEffectID.MentalBreak:
                return "정신 붕괴";
            case StatusEffectID.Lethargy:
                return "무기력";        
            case StatusEffectID.Split:
                return "분열";
            case StatusEffectID.SelfHarm:
                return "자해";
            case StatusEffectID.MarkOfDoom:
                return "소멸멸 표식";
            case StatusEffectID.Spended:
                return "과잉";
            case StatusEffectID.Exhaustion:
                return "과로";
            case StatusEffectID.CantAction:
                return "행동불능";
            default:
                return "Unknown";
        }
    }

    #region 한국어 조사 처리
    /// <summary>
    /// 단어의 받침 유무를 확인합니다.
    /// </summary>
    /// <param name="word">확인할 단어</param>
    /// <returns>받침이 있으면 true, 없으면 false</returns>
    public static bool HasFinalConsonant(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;
        
        char lastChar = word[word.Length - 1];
        
        // 한글 범위 확인 (가: 0xAC00, 힣: 0xD7A3)
        if (lastChar < 0xAC00 || lastChar > 0xD7A3) 
        {
            // 한글이 아닌 경우 숫자나 영어 등으로 판단
            // 숫자의 경우 받침 규칙 적용
            if (char.IsDigit(lastChar))
            {
                switch (lastChar)
                {
                    case '0': case '1': case '3': case '6': case '7': case '8':
                        return true;  // 받침 있음
                    case '2': case '4': case '5': case '9':
                        return false; // 받침 없음
                    default:
                        return false;
                }
            }
            // 영어의 경우 대부분 받침 있는 것으로 처리
            return true;
        }
        
        // 한글인 경우 받침 확인
        // (글자 - 0xAC00) % 28 == 0이면 받침 없음
        return (lastChar - 0xAC00) % 28 != 0;
    }

    /// <summary>
    /// 이/가 조사를 자동으로 선택합니다.
    /// </summary>
    /// <param name="word">주어가 될 단어</param>
    /// <returns>"이" 또는 "가"</returns>
    public static string GetSubjectParticle(string word)
    {
        return HasFinalConsonant(word) ? "이" : "가";
    }

    /// <summary>
    /// 을/를 조사를 자동으로 선택합니다.
    /// </summary>
    /// <param name="word">목적어가 될 단어</param>
    /// <returns>"을" 또는 "를"</returns>
    public static string GetObjectParticle(string word)
    {
        return HasFinalConsonant(word) ? "을" : "를";
    }

    /// <summary>
    /// 은/는 조사를 자동으로 선택합니다.
    /// </summary>
    /// <param name="word">주제가 될 단어</param>
    /// <returns>"은" 또는 "는"</returns>
    public static string GetTopicParticle(string word)
    {
        return HasFinalConsonant(word) ? "은" : "는";
    }

    /// <summary>
    /// 와/과 조사를 자동으로 선택합니다.
    /// </summary>
    /// <param name="word">단어</param>
    /// <returns>"과" 또는 "와"</returns>
    public static string GetWithParticle(string word)
    {
        return HasFinalConsonant(word) ? "과" : "와";
    }

    /// <summary>
    /// 단어에 적절한 조사를 붙여서 반환합니다.
    /// </summary>
    /// <param name="word">단어</param>
    /// <param name="particleType">조사 타입 ("이/가", "을/를", "은/는", "와/과")</param>
    /// <returns>조사가 붙은 단어</returns>
    public static string AddParticle(string word, string particleType)
    {
        string particle = particleType switch
        {
            "이/가" => GetSubjectParticle(word),
            "을/를" => GetObjectParticle(word),
            "은/는" => GetTopicParticle(word),
            "와/과" => GetWithParticle(word),
            _ => ""
        };
        
        return word + particle;
    }
    #endregion
}

public static class LogUtil
{
    public static void Log(string _message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(_message);
#endif
    }

    public static void LogWarning(string _message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning(_message);
#endif
    }

    public static void LogError(string _message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogError(_message);
#endif
    }
}