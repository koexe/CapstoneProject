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
}



