using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class CameraManager
{
    private static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    public static CinemachineVirtualCamera ActiveCamera = null;
    public static CinemachineVirtualCamera LastCamera = null;

    public static bool isActiveCamera(CinemachineVirtualCamera camera) => camera == ActiveCamera;

    public static void SwitchCamera(CinemachineVirtualCamera newCamera)
    {
        if (ActiveCamera != null)
            LastCamera = ActiveCamera;
        newCamera.Priority = 10;
        ActiveCamera = newCamera;

        for (int i = 0; i < cameras.Count; i++)
        {
            CinemachineVirtualCamera camera = cameras[i];

            if (camera != newCamera && camera.Priority != 0)
                camera.Priority = 0;
        }
    }

    public static void Register(CinemachineVirtualCamera camera) => cameras.Add(camera);
    public static void Unregister(CinemachineVirtualCamera camera) => cameras.Remove(camera);
}
