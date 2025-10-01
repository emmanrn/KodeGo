using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace TESTING
{

    public class TestingArchi : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        string[] lines = new string[5]
        {
            "This is my random dialogue",
            "RAWWRRRRR I'm Phyrewall!",
            "Bruh why are you screaming like that?",
            "I dunno whats the matter with you?",
            "Ok cmon just do what we planned from the start",
        };
        string longLine = "hello, dont worry about this as this is just a very long line for exampling but, hey I like long things don't ya think as well? What is your favorite long thing eh????";

        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueTxt);
            architect.speed = 0.5f;
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.isBuilding)
                {
                    if (!architect.hurryUp)
                    {
                        architect.hurryUp = true;
                    }
                    else
                    {
                        // force complete
                        architect.ForceComplete();
                    }
                }
                else
                {
                    architect.Build(longLine);
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(longLine);
            }
        }

    }

}
