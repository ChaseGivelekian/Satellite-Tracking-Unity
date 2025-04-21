using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ISSLocationAPI
{
    public class IssLocation : MonoBehaviour
    {
        private string _issData;

        private void Start()
        {
            StartCoroutine(GetIssData());
        }

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
                        Debug.Log(_issData);
                    }
                    else
                    {
                        Debug.LogError($"Error: {request.error}");
                    }
                }

                yield return waitTime;
            }
        }
    }
}