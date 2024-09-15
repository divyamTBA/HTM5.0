using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Unity.VisualScripting;
public class PersonalUploadPanel : MonoBehaviour
{
    public static PersonalUploadPanel Instance { get; private set; }
    public ImageData dataToChange;
    public Button uploadButton;
    public TMP_InputField nameInput;
    public TMP_InputField descInput;
    public RawImage rawImage;
    public PersonalSllot slotToChange;
    private void Awake()
    {
        // Ensure only one instance of DataPersistenceManager exists
        if (Instance != null)
        {
            Debug.Log("Found more than one Upload Panel in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void OnEnable()
    {
        rawImage.GetComponent<Button>().onClick.RemoveAllListeners();
        uploadButton.interactable = false;
        nameInput.text = "";
        descInput.text = "";
        rawImage.texture = null;
        rawImage.GetComponent<Button>().onClick.AddListener(PickImage);
    }
    private void Start()
    {
        this.gameObject.SetActive(false);
    }
    public void SetImageDataVariables(ImageData imageData)
    {
        imageData.imageName = this.nameInput.text;
        imageData.imageDescription = this.descInput.text;
    }
    public void SetData(ImageData imageData)
    {
        this.dataToChange = imageData;
    }
    public void SetSlot(PersonalSllot slot)
    {
        this.slotToChange = slot;
    }
    public void PickImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
       {
           Debug.Log("Image path: " + path);
           if (path != null)
           {
               // Create Texture from selected image
               Texture2D texture = NativeGallery.LoadImageAtPath(path);
               slotToChange.texture = texture;
               slotToChange.rawImage.texture = texture;
               //File.WriteAllBytes(Application.persistentDataPath + "/art" + "uploadImg", data);

               rawImage.texture = texture;
               if (texture == null)
               {
                   Debug.Log("Couldn't load texture from " + path);
                   uploadButton.onClick.RemoveAllListeners();
                   uploadButton.onClick.AddListener(() =>
                     {
                         this.gameObject.SetActive(false);
                     });
                   uploadButton.interactable = true;
                   return;
               }
               else
               {
                   slotToChange.imageData.imageBytes = File.ReadAllBytes(path);
                   rawImage.GetComponent<Button>().onClick.RemoveAllListeners();
                   uploadButton.onClick.AddListener(() =>
                    {
                        SetImageDataVariables(this.dataToChange);
                        this.gameObject.SetActive(false);
                    });
                   uploadButton.interactable = true;
               }
           }
       });

        Debug.Log("Permission result: " + permission);
        Debug.Log("Image loaded successfully");
    }
}