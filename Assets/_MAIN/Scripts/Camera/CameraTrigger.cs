using Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Animator anim;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.gameObject.CompareTag("Player"))
        //     if (CameraManager.ActiveCamera != cam)
        //     {
        //         CameraManager.SwitchCamera(cam);
        //         anim.Play("Enter");
        //     }
        if (other.gameObject.CompareTag("Player"))
        {
            if (!CameraManager.isActiveCamera(cam))
            {
                CameraManager.SwitchCamera(cam);

                if (CameraManager.LastCamera != cam)
                {
                    AudioManager.instance.PlaySoundEffect(FilePaths.GetPathToResource(FilePaths.resources_sfx, "camera_switch"));
                    anim.SetTrigger("EnterTrigger");
                }
            }
        }
    }





}
