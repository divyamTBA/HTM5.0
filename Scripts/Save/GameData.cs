using System;
using UnityEngine;

[Serializable]
public class ImageData
{
    public string imageName;
    [HideInInspector]
    public byte[] imageBytes;
    public string imageDescription;
    public int frameIndex = -1;
    public string price;
}

[Serializable]
public class GameData
{
    public bool IsVerified;
    public string userSendUrl;
    public string tokenFetchUrl;
    public string tokenSendUrl;
    public string userName;
    public string tokenId;
    public const int MaxImageSlots = 4;
    public const int MaxPersonalImageSlots = 20;
    public ImageData[] imageSlots;
    public ImageData[] personalImageSlots;
    public long lastUpdated;

    public GameData()
    {
        IsVerified = false;
        userName = "Anonymous";
        tokenId = "";
        imageSlots = new ImageData[MaxImageSlots];
        for (int i = 0; i < MaxImageSlots; i++)
        {
            imageSlots[i] = new ImageData();
        }
        personalImageSlots = new ImageData[MaxPersonalImageSlots];
        for (int j = 0; j < MaxPersonalImageSlots; j++)
        {
            personalImageSlots[j] = new ImageData();
        }
    }
}