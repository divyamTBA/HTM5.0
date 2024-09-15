using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using EasyTransition;
using UniqueKey;
using Unity.VisualScripting;
using System.IO;
public class MainMenuManager : MonoBehaviour, IDataPersistence
{
    public TMP_InputField playerNameInput;
    public Button enterButton;
    public TransitionSettings transition;
    public float transitionDelay;
    public string tokenId;
    public string userName;
    public VerifyName verifyName;
    public Button globalButton;
    public Button ImageButton;
    private byte[] imageData;
    // Start is called before the first frame update
    void Start()
    {
        playerNameInput.text = this.userName;
        playerNameInput.onEndEdit.AddListener((string text) =>
        {
            GenerateCode(5);
            this.userName = playerNameInput.text;
            // SetupButton(this.userName, this.tokenId, this.imageData);
        });
        globalButton.onClick.AddListener(SetWorldUrls);
        ImageButton.onClick.AddListener(PickImage);
    }

    public void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
{
    Debug.Log("Image path: " + path);
    if (path != null)
    {
        Texture2D texture = NativeGallery.LoadImageAtPath(path);
        if (texture == null)
        {
            Debug.Log("Couldn't load texture from " + path);
            return;
        }
        else
        {

            this.imageData = File.ReadAllBytes(path);
            Debug.Log(this.imageData.Length);
            SetupButton(this.userName, this.tokenId, this.imageData);

        }
    }
});

        Debug.Log("Permission result: " + permission);
        Debug.Log("Image loaded successfully");
    }

    public void GenerateCode(int length)
    {
        tokenId = tokenId == "" ? KeyGenerator.GetUniqueKey(length) : tokenId;
        Debug.Log(tokenId);
    }

    public void LoadData(GameData data)
    {
        this.userName = data.userName;
        this.tokenId = data.tokenId;
    }

    public void SaveData(GameData data)
    {
        data.userName = this.userName;
        data.tokenId = this.tokenId;
    }
    void SetupButton(string userName, string tokenId, byte[] imageData)
    {
        enterButton.interactable = true;
        // enterButton.onClick.AddListener(() => TransitionManager.Instance().Transition("Playground", transition, transitionDelay));
        enterButton.onClick.AddListener(() =>
        {
            verifyName.StartCheck(userName, tokenId, imageData);
        }

        );
    }
    public void SetWorldUrls()
    {
        OpenPrivateRoom.Instance.SetWorldDetails(PlayerPrefs.GetString("Username"));
    }
    public void SetTransition()
    {
        TransitionManager.Instance().Transition("Playground", transition, transitionDelay);
    }
}
