using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{

    public class DL_SPEAKER_DATA
    {
        public string name, castName;
        //this will basically determine which name to show if cast name is empty or not
        public string displayName => isCastingName ? castName : name;
        public Vector2 castPosition;
        public List<(int layer, string expression)> CastExpressions { get; set; }
        public bool isCastingName => castName != string.Empty;
        public bool isCastingExpressions => CastExpressions.Count > 0;

        public bool makeCharEnter = false;
        private const string NAMECAST_ID = " as ";
        private const string POSCAST_ID = " at ";
        private const string EXPRESSION_CAST_ID = " [";
        private const char AXIS_DELIMITER = ':';
        private const char EXPRESSIONLAYER_JOINER = ',';
        private const char EXPRESSIONLAYER_DELIMITER = ':';
        private const string ENTER_KEYWORD = "enter ";

        private string ProcessKeywords(string rawSpeaker)
        {
            if (rawSpeaker.StartsWith(ENTER_KEYWORD))
            {
                rawSpeaker = rawSpeaker.Substring(ENTER_KEYWORD.Length);
                makeCharEnter = true;
            }

            return rawSpeaker;
        }
        public DL_SPEAKER_DATA(string rawSpeaker)
        {
            rawSpeaker = ProcessKeywords(rawSpeaker);
            string pattern = @$"{NAMECAST_ID}|{POSCAST_ID}|{EXPRESSION_CAST_ID.Insert(EXPRESSION_CAST_ID.Length - 1, @"\")}";
            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);
            castName = "";
            castPosition = Vector2.zero;
            CastExpressions = new List<(int layer, string expression)>();

            if (matches.Count == 0)
            {
                name = rawSpeaker;
                return;
            }

            int firstMatchIdx = matches[0].Index;

            name = rawSpeaker.Substring(0, firstMatchIdx);

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIdx = 0, endIdx = 0;

                if (match.Value == NAMECAST_ID)
                {
                    startIdx = match.Index + NAMECAST_ID.Length;
                    endIdx = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIdx, endIdx - startIdx);
                }
                else if (match.Value == POSCAST_ID)
                {
                    startIdx = match.Index + POSCAST_ID.Length;
                    endIdx = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIdx, endIdx - startIdx);

                    string[] axis = castPos.Split(AXIS_DELIMITER, System.StringSplitOptions.RemoveEmptyEntries);

                    // if x is the only specified pos
                    float.TryParse(axis[0], out castPosition.x);

                    // if y is also specified
                    if (axis.Length > 1)
                        float.TryParse(axis[1], out castPosition.y);
                }
                else if (match.Value == EXPRESSION_CAST_ID)
                {
                    startIdx = match.Index + EXPRESSION_CAST_ID.Length;
                    endIdx = (i < matches.Count - 1) ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIdx, endIdx - (startIdx + 1));

                    CastExpressions = castExp.Split(EXPRESSIONLAYER_JOINER).Select(x =>
                    {
                        var parts = x.Trim().Split(EXPRESSIONLAYER_DELIMITER);
                        // this allows if we want to just specify the sprite expression or for my case just getting the sprite from the spritesheet
                        // without specifying a layer in the dialogue file
                        if (parts.Length == 2)
                            return (int.Parse(parts[0]), parts[1]);
                        else
                            return (0, parts[0]);


                    }).ToList();

                }
            }
        }
    }
}