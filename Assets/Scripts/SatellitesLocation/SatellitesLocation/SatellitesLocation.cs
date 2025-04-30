using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SatellitesLocation.SatellitesLocation
{
    public class SatellitesLocation : MonoBehaviour
    {
        private void Start()
        {

        }

        private void Update()
        {

        }

        private IEnumerator GetSatelliteData()
        {
            var waitTime = new WaitForSecondsRealtime(1.0f);

            while (true)
            {
                using (var request = UnityWebRequest.Get(""))
                {

                }
            }
        }
    }
}