using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
public class SetDetails : MonoBehaviour
{
    [System.Serializable]
    public class AdressDetails
    {
        public TMP_InputField inputField;
        public string playerPref;
    }
    public Button SaveButton;
    public Button SendButton;
    public AdressDetails[] details;
    public string serverURL;
    private void SetData()
    {
        foreach (AdressDetails details in details)
        {
            PlayerPrefs.SetString($"{details.playerPref}", details.inputField.text);
        }
    }
    private void Start()
    {
        // foreach (AdressDetails details in details)
        // {
        //     PlayerPrefs.SetString($"{details.playerPref}", details.inputField.text);
        // }
        SendButton.onClick.AddListener(() =>
        {
            StartCoroutine(SendDataToAPI());
        });
    }

    IEnumerator SendDataToAPI()
    {
        WWWForm form = new WWWForm();
        form.AddField("firstName", "Divyam");
        form.AddField("lastName", "Gupta");
        form.AddField("address", "Marwadi University");
        form.AddField("city", "Rajkot");
        form.AddField("state", "Gujrat");
        form.AddField("postcode", "360003");
        form.AddField("country", "India");
        form.AddField("email", "divyam@email.com");
        form.AddField("number", "1234567890");

        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
            }
        }
    }
}
