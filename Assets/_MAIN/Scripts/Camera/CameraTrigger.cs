using Cinemachine;
using MAIN_GAME;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Animator anim;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!CameraManager.isActiveCamera(cam))
            {
                CameraManager.SwitchCamera(cam);

                string roomID = cam.name;
                LevelData level = LevelProgressManager.GetLevel(GameManager.instance.LEVEL_NAME);

                if (!level.visitedRooms.Contains(roomID))
                {
                    level.visitedRooms.Add(roomID);

                    int totalRooms = LevelProgressManager.levels.GetLevel(GameManager.instance.LEVEL_NAME).totalRooms;
                    float exploredPercent = (float)level.visitedRooms.Count / totalRooms;

                    LevelProgressManager.SetExplorationPercent(GameManager.instance.LEVEL_NAME, exploredPercent);
                }

                if (CameraManager.LastCamera != cam)
                {
                    AudioManager.instance.PlaySoundEffect(FilePaths.GetPathToResource(FilePaths.resources_sfx, "camera_switch"));
                    anim.SetTrigger("EnterTrigger");
                }
            }
        }
    }





}
