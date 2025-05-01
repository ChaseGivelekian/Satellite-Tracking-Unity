using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CameraController
{
    public class CameraSwitcher : MonoBehaviour
    {
        [Header("Camera Setup")]
        [SerializeField] private List<Camera> cameras = new();

        private void Start()
        {
            // Disable all cameras except the first one
            if (cameras.Count > 0)
            {
                for (var i = 0; i < cameras.Count; i++)
                {
                    if (cameras[i] != null)
                    {
                        cameras[i].enabled = i == 0;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No cameras assigned to CameraSwitcher.");
            }
        }

        private void Update()
        {
            for (var i = 0; i < cameras.Count && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) && cameras[i])
                {
                    SwitchToCamera(i);
                }
            }
        }

        private void SwitchToCamera(int index)
        {
            // Disable all cameras
            foreach (var t in cameras.Where(t => t))
            {
                t.enabled = false;
            }

            // Enable the selected camera
            if (index >= 0 && index < cameras.Count && cameras[index])
            {
                cameras[index].enabled = true;
            }
        }
    }
}