using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Networking;

namespace SatellitesLocation.GetLocation
{
    public class SatellitesLocation : MonoBehaviour
    {
        [SerializeField] private int satelliteId;
        [SerializeField] private LatLonAltToUnity.LatLonAltToUnity latLonAltToUnity;
        private SatelliteData _satelliteData;

        [System.Serializable]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public class SatelliteInfo
        {
            public string satname;
            public int satid;
            public int transactionscount;
        }

        [System.Serializable]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public class SatellitePosition
        {
            public float satlatitude;
            public float satlongitude;
            public float sataltitude;
            public float azimuth;
            public float elevation;
            public float ra;
            public float dec;
            public long timestamp;
        }

        [System.Serializable]
        public class SatelliteData
        {
            public SatelliteInfo info;
            public List<SatellitePosition> positions;
        }

        private void Start()
        {
            if (latLonAltToUnity == null)
            {
                Debug.LogError("LatLonAltToUnity reference not assigned to SatellitesLocation!");
                enabled = false;
                return;
            }

            StartCoroutine(GetSatelliteData());
        }

        private IEnumerator GetSatelliteData()
        {
            var waitTime = new WaitForSecondsRealtime(1.0f);
            var transactionCount = 0;

            do
            {
                using (var request =
                       UnityWebRequest.Get(
                           $"https://api.n2yo.com/rest/v1/satellite/positions/{satelliteId}/41.702/-76.014/0/2/&apiKey=https://api.n2yo.com/rest/v1/satellite/positions/25544/41.702/-76.014/0/2/&apiKey=FT9GB3-V6KYXA-WWLW66-5GPP"))
                {
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        _satelliteData = JsonUtility.FromJson<SatelliteData>(request.downloadHandler.text);
                        transactionCount = _satelliteData.info.transactionscount;
                        GetLonLatAlt();
                    }
                    else
                    {
                        Debug.LogError($"Error: {request.error}");
                    }
                }

                Debug.Log(transactionCount);

                yield return waitTime;
            } while (transactionCount < 1000);
        }

        private void GetLonLatAlt()
        {
            if (_satelliteData?.positions == null || _satelliteData.positions.Count == 0)
                return;

            var position = _satelliteData.positions[0];

            var latitude = position.satlatitude;
            var longitude = position.satlongitude;
            var altitude = position.sataltitude;

            latLonAltToUnity.SatelliteToUnity(latitude, longitude, altitude);
        }
    }
}