using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour, IInteractable
{

    [SerializeField] private InputReader inputReader;
    [SerializeField] private GameObject rootTerminal;
    // private GameStateManager gameState;

    void Awake()
    {
        // gameState = GameStateManager.instance;
    }

    public bool isInteractable() => true;
    public void Interact()
    {
        Debug.Log("npc");
        // if (gameState.isPaused && !gameState.isRunningDialogue)
        //     return;

        rootTerminal.SetActive(true);
    }

    public void CloseWindow()
    {
        rootTerminal.SetActive(false);
    }


}
