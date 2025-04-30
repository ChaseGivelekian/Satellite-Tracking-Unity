using UnityEngine;
using System;

namespace Solar_System
{
    public class EarthRotation : MonoBehaviour
    {
        [Header("Rotation Settings")] public float rotationSpeed = .00416667f;

        [Tooltip("Earth's axial tilt in degrees")]
        public float axialTilt = 23.5f;

        [Tooltip("Rotation axis direction")] public Vector3 rotationAxis = Vector3.up;

        private void Start()
        {
            // Set initial rotation based on the current GMT time
            SetInitialRotationBasedOnGMT();

            // Apply the axial tilt to the Earth at initialization
            ApplyAxialTilt();
        }

        private void Update()
        {
            // Rotate the Earth around its tilted axis
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }

        private void ApplyAxialTilt()
        {
            // Calculate the tilted axis
            rotationAxis = Quaternion.Euler(axialTilt, 0, 0) * Vector3.up;
        }

        private void SetInitialRotationBasedOnGMT()
        {
            var utcNow = DateTime.UtcNow;

            // Calculate rotation around the Y-axis based on UTC time
            var rotationY = (utcNow.Hour + utcNow.Minute / 60f) * -15f;

            // Apply the rotation 
            transform.rotation = Quaternion.Euler(axialTilt, rotationY - 180f, 0f);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw the rotation axis for visualization in the editor
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, rotationAxis.normalized * 2.0f);
        }
    }
}