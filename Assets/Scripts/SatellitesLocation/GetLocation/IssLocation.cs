using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Networking;

namespace SatellitesLocation.GetLocation
{
    public class IssLocation : MonoBehaviour
    {
        [SerializeField] private Transform sun;
        [SerializeField] private LatLonAltToUnity.LatLonAltToUnity latLonAltToUnity;
        private string _issData;

        [System.Serializable]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public class IssData
        {
            public string name;
            public int id;
            public float latitude;
            public float longitude;
            public float altitude;
            public float velocity;
            public string visibility;
            public float footprint;
            public long timestamp;
            public double daynum;
            public float solarLat;
            public float solarLon;
            public string units;
        }

        private void Start()
        {
            StartCoroutine(GetIssData());
        }

        private void Update()
        {
            if (!sun)
            {
                Debug.LogError("Sun transform not assigned to IssLocation!");
                enabled = false;
                return;
            }

            transform.LookAt(sun);
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator GetIssData()
        {
            var waitTime = new WaitForSecondsRealtime(1.0f);

            while (true)
            {
                using (var request = UnityWebRequest.Get("https://api.wheretheiss.at/v1/satellites/25544"))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        _issData = request.downloadHandler.text;
                        GetLonLatAlt();
                    }
                    else
                    {
                        Debug.LogError($"Error: {request.error}");
                    }
                }

                yield return waitTime;
            }
        }

        private void GetLonLatAlt()
        {
            if (string.IsNullOrEmpty(_issData))
                return;

            var issData = JsonUtility.FromJson<IssData>(_issData);

            var latitude = issData.latitude;
            var longitude = issData.longitude;
            var altitude = issData.altitude;

            latLonAltToUnity.SatelliteToUnity(latitude, longitude, altitude);
        }
    }
}