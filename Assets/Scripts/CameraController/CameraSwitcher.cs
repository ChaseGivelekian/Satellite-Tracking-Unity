using UnityEngine;

namespace CameraController
{
    public class CameraSwitcher : MonoBehaviour
    {
        [Header("Camera Setup")]
        [SerializeField] private Camera camera1;
        [SerializeField] private Camera camera2;

        private void Start()
        {
            camera1.enabled = true;
            camera2.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                camera1.enabled = true;
                camera2.enabled = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                camera1.enabled = false;
                camera2.enabled = true;
            }
        }
    }
}
