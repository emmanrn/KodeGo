using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DIALOGUE;
using UnityEngine;

namespace TESTING
{
    public class TestParser : MonoBehaviour
    {
        void Start()
        {

            SendFileToParse();

        }

        private void SendFileToParse()
        {
            List<string> lines = FileManager.ReadTxtAsset("testFile");

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] == string.Empty)
                    continue;

                DIALOGUE_LINES dl = DialogueParser.Parse(lines[i]);
            }
        }

    }
}
