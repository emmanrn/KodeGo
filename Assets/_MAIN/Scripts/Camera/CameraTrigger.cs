using Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
            if (CameraManager.ActiveCamera != cam)
            {
                CameraManager.SwitchCamera(cam);
            }
    }





}
