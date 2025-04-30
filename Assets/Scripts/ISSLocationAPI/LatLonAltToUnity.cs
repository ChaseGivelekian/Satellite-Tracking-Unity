using System;
using System.Collections;
using UnityEngine;

namespace ISSLocationAPI
{
    public class LatLonAltToUnity : MonoBehaviour
    {
        [SerializeField] private GameObject earth;
        private static float _earthRadius;

        private const double E = 8.1819190842622e-2;
        private static readonly double Esq = Math.Pow(E, 2);

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
            var lat = latitude * Mathf.Deg2Rad;
            var lon = longitude * Mathf.Deg2Rad;

            var n = _earthRadius / Math.Sqrt(1 - Esq * Math.Pow(Math.Sin(lat), 2));

            var x = (n + altitude) * Math.Cos(lat) * Math.Cos(lon);
            var z = (n + altitude) * Math.Cos(lat) * Math.Sin(lon);
            var y = ((1 - Esq) * n + altitude) * Math.Sin(lat);

            var position = new Vector3((float)x, (float)y, (float)z);

            // Apply Earth's rotation to the calculated position
            if (earth)
            {
                position = earth.transform.rotation * position;
            }

            return position;
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