using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodeLine : MonoBehaviour
{
    private InputLine[] textMeshPro;
    public List<InputLine> blankLines;


    void Start()
    {
        textMeshPro = GetComponentsInChildren<InputLine>();

        for (int i = 0; i < textMeshPro.Length; i++)
            blankLines.Add(textMeshPro[i]);

    }

}
