using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private GameObject interactObj;
    private bool hasTlked;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private TextAsset fileToRead = null;

    void Start()
    {
        interactObj = gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger && !hasTlked)
        {
            StartCoroutine(StartConversation());
        }
    }

    private IEnumerator StartConversation()
    {
        // set input action map to the general ui where the next prompt button is at
        inputReader.SetGeneral();

        List<string> lines = FileManager.ReadTxtAsset(fileToRead);
        yield return DialogueSystem.instance.Say(lines);

        // once the dialogue is done set the player controls back
        inputReader.SetPlayerMovement();
        hasTlked = true;

    }



}
