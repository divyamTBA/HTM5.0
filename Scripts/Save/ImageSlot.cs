using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class ImageSlot : MonoBehaviour, IDataPersistence
{
    public int index;
    public string tokenSendUrl;
    public string userSendUrl;
    public string imageName;
    [Multiline] public string imageDescription;
    private byte[] imageBytes;
    public int frameIndex = -1;
    public DataPersistenceManager dataPersistenceManager;
    [Header("UI")]
    public TMP_Text imageNameText;
    public TMP_Text imageDescriptionText;
    public RawImage rawImage;
    public Button uploadButton;
    public GameObject UploadPanel;
    [SerializeField] public Texture2D texture;
    public ApiFetch apiFetch;
    public TransitionControllerGame loadingTransition;
    void Start()
    {
        dataPersistenceManager = FindObjectOfType<DataPersistenceManager>();
        this.uploadButton.onClick.AddListener(OpenUploadPanel);
        UploadPanel.SetActive(false);
        this.tokenSendUrl = URLManager.instance.tokenSendUrl;
        this.userSendUrl = URLManager.instance.userSendUrl;
    }

    public void OpenUploadPanel()
    {
        UploadPanel.SetActive(true);
        UploadPanel.transform.Find("RawImage").GetComponent<Button>().onClick.AddListener(PickImage);
        UploadPanel.GetComponentInChildren<RawImage>().texture = null;
        UploadPanel.transform.Find("nameInput").GetComponent<TMP_InputField>().text = "";
        UploadPanel.transform.Find("descInput").GetComponent<TMP_InputField>().text = "";

    }
    public void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
       {
           Debug.Log("Image path: " + path);
           if (path != null)
           {
               texture = NativeGallery.LoadImageAtPath(path);
               UploadPanel.GetComponentInChildren<RawImage>().texture = texture;
               if (texture == null)
               {
                   Debug.Log("Couldn't load texture from " + path);
                   UploadPanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
                     {
                         UploadPanel.SetActive(false);
                     });
                   return;
               }
               else
               {
                   UploadPanel.transform.Find("RawImage").GetComponent<Button>().onClick.RemoveAllListeners();
                   UploadPanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        string name = UploadPanel.transform.Find("nameInput").GetComponent<TMP_InputField>().text;
                        string desc = UploadPanel.transform.Find("descInput").GetComponent<TMP_InputField>().text;
                        byte[] data = File.ReadAllBytes(path);
                        SaveImageData(name, desc, data, texture);
                        ApiSend apiSender = new();
                        SetIndex(
                        () =>
                        {
                            this.frameIndex = apiFetch.fetchedItems != null ? apiFetch.fetchedItems.Count + 1 : 0;
                            Debug.Log($"set the frame index of slot {index} to : {frameIndex}");
                            StartCoroutine(apiSender.UploadCoroutine(tokenSendUrl, name, data, desc, this.frameIndex, "100", () => StartCoroutine(apiFetch.FetchData())));
                        },
                        () =>
                        {
                            StartCoroutine(apiSender.UploadCoroutine(tokenSendUrl, name, data, desc, this.frameIndex, "100", () => StartCoroutine(apiFetch.FetchData())));

                        }
                        );
                        dataPersistenceManager.SaveGame();
                        UploadPanel.SetActive(false);
                    });
               }
           }
       });

        Debug.Log("Permission result: " + permission);
        Debug.Log("Image loaded successfully");
    }

    public void SetIndex(Action a, Action b)
    {
        if (this.frameIndex < 0)
        {
            StartCoroutine(apiFetch.FetchData(a, false));
            // this.frameIndex = apiFetch.items.Count + 1;
        }
        else
        {
            Debug.Log($"Frame index not updated");
            b();
        }


    }
    public void SaveImageData(string imageName, string description, byte[] data, Texture2D texture)
    {
        this.imageName = imageName;
        this.imageBytes = data;
        this.imageDescription = description;
        this.texture = texture;
        UpdateImageDetails();
    }
    public void UpdateImageDetails()
    {
        this.rawImage.texture = this.texture;
        this.imageNameText.text = this.imageName;
        this.imageDescriptionText.text = this.imageDescription;
    }
    public void LoadData(GameData data)
    {
        if (index < 0 || index >= GameData.MaxImageSlots)
        {
            Debug.LogError($"Invalid slot index: {index}");
        }
        ImageData imageData = data.imageSlots[index];
        this.imageBytes = imageData.imageBytes;
        this.imageName = imageData.imageName;
        this.imageDescription = imageData.imageDescription;
        this.frameIndex = imageData.frameIndex;

        this.texture = new Texture2D(100, 100);
        this.texture.LoadImage(this.imageBytes);
        Debug.Log("Loaded data for slot " + this.index);
        UpdateImageDetails();
    }

    public void SaveData(GameData data)
    {
        if (index < 0 || index >= GameData.MaxImageSlots)
        {
            Debug.LogError($"Invalid slot index: {index}");
        }

        data.imageSlots[index].imageName = this.imageName;
        data.imageSlots[index].imageBytes = this.imageBytes;
        data.imageSlots[index].imageDescription = this.imageDescription;
        data.imageSlots[index].frameIndex = this.frameIndex;
        Debug.Log("Saved data for slot " + this.index);
    }
}
