using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PracticeTerminalConfig", menuName = "Terminals/Practice Terminal Config")]
public class PracticeTermConfig : CodeTerminalConfig
{
    [Tooltip("Optional hints or tips to help the player solve the challenge.")]
    [TextArea(1, 10)]
    public string[] hints;
}

