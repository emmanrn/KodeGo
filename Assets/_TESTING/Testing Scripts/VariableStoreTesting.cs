using Unity.VisualScripting;
using UnityEngine;

namespace TESTING
{
    public class VariableStoreTesting : MonoBehaviour
    {
        public int var_int = 0;
        public float var_float = 0f;
        public bool var_bool = false;
        public string var_str = "";
        void Start()
        {
            // linking the database to those public vars
            VariableStore.CreateDatabase("DB_Links");

            VariableStore.CreateVariable("DB_Links.L_int", var_int, () => var_int, value => var_int = value);
            VariableStore.CreateVariable("DB_Links.L_bool", var_bool, () => var_bool, value => var_bool = value);

            VariableStore.CreateDatabase("DB_Numbers");
            VariableStore.CreateDatabase("DB_Bools");
            VariableStore.CreateDatabase("DB_3");

            VariableStore.CreateVariable("DB_Numbers.num1", 1);
            VariableStore.CreateVariable("DB_Numbers.num5", 5);
            VariableStore.CreateVariable("DB_Bools.isLightOn", true);
            VariableStore.CreateVariable("DB_Bools.isCorretAnswer", false);
            VariableStore.CreateVariable("DB_Numbers.float1", 7.5f);
            VariableStore.CreateVariable("sampleString", "Hello World");


            VariableStore.PrintAllDatabases();

            VariableStore.PrintAllVariables();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                VariableStore.PrintAllVariables();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                VariableStore.TryGetValue("DB_Links.L_int", out object linked_int);
                VariableStore.TrySetValue("DB_Links.L_int", (int)linked_int + 5);

            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                VariableStore.TryGetValue("DB_Links.L_bool", out object linked_bool);
                VariableStore.TrySetValue("DB_Links.L_bool", !(bool)linked_bool);
            }
        }
    }

}

