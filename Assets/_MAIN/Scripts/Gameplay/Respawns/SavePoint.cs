using System.Collections;
using System.Collections.Generic;
using MAIN_GAME;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private string levelName => GameManager.instance.LEVEL_NAME;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var level = LevelProgressManager.GetLevel(levelName);

            level.checkpoint = transform.position;
            level.hasCheckpoint = true;

            GameSave.activeFile.Save();
            Debug.Log("save point reached");
        }
    }
}
