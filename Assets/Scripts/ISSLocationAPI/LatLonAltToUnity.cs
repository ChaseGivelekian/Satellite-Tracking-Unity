using System;
using System.Collections;
using UnityEngine;

namespace ISSLocationAPI
{
    public class LatLonAltToUnity : MonoBehaviour
    {
        [SerializeField] private GameObject earth;
        private float _earthRadius;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _isFirstPosition = true;
        private bool _isMoving;
        private Coroutine _moveCoroutine;

        private void Start()
        {
            _earthRadius = earth.GetComponent<Transform>().localScale.x / 2;
            _targetPosition = transform.position;
            _startPosition = transform.position;
        }

        private Vector3 ConvertLatLonAltToUnityPosition(double latitude, double longitude, double altitude)
        {
            // Convert latitude and longitude to radians
            var latitudeRad = Math.PI * latitude / 180.0;
            var longitudeRad = Math.PI * longitude / 180.0;
            
            // Add altitude to the Earth's radius to get the distance from the center
            var distance = _earthRadius + altitude;
            
            // In Unity's coordinate system:
            // - X is east-west
            // - Y is up-down
            // - Z is north-south

            // Calculate coordinates:
            // Unity coordinates need proper orientation adjustment
            // For a typical Earth representation in Unity:

            // X-axis -> -longitude (east is negative, west is positive)
            var x = (float)(distance * Math.Cos(latitudeRad) * Math.Sin(longitudeRad));

            // Y-axis -> latitude (north is positive, south is negative)
            var y = (float)(distance * Math.Sin(latitudeRad));

            // Z axis -> longitude (North Pole facing camera)
            var z = (float)(-distance * Math.Cos(latitudeRad) * Math.Cos(longitudeRad));
            
            // Return the Unity Vector3 position
            return new Vector3(x, y, z);
        }

        // Method to move the object smoothly over exactly 1 second
        private IEnumerator MoveOverOneSecond(Vector3 start, Vector3 end)
        {
            float elapsedTime = 0;
            const float duration = 1.1f;

            while (elapsedTime < duration)
            {
                var t = elapsedTime / duration;
                transform.position = Vector3.Lerp(start, end, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure we reach the exact end position
            transform.position = end;
            _isMoving = false;
        }

        public void IssToUnity(float latitude, float longitude, float altitude)
        {
            // Convert the coordinates to a Unity position
            var issPosition = ConvertLatLonAltToUnityPosition(latitude, longitude, altitude);

            if (_isFirstPosition)
            {
                // For the first position, set it directly without interpolation
                transform.position = issPosition;
                _targetPosition = issPosition;
                _startPosition = issPosition;
                _isFirstPosition = false;
            }
            else
            {
                // Stop any ongoing movement
                if (_isMoving && _moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);
                }

                // Start a new movement from current position to new target
                _startPosition = transform.position;
                _targetPosition = issPosition;
                _isMoving = true;
                _moveCoroutine = StartCoroutine(MoveOverOneSecond(_startPosition, _targetPosition));
            }
        }
    }
}