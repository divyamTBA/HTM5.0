using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PersonalSllot : MonoBehaviour, IDataPersistence
{
    public ImageData imageData;
    public Button button;
    public int index;
    public Texture2D texture;
    public RawImage rawImage;
    private void Awake()
    {
        this.rawImage = GetComponentInChildren<RawImage>();
    }
    public void LoadData(GameData data)
    {
        if (index < 0 || index >= GameData.MaxPersonalImageSlots)
        {
            Debug.LogError($"Invalid personal slot index: {index}");
        }
        PersonalUploadManager.Instance.imageDatas = data.personalImageSlots.ToList();
        ImageData imageData = data.personalImageSlots[index];

        this.imageData.imageBytes = imageData.imageBytes;
        this.imageData.imageName = imageData.imageName;
        this.imageData.imageDescription = imageData.imageDescription;
        this.imageData.frameIndex = imageData.frameIndex;
        this.imageData.price = imageData.price;

        this.texture = new Texture2D(100, 100);
        this.texture.LoadImage(this.imageData.imageBytes);
        this.rawImage.texture = this.texture;

        Debug.Log("Loaded data for personal slot " + this.index);
    }

    public void SaveData(GameData data)
    {
        if (index < 0 || index >= GameData.MaxPersonalImageSlots)
        {
            Debug.LogError($"Invalid personal slot index: {index}");
        }
        PersonalUploadManager.Instance.imageDatas = data.personalImageSlots.ToList();
        data.personalImageSlots[index].imageName = this.imageData.imageName;
        data.personalImageSlots[index].imageBytes = this.imageData.imageBytes;
        data.personalImageSlots[index].imageDescription = this.imageData.imageDescription;
        data.personalImageSlots[index].frameIndex = this.imageData.frameIndex;
        data.personalImageSlots[index].price = this.imageData.price;

        Debug.Log("Saved data for personal slot " + this.index);
    }

    private void Start()
    {
        this.imageData.frameIndex = this.index + 1;
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            PersonalUploadPanel.Instance.gameObject.SetActive(true);
            PersonalUploadPanel.Instance.SetData(this.imageData);
            PersonalUploadPanel.Instance.SetSlot(this);
        });


    }
}
