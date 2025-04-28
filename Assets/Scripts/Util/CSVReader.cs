using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static Dictionary<int, BattleConversationData> ReadConversationTable(TextAsset _asset)
    {
        var list = new Dictionary<int, BattleConversationData>();

        var lines = Regex.Split(_asset.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;
            BattleConversationData t_data = new BattleConversationData();
            t_data.index = int.Parse(values[1]);
            t_data.dialog = values[2];
            t_data.choices = Regex.Split(values[3], "/");
            t_data.result = Regex.Split(values[4], "/");
            list.Add(int.Parse(values[0]), t_data);
        }
        return list;
    }

    public static Dictionary<int, DialogData> ReadDialogData(TextAsset _asset)
    {
        var list = new Dictionary<int, DialogData>();

        var lines = Regex.Split(_asset.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            DialogData t_data = new DialogData();
            t_data.index = int.Parse(values[0]);
            var t_dialogData = Regex.Split(values[1], "/");
            string talkingCharacter = string.Empty;
            (string, string)[] t_dialogs = new (string, string)[t_dialogData.Length];
            (string, int)[][] t_characters = new (string, int)[t_dialogData.Length][];

            for (int j = 0; j < t_dialogData.Length; j++)
            {
                var t_dialogDetail = Regex.Split(t_dialogData[j], "\\*");

                if (t_dialogDetail.Length == 1)
                {
                    t_characters[j] = null;
                }
                else
                {
                    var t_temp = Regex.Split(t_dialogDetail[0], "\\#");

                    var t_characterDetail = Regex.Split(t_temp[0], "\\,");
                    t_characters[j] = new (string, int)[t_characterDetail.Length];

                    for (int t_index = 0; t_index < t_characterDetail.Length; t_index++)
                    {
                        var t_character = Regex.Split(t_characterDetail[t_index].Trim().Trim('"'), "_");
                        t_characters[j][t_index] = (t_character[0].Trim(), int.Parse(t_character[1]));
                    }
                    talkingCharacter = t_temp[1];

                }
                t_dialogs[j] = (talkingCharacter, t_dialogDetail[t_dialogDetail.Length - 1]);
            }
            t_data.dialogs = t_dialogs;
            t_data.characters = t_characters;

            if (values[2] == "NONE")
            {
                t_data.condition = (-1, false, null, -1);
            }
            else
            {
                var t_conditionData = Regex.Split(values[2], "/");
                t_data.condition = (int.Parse(t_conditionData[0]), bool.Parse(t_conditionData[1]), t_conditionData[2], int.Parse(t_conditionData[3]));
            }

            if (values[3] == "NONE")
            {
                t_data.choices = null;
            }
            else
            {
                var t_choicesData = Regex.Split(values[3], "/");
                (string, int)[] t_choices = new (string, int)[t_choicesData.Length];

                for (int j = 0; j < t_choices.Length; j++)
                {
                    var t_choceDetail = Regex.Split(t_choicesData[j], "\\*");
                    t_choices[j] = (t_choceDetail[0], int.Parse(t_choceDetail[1]));
                }
                t_data.choices = t_choices;
            }

            int key = int.Parse(values[0]);
            if (!list.ContainsKey(key))
            {
                list.Add(key, t_data);
            }
            else
            {
                Debug.LogWarning($"�ߺ��� ��� index: {key}, �ǳʶ�");
            }
        }
        return list;
    }


    public static Dictionary<StatusEffectID, StatusEffectInfo> ReadEffectData(TextAsset _asset)
    {
        Dictionary<StatusEffectID, StatusEffectInfo> effectDict = new Dictionary<StatusEffectID, StatusEffectInfo>();
        var lines = Regex.Split(_asset.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
            return effectDict;

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length < 7)
                continue;

            var effect = new StatusEffectInfo();
            effect.id = ParseEffectID(values[0]);
            effect.category = ParseCategory(values[1]);
            effect.description = values[2].Trim();
            effect.duration = ParseInt(values[3]);
            effect.activationChance = float.Parse(values[4]);
            effect.maxStack = ParseStack(values[5]);
            effect.isStackable = effect.maxStack != -1 ? true : false;

            if (!effectDict.ContainsKey(effect.id))
                effectDict.Add(effect.id, effect);
        }

        return effectDict;
    }
    static StatusEffectID ParseEffectID(string raw)
    {
        try
        {
            // 공백 제거하고 PascalCase로 정리
            string cleaned = raw.Trim().Replace(" ", "");
            return (StatusEffectID)System.Enum.Parse(typeof(StatusEffectID), cleaned, true);
        }
        catch
        {
            Debug.LogWarning($"Unknown StatusEffectID: {raw}");
            return StatusEffectID.Bleed; // 기본값
        }
    }
    static StatusCategory ParseCategory(string raw)
    {
        raw = raw.Trim();
        return raw switch
        {
            "Debuff" => StatusCategory.Debuff,
            "Restriction" => StatusCategory.Restriction,
            "SpecialEffect" => StatusCategory.SpecialEffect,
            _ => StatusCategory.Debuff
        };
    }

    static int ParseStack(string raw)
    {
        return raw.Trim() == "-" ? -1 : ParseInt(raw);
    }

    static bool ParseBool(string raw)
    {
        return raw.Trim().Equals("TRUE", System.StringComparison.OrdinalIgnoreCase);
    }

    static int ParseInt(string raw)
    {
        return int.TryParse(raw.Trim(), out int val) ? val : 0;
    }
}



