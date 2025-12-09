using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static List<CinemachineCamera> cameras = new();

    public static CinemachineCamera activeCamera = null;

    public static bool IsCameraActive(CinemachineCamera cam)
    {
        return activeCamera == cam;
    }

    public static void SwitchCamera(CinemachineCamera newCam)
    {
        newCam.Priority = 10;
        activeCamera = newCam;

        foreach (CinemachineCamera cam in cameras)
        {
            if (cam != newCam)
            {
                cam.Priority = 0;
            }
        }
    }

    public static void RegisterCamera(CinemachineCamera cam)
    {
        if (!cameras.Contains(cam))
        {
            cameras.Add(cam);
        }
    }

    public static void UnregisterCamera(CinemachineCamera cam)
    {
        if (cameras.Contains(cam))
        {
            cameras.Remove(cam);
        }
    }
}
