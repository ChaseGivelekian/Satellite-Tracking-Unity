using UnityEngine;

namespace Solar_System
{
    public class SunOrbit : MonoBehaviour
    {
        [Header("References")] public Transform earth;

        [Header("Orbit Settings")] [Tooltip("Distance from the sun to the earth (in Unity units)")]
        public float orbitDistance = 149.6f; // Scaled distance

        [Tooltip("Speed of the sun's orbit (degrees per second)")]
        public float orbitSpeed = 0.1f; // Slower for realism

        [Tooltip("Tilt of the orbital plane in degrees")]
        public float orbitTilt = 23.5f; // Earth's axial tilt

        [Tooltip("Initial angle of the sun in the orbit (in degrees)")]
        public float initialAngle;

        private float _currentAngle;
        private Vector3 _rotationAxis;

        private void Start()
        {
            if (!earth)
            {
                Debug.LogWarning("Earth transform not assigned to SunOrbit. Will use world origin instead.");
            }

            // Set the initial position
            _currentAngle = initialAngle;

            // Create a rotation axis with tilt
            _rotationAxis = Quaternion.Euler(orbitTilt, 0, 0) * Vector3.up;

            // Set the initial position
            UpdatePosition();
        }

        private void Update()
        {
            // Update the orbit angle
            _currentAngle += orbitSpeed * Time.deltaTime;
            if (_currentAngle >= 360f) _currentAngle -= 360f;

            // Update position
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            // Get earth position (or use origin if not assigned)
            var earthPosition = earth ? earth.position : Vector3.zero;

            // Calculate position in the orbital plane
            var radians = _currentAngle * Mathf.Deg2Rad;
            var offset = new Vector3(
                Mathf.Sin(radians) * orbitDistance,
                0,
                Mathf.Cos(radians) * orbitDistance
            );

            // Apply tilt to the orbit
            offset = Quaternion.Euler(orbitTilt, 0, 0) * offset;

            // Set position
            transform.position = earthPosition + offset;

            // Make the directional light always point at the earth
            transform.LookAt(earthPosition);
        }

        // For visualizing the orbit in the editor
        private void OnDrawGizmosSelected()
        {
            var center = earth ? earth.position : Vector3.zero;

            // Set gizmo color
            Gizmos.color = Color.yellow;

            // Draw orbit circle
            const int segments = 36;
            var prev = Vector3.zero;

            for (var i = 0; i <= segments; i++)
            {
                var angle = (float)i / segments * 2 * Mathf.PI;
                var pos = new Vector3(
                    Mathf.Sin(angle) * orbitDistance,
                    0,
                    Mathf.Cos(angle) * orbitDistance
                );

                // Apply tilt
                pos = Quaternion.Euler(orbitTilt, 0, 0) * pos;

                if (i > 0)
                {
                    Gizmos.DrawLine(center + prev, center + pos);
                }

                prev = pos;
            }
        }
    }
}