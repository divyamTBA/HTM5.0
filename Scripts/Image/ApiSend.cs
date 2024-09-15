using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;
public class ApiSend
{
    [SerializeField] private string apiUrl = "https://twis.in/shop/htm/api.php";


    public IEnumerator UploadCoroutine(string apiUrl, string name, byte[] imageBytes, string description, int frameIndex, string price, Action a = null)
    {
        LoadingManager.Instance.StartLoad(5);
        // LoadingManager.Instance.StartLoad(2.5f);
        // Convert byte array to Base64 string
        string base64Image = Convert.ToBase64String(imageBytes);

        // Create the JSON payload
        string jsonPayload = JsonUtility.ToJson(new UploadData
        {
            name = name,
            image = base64Image,
            description = description,
            frameIndex = frameIndex,
            price = price
        });
        LoadingManager.Instance.SetLoadingText("Uploading your art...");
        // Create a UnityWebRequest to send the data
        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                LoadingManager.Instance.SetLoadingText("Failed");
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                LoadingManager.Instance.SetLoadingText("Success");
                Debug.Log("Response from API:");
                Debug.Log(www.downloadHandler.text);
                Debug.Log(name);
                Debug.Log(description);
                Debug.Log(frameIndex);
                Debug.Log(apiUrl);
                Debug.Log(imageBytes.Length);
            }
            a ??= (() => Debug.Log("test"));
            a();
        }
    }

    [System.Serializable]
    private class UploadData
    {
        public string name;
        public string image;
        public string description;
        public int frameIndex;
        public string price;
    }
}