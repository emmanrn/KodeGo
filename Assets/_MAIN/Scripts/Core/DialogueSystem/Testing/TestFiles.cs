using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TestFiles : MonoBehaviour
    {
        [SerializeField] private TextAsset fileName;

        void Start()
        {
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            List<string> lines = FileManager.ReadTxtAsset(fileName, false);

            for (int i = 0; i < lines.Count; i++)
            {
                Debug.Log(lines[i]);
            }
            yield return null;
        }
    }

}
