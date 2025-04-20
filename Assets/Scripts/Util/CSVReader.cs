using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            (string, int, string)[] t_dialogs = new (string, int, string)[t_dialogData.Length];

            for (int j = 0; j < t_dialogData.Length; j++)
            {
                var t_dialogDetail = Regex.Split(t_dialogData[j], "\\*");
                if (t_dialogDetail.Length == 1)
                {
                    t_dialogs[j] = (null, 0, t_dialogDetail[0]);
                }
                else
                {
                    t_dialogs[j] = (t_dialogDetail[0], int.Parse(t_dialogDetail[1]), t_dialogDetail[2]);
                }
            }
            t_data.dialogs = t_dialogs;


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
                Debug.LogWarning($"중복된 대사 index: {key}, 건너뜀");
            }
        }
        return list;
    }
}



