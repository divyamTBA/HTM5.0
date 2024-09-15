using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AdsManager : MonoBehaviour
{
    [System.Serializable]
    public class AdData
    {
        public string rowId;
        public string url;
    }

    [System.Serializable]
    public class AdDataList
    {
        public List<AdData> Items;
    }
    private string apiUrl = "https://fleastore.in/htm/advertisment/data.json"; // Replace with your API URL
    [SerializeField] private List<AdData> adsList = new List<AdData>();

    public AdPanel[] adPanels; // Assign the 5 AdPanel scripts in the Inspector

    void Start()
    {
        StartCoroutine(FetchAdsFromAPI());
    }

    IEnumerator FetchAdsFromAPI()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching ads: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            AdDataList adDataList = JsonUtility.FromJson<AdDataList>(JsonHelper.FixJsonArray(jsonResponse));
            adsList = adDataList.Items;

            foreach (AdPanel adPanel in adPanels)
            {
                string targetRowId = adPanel.rowIndex.ToString();
                AdData matchingAd = adsList.Find(ad => ad.rowId == targetRowId);

                if (matchingAd != null)
                {
                    adPanel.SetAdURL(matchingAd.url); // Set the URL for the specific panel
                }
                else
                {
                    Debug.LogWarning("No ad found for row index: " + targetRowId);
                }
            }
        }
    }
    public static class JsonHelper
    {
        public static string FixJsonArray(string json)
        {
            return "{\"Items\":" + json + "}";
        }
    }
}
