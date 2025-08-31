using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{

    public class CommandTesting : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            yield return CommandManager.instance.Execute("greet");
            yield return CommandManager.instance.Execute("lambda");
            yield return CommandManager.instance.Execute("lambda_p1", "Hello World");
            yield return CommandManager.instance.Execute("lambda_p2", "Hello World", "What?", "Bruh");

            yield return CommandManager.instance.Execute("process");
            yield return CommandManager.instance.Execute("process1", "10");

        }

    }
}
