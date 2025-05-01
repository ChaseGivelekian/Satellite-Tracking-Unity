using UnityEngine;

namespace CameraController
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target")] public Transform objectTransform;

        [Header("Orbit Settings")] public float orbitSpeed = 100f;

        [Header("Initial Camera Setup")] public float initialDistance;
        public float minDistance;
        public float maxDistance;
        public float zoomSpeed;

        // Private variables
        private Vector3 _offset;
        private float _currentDistance;
        private bool _isOrbiting;
        private Vector2 _lastMousePosition;

        // Camera pivot to handle rotations
        private GameObject _cameraPivot;
        private float _horizontalAngle;
        private float _verticalAngle;

        private void Start()
        {
            if (objectTransform == null)
            {
                Debug.LogWarning("Transform not assigned to Camera Controller!");
            }

            // Create a pivot object for the camera
            _cameraPivot = new GameObject("CameraPivot")
            {
                transform =
                {
                    position = objectTransform.position
                }
            };

            // Set initial distance and position
            _currentDistance = initialDistance;

            // Initialize camera position
            UpdateCameraPosition();

            // Calculate the initial angles from the camera's current orientation
            CalculateInitialAngles();
        }

        private void CalculateInitialAngles()
        {
            // Calculate direction vector from the target's center to the camera
            var direction = (transform.position - objectTransform.position).normalized;

            // Calculate initial horizontal angle (around Y axis)
            _horizontalAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            // Calculate initial vertical angle (around X axis)
            var directionYFlat = new Vector3(direction.x, 0, direction.z).magnitude;
            _verticalAngle = Mathf.Atan2(direction.y, directionYFlat) * Mathf.Rad2Deg;
        }

        private void Update()
        {
            if (!objectTransform)
                return;

            // Check for the mouse button down
            if (Input.GetMouseButtonDown(0))
            {
                _isOrbiting = true;
                _lastMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            // Check for mouse button-up
            if (Input.GetMouseButtonUp(0))
            {
                _isOrbiting = false;
            }

            // Make sure the pivot stays at the object's position
            _cameraPivot.transform.position = objectTransform.position;

            // Handle orbiting while the mouse is held down
            if (_isOrbiting)
            {
                var currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                var mouseDelta = currentMousePosition - _lastMousePosition;

                // Update the rotation angles
                _horizontalAngle += mouseDelta.x * orbitSpeed * Time.deltaTime;
                _verticalAngle = Mathf.Clamp(_verticalAngle - mouseDelta.y * orbitSpeed * Time.deltaTime, -89.5f,
                    89.5f);

                // Apply the rotations - important to do this in the right order
                UpdateCameraPosition();

                _lastMousePosition = currentMousePosition;
            }

            // Handle zooming with a scroll wheel
            var scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta == 0) return;
            _currentDistance = Mathf.Clamp(_currentDistance - scrollDelta * zoomSpeed, minDistance, maxDistance);
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            // Reset pivot rotation
            _cameraPivot.transform.rotation = Quaternion.identity;

            // Apply rotations in the correct order
            _cameraPivot.transform.Rotate(Vector3.up, _horizontalAngle);
            _cameraPivot.transform.Rotate(Vector3.right, _verticalAngle);

            // Position the camera based on the pivot's forward direction
            transform.position = _cameraPivot.transform.position - _cameraPivot.transform.forward * _currentDistance;

            // Look at the object
            transform.LookAt(objectTransform);
        }
    }
}