using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DIALOGUE
{

    public class DL_DIALOGUE_DATA
    {
        public string rawData { get; private set; } = string.Empty;
        public List<DIALOGUE_SEGMENT> segments;

        // regex pattern
        // this is going to look for the signals c, a, wc, wa
        // [ca] means it can either be = c or an a
        // then | char for 'or'
        // then either if it found a 'w' then it can have an optional 'c' or 'a' after it
        // \s - for space, \d* - for a digit of any length, \.? - for decimals which can be optional, then do the digit thing again
        private const string segmentIdentifierPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";
        public DL_DIALOGUE_DATA(string rawDialogue)
        {
            this.rawData = rawDialogue;
            segments = RipSegments(rawDialogue);
        }

        public List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
        {
            List<DIALOGUE_SEGMENT> segments = new List<DIALOGUE_SEGMENT>();
            MatchCollection matches = Regex.Matches(rawDialogue, segmentIdentifierPattern);

            int lastIndex = 0;
            // finding the first or the only segment in the file
            DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();
            segment.dialogue = (matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index));
            segment.startSignal = DIALOGUE_SEGMENT.StartSignal.NONE;
            segment.signalDelay = 0;
            segments.Add(segment);

            if (matches.Count == 0)
                return segments;
            else
                lastIndex = matches[0].Index;

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                segment = new DIALOGUE_SEGMENT();

                // get start signal for the segment
                string signalMatch = match.Value;
                signalMatch = signalMatch.Substring(1, match.Length - 2);
                string[] signalSplit = signalMatch.Split(' ');

                segment.startSignal = (DIALOGUE_SEGMENT.StartSignal)Enum.Parse(typeof(DIALOGUE_SEGMENT.StartSignal), signalSplit[0].ToUpper());

                // get the signal delay
                if (signalSplit.Length > 1)
                    float.TryParse(signalSplit[1], out segment.signalDelay);

                // get the dialogue for the segment
                int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length;
                segment.dialogue = rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
                lastIndex = nextIndex;

                segments.Add(segment);
            }
            return segments;
        }

        // the structure goes like this
        // this is a dialogue {} line 2 {} line 4 dialogue
        // the ones that are gonna be inside the brackets are the startSignals, 
        // c - clear
        // a - append
        // wa - wait and append
        // wc - wait and clear
        public struct DIALOGUE_SEGMENT
        {
            public string dialogue;
            public StartSignal startSignal;
            public float signalDelay;
            public enum StartSignal { NONE, C, A, WA, WC }
            public bool appendText => (startSignal == StartSignal.A || startSignal == StartSignal.WA);
        }
    }

}