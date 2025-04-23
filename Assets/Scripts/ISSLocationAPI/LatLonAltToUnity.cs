using System;
using UnityEngine;

namespace ISSLocationAPI
{
    public class LatLonAltToUnity : MonoBehaviour
    {
        // Radius of the Earth sphere in your Unity scene (adjust as needed)
        public float earthRadius = 6371f; // Approximate average radius in kilometers

        // Function to convert latitude, longitude, and altitude to a Unity position
        private Vector3 ConvertLatLonAltToUnityPosition(double latitude, double longitude, double altitude)
        {
            // Convert latitude and longitude to radians
            var latitudeRad = Math.PI * latitude / 180.0;
            var longitudeRad = Math.PI * longitude / 180.0;

            // Add altitude to the Earth's radius to get the distance from the center
            var distance = earthRadius + altitude;

            // Calculate the X, Y, and Z coordinates based on spherical coordinates
            // Note: Unity's coordinate system has Y as the up axis, while in geocentric
            // coordinates, Z is often the polar axis. We'll adjust accordingly.

            // X coordinate (East-West)
            var x = (float)(distance * Math.Cos(latitudeRad) * Math.Cos(longitudeRad));

            // Z coordinate (North-South) - Swapped with Y for Unity's up axis
            var z = (float)(distance * Math.Cos(latitudeRad) * Math.Sin(longitudeRad));

            // Y coordinate (Up-Down) - Corresponds to the geocentric Z-axis
            var y = (float)(distance * Math.Sin(latitudeRad));

            // Return the Unity Vector3 position
            return new Vector3(x, y, z);
        }

        // Example usage with the provided JSON data
        public void IssToUnity(float latitude, float longitude, float altitude)
        {
            // Data from the JSON
            // var latitude = 50.11496269845;
            // var longitude = 118.07900427317;
            // var altitude = 408.05526028199; // in kilometers

            // Convert the coordinates to a Unity position
            var issPosition = ConvertLatLonAltToUnityPosition(latitude, longitude, altitude);

            // Create a GameObject to represent the ISS (optional)
            var issObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            issObject.name = "ISS";

            // Set the position of the ISS object relative to the Earth sphere
            // Assuming your Earth sphere is at the origin (0, 0, 0)
            issObject.transform.position = issPosition;

            // You might want to scale down the ISS object for visualization
            issObject.transform.localScale = Vector3.one * 0.1f; // Example scaling

            Debug.Log("ISS Unity Position: " + issPosition);
        }
    }
}