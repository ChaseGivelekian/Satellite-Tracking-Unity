using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform earthTransform; // Reference to the Earth object


    [Header("Initial Camera Setup")]
    public float initialDistance; // Starting distance from Earth

    [Header("Orbit Settings")]
    public float orbitSpeed = 10f; // Speed of orbit rotation
    public float minDistance; // Minimum distance from Earth
    public float maxDistance; // Maximum distance from Earth
    public float zoomSpeed = 5f; // Speed of zooming

    // Private variables
    private Vector3 _offset;
    private float _currentDistance;
    private bool _isOrbiting;
    private Vector2 _lastMousePosition;

    private void Start()
    {
        if (earthTransform == null)
        {
            Debug.LogWarning("Earth transform not assigned to Camera Controller!");
            // Try to find Earth in the scene
            var earthObj = GameObject.Find("Earth");
            if (earthObj != null)
                earthTransform = earthObj.transform;
        }

        // Set initial distance and position
        _currentDistance = initialDistance;
        _offset = new Vector3(0, 0, -_currentDistance);
        transform.position = earthTransform!.position + _offset;
        transform.LookAt(earthTransform);
    }

    private void Update()
    {
        if (!earthTransform)
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

        // Handle orbiting while the mouse is held down
        if (_isOrbiting)
        {
            var currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var mouseDelta = currentMousePosition - _lastMousePosition;

            // Rotate the camera around the Earth based on mouse movement
            transform.RotateAround(earthTransform.position, transform.right, -mouseDelta.y * orbitSpeed * Time.deltaTime);
            transform.RotateAround(earthTransform.position, Vector3.up, mouseDelta.x * orbitSpeed * Time.deltaTime);

            _lastMousePosition = currentMousePosition;
        }

        // Handle zooming with a scroll wheel
        var scrollDelta = Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            _currentDistance = Mathf.Clamp(_currentDistance - scrollDelta * zoomSpeed, minDistance, maxDistance);

            // Update position based on a new distance
            var direction = (transform.position - earthTransform.position).normalized;
            transform.position = earthTransform.position + direction * _currentDistance;
        }

        // Always look at the Earth
        transform.LookAt(earthTransform);
    }
}