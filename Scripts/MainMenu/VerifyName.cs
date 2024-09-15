using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UniqueKey;
using System;
public class VerifyName : MonoBehaviour, IDataPersistence
{
    public GameObject LoginPanel;
    public bool verified;
    public string serverURL = "";
    public ApiResponse response;
    public GameObject spinner;
    public bool isUsernameAvailable = false;
    public TMP_Text statusText;
    public Button LoginButton;

    private void Start()
    {
        if (!verified)
        {
            LoginPanel.SetActive(true);
        }
    }
    IEnumerator CheckUsername(string usernameToCheck, string tokenId, byte[] imageData)
    {
        string base64Image = Convert.ToBase64String(imageData);
        string imageDataUrl = "data:image/png;base64," + base64Image;  // Adjust mime type if needed


        WWWForm form = new WWWForm();
        form.AddField("username", usernameToCheck);
        form.AddField("token_id", tokenId);
        form.AddField("image", imageDataUrl);  // Send the image as base64 string
        spinner.SetActive(true);
        using (UnityWebRequest www = UnityWebRequest.Post(serverURL, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                response = JsonUtility.FromJson<ApiResponse>(jsonResponse);

                if (response != null)
                {
                    isUsernameAvailable = response.status == "success";
                    Debug.Log(response.message);
                    // statusText.text = response.message;
                    statusText.text = response.message;
                    statusText.color = isUsernameAvailable ? Color.green : Color.red;

                    if (isUsernameAvailable)
                    {
                        PlayerPrefs.SetString("Username", usernameToCheck);
                        PlayerPrefs.SetString("TokenId", tokenId);
                        verified = true;
                        LoginPanel.SetActive(false);
                        DataPersistenceManager.instance.SaveGame();
                    }
                    else
                    {
                        // Disable login button or show error
                        verified = false;
                    }
                }
                else
                {
                    Debug.LogError("Failed to parse JSON response.");
                }

                spinner.SetActive(false);
            }
        }
    }
    public void StartCheck(string usernameToCheck, string tokenId, byte[] imageData)
    {
        StartCoroutine(CheckUsername(usernameToCheck, tokenId, imageData));
    }

    public void LoadData(GameData data)
    {
        this.verified = data.IsVerified;
    }

    public void SaveData(GameData data)
    {
        data.IsVerified = this.verified;
    }

    [System.Serializable]
    public class ApiResponse
    {
        public string status;
        public string message;
    }
}
