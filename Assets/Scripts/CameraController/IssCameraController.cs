using UnityEngine;

namespace CameraController
{
    public class IssCameraController : MonoBehaviour
    {
        [Header("Target")] 
        public Transform issTransform;

        [Header("Orbit Settings")] 
        public float orbitSpeed = 50f;
        public float initialDistance = 10f;
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
            if (issTransform == null)
            {
                Debug.LogWarning("ISS transform not assigned to Camera Controller!");
                // Try to find ISS in the scene
                var issObj = GameObject.Find("ISS");
                if (issObj != null)
                    issTransform = issObj.transform;
                else
                {
                    Debug.LogError("Could not find ISS object in scene!");
                    enabled = false;
                    return;
                }
            }

            // Set the initial distance
            _currentDistance = initialDistance;
            
            // Initialize camera position
            UpdateCameraPosition();
        }

        private void Update()
        {
            if (!issTransform)
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

                // Rotate the camera around the ISS based on mouse movement
                transform.RotateAround(issTransform.position, transform.right,
                    -mouseDelta.y * orbitSpeed * Time.deltaTime);
                transform.RotateAround(issTransform.position, Vector3.up,
                    mouseDelta.x * orbitSpeed * Time.deltaTime);

                _lastMousePosition = currentMousePosition;
                
                // Store the current offset direction after rotation
                _offsetDirection = (transform.position - issTransform.position).normalized;
            }

            // Handle zooming with a scroll wheel
            var scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta != 0)
            {
                _currentDistance = Mathf.Clamp(_currentDistance - scrollDelta * zoomSpeed, minZoomDistance, maxZoomDistance);
                
                // Store the current offset direction if we haven't been orbiting
                if (!_isOrbiting)
                {
                    _offsetDirection = (transform.position - issTransform.position).normalized;
                }
            }

            // Update camera position to follow the ISS as it moves
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            if (!issTransform) return;
            
            // If we don't have an offset direction yet, initialize it
            if (_offsetDirection == Vector3.zero)
            {
                // Default to a position behind and slightly above the ISS
                transform.position = issTransform.position - issTransform.forward * _currentDistance + Vector3.up * 2f;
                _offsetDirection = (transform.position - issTransform.position).normalized;
            }
            else
            {
                // Position the camera based on the current offset direction and distance
                transform.position = issTransform.position + _offsetDirection * _currentDistance;
            }
            
            // Always look at the ISS
            transform.LookAt(issTransform);
        }
    }
}