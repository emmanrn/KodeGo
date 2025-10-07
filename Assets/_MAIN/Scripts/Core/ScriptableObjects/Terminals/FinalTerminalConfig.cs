using UnityEngine;

[CreateAssetMenu(fileName = "FinalTerminalConfig", menuName = "Terminals/Final Terminal Config")]
public class FinalTerminalConfig : CodeTerminalConfig
{
    [Header("Available Code Blocks")]
    [Tooltip("All code blocks the player can drag into slots.")]
    public string[] codeBlocks;
}
