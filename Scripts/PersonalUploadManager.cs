using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersonalUploadManager : MonoBehaviour
{
    public static PersonalUploadManager Instance { get; private set; }
    public List<ImageData> imageDatas;
    public Button uploadbutton;
    public Button loadSceneButton;
    public string userName;
    public string userCode;
    public string tokenSendurl;
    public string userSendurl;
    private void Awake()
    {
        // Ensure only one instance of DataPersistenceManager exists
        if (Instance != null)
        {
            Debug.Log("Found more than one Upload Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void OnEnable()
    {
        uploadbutton.onClick.AddListener(() =>
        {
            DataPersistenceManager.instance.SaveGame();
            OpenPrivateRoom.Instance.SetPrivateDetails(PlayerPrefs.GetString("Username"), PlayerPrefs.GetString("TokenId"), false);
            tokenSendurl = URLManager.instance.tokenSendUrl;
            userSendurl = URLManager.instance.userSendUrl;
            SendData();
            loadSceneButton.interactable = true;
            loadSceneButton.onClick.AddListener(() =>
            {
                URLManager.instance.SetTransition();
            });
        });
    }
    public void SendData()
    {
        ApiSend apiSender = new();
        foreach (ImageData item in imageDatas)
        {
            if (item.imageName != null && item.imageName != "")
            {
                item.price = "100";
                StartCoroutine(apiSender.UploadCoroutine(tokenSendurl, item.imageName, item.imageBytes, item.imageDescription, item.frameIndex, item.price));
            }

        }
    }
}
