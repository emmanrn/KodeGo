using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private GameObject interactObj;
    [SerializeField] private TextAsset fileToRead = null;

    void Start()
    {
        interactObj = gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            Debug.Log("Start Dialogue");
            StartConversation();

        }
    }

    private void StartConversation()
    {
        PlayerInputManager.instance.EnableGeneral(GeneralActionMap.DIALOGUE);
        List<string> lines = FileManager.ReadTxtAsset(fileToRead);
        DialogueSystem.instance.Say(lines);


    }


}
