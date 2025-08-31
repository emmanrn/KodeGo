using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{

    public class CMD_DB_EXTENSION_EXAMPLES : CMD_DB_EXTENSION
    {
        new public static void Extend(CommandDatabase database)
        {
            // add action with no params
            database.AddCommand("greet", new Action(Greet));

            //add lambda with no params
            // can be useful for small functions or one liners
            database.AddCommand("lambda", new Action(() => { Debug.Log("Print using lmda epression"); }));
            database.AddCommand("lambda_p1", new Action<string>((arg) => { Debug.Log($"Print using lmda epression with arg {arg}"); }));
            database.AddCommand("lambda_p2", new Action<string[]>((args) => { Debug.Log(string.Join(", ", args)); }));

            //add coroutine with no params
            database.AddCommand("process", new Func<IEnumerator>(SimpleProcess));
            database.AddCommand("process1", new Func<string, IEnumerator>(ProcessParams));

        }
        private static void Greet()
        {
            Debug.Log("Hello World");
        }

        private static IEnumerator SimpleProcess()
        {
            for (int i = 0; i < 5; i++)
            {
                Debug.Log($"Counting {i}");
                yield return new WaitForSeconds(1f);
            }
        }
        private static IEnumerator ProcessParams(string data)
        {
            if (int.TryParse(data, out int num))
            {
                for (int i = 0; i < num; i++)
                {
                    Debug.Log($"Counting {i}");
                    yield return new WaitForSeconds(1f);
                }

            }
        }
    }
}