using UnityEngine;

[CreateAssetMenu(fileName = "DebugTerminalConfig", menuName = "Terminals/Debug Terminal Config")]
public class DebugTerminalConfig : CodeTerminalConfig
{
    [Tooltip("Code blocks to fill each {input} slot in order.")]
    public string[] prefilledBlocks;
}
