using UnityEngine;

namespace CameraController
{
    public class SatelliteCameraController : MonoBehaviour
    {
        [Header("Target")] public Transform satelliteTransform;

        [Header("Orbit Settings")] public float orbitSpeed = 50f;
        public float initialDistance;
        public float minZoomDistance = 5f;
        public float maxZoomDistance = 30f;
        public float zoomSpeed = 2f;

        // Private variables
        private float _currentDistance;
        private bool _isOrbiting;
        private Vector2 _lastMousePosition;
        private Vector3 _offsetDirection;

        private void Start()
        {
            if (satelliteTransform == null)
            {
                Debug.LogWarning("Transform not assigned to Camera Controller!");
            }

            // Set the initial distance
            _currentDistance = initialDistance;

            // Initialize camera position
            UpdateCameraPosition();
        }

        private void Update()
        {
            if (!satelliteTransform)
                return;

            // Check for mouse button press
            if (Input.GetMouseButtonDown(0))
            {
                _isOrbiting = true;
                _lastMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            // Check for mouse button release
            if (Input.GetMouseButtonUp(0))
            {
                _isOrbiting = false;
            }

            // Handle orbiting while the mouse button is held down
            if (_isOrbiting)
            {
                var currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                var mouseDelta = currentMousePosition - _lastMousePosition;

                // Rotate the camera around the satellite based on mouse movement
                transform.RotateAround(satelliteTransform.position, transform.right,
                    -mouseDelta.y * orbitSpeed * Time.deltaTime);
                transform.RotateAround(satelliteTransform.position, Vector3.up,
                    mouseDelta.x * orbitSpeed * Time.deltaTime);

                _lastMousePosition = currentMousePosition;

                // Store the current offset direction after rotation
                _offsetDirection = (transform.position - satelliteTransform.position).normalized;
            }

            // Handle zooming with a scroll wheel
            var scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta != 0)
            {
                _currentDistance = Mathf.Clamp(_currentDistance - scrollDelta * zoomSpeed, minZoomDistance,
                    maxZoomDistance);

                // Store the current offset direction if we haven't been orbiting
                if (!_isOrbiting)
                {
                    _offsetDirection = (transform.position - satelliteTransform.position).normalized;
                }
            }

            // Update camera position to follow the satellite as it moves
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            if (!satelliteTransform) return;

            // If we don't have an offset direction yet, initialize it
            if (_offsetDirection == Vector3.zero)
            {
                // Default to a position behind and slightly above the satellite
                transform.position = satelliteTransform.position - satelliteTransform.forward * _currentDistance +
                                     Vector3.up * 2f;
                _offsetDirection = (transform.position - satelliteTransform.position).normalized;
            }
            else
            {
                // Position the camera based on the current offset direction and distance
                transform.position = satelliteTransform.position + _offsetDirection * _currentDistance;
            }

            // Always look at the satellite
            transform.LookAt(satelliteTransform);
        }
    }
}