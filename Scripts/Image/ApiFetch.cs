using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using Castle.Components.DictionaryAdapter.Xml;
using TMPro;

[System.Serializable]
public class ItemData
{
    public string name;
    public string image_url;
    public string description;
    public int frameIndex;
    public string price = "100";
}
[System.Serializable]
public class ItemDataWrapper
{
    public List<ItemData> items;
}

public class ApiFetch : MonoBehaviour
{
    public string jsonUrl;
    public List<ItemData> fetchedItems;
    public Transform FrameLocationsParent;
    // public Transform framesParent;
    public List<Frame> Frames;

    [Header("UI")]
    public Transform canvasParent;
    public GameObject frameTextPrefab;

    private void Awake()
    {
        this.jsonUrl = URLManager.instance.tokenFetchUrl;
    }
    public void StartFetch(bool updatePanels)
    {
        StartCoroutine(FetchData(null, updatePanels));
    }
    public IEnumerator FetchData(Action a = null, bool updatePanels = true)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(jsonUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string jsonResult = webRequest.downloadHandler.text;
                Debug.Log(jsonResult);
                try
                {
                    ItemDataWrapper wrapper = JsonUtility.FromJson<ItemDataWrapper>("{\"items\":" + jsonResult + "}");
                    fetchedItems = wrapper.items;
                }
                catch (System.Exception e) { Debug.Log(e); }

                if (fetchedItems != null && fetchedItems.Count > 0)
                {
                    LoadingManager.Instance.StartLoad(7f);
                    LoadingManager.Instance.SetLoadingText("Fetching artworks...");

                    if (updatePanels)
                    {
                        foreach (Transform child in canvasParent)
                        {
                            Destroy(child.gameObject);
                        }
                        foreach (ItemData item in fetchedItems)
                        {
                            StartCoroutine(DownloadAndDisplayImage(item.image_url, item));
                        }
                    }
                    // ItemData firstItem = items[0];
                    // itemName = firstItem.name;
                    // itemDescription = firstItem.description;

                    // Debug.Log("Name: " + itemName);
                    // Debug.Log("Description: " + itemDescription);

                    // StartCoroutine(DownloadAndDisplayImage(firstItem.image_url));
                }
                else
                {
                    Debug.LogError("No items found in the JSON data.");
                }
            }
            a ??= (() => Debug.Log("test_from_fetch"));
            a();
        }
    }
    IEnumerator DownloadAndDisplayImage(string imageUrl, ItemData item)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading image: " + webRequest.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                Debug.Log("downloaded texture from: " + imageUrl);
                DisplayImageOnQuad(texture, item);
            }
        }
    }
    void DisplayImageOnQuad(Texture2D texture, ItemData item)
    {
        int index = fetchedItems.IndexOf(item);
        // GameObject quad = Instantiate(quadPrefab, Transforms.GetChild(index).position, Transforms.GetChild(index).rotation, framesParent);
        GameObject frameObject = FrameLocationsParent.GetChild(index).gameObject;
        frameObject.name = "F" + item.frameIndex.ToString("00");
        Frame frame = frameObject.GetComponent<Frame>();
        // Frame quadFrame = quad.GetComponent<Frame>();
        frame.SetFrame(item.frameIndex, item.image_url, texture, item.description, item.name, item.price);
        GameObject _frameText = Instantiate(frameTextPrefab, FrameLocationsParent.GetChild(index).position, FrameLocationsParent.GetChild(index).rotation, canvasParent);
        _frameText.GetComponentInChildren<TextMeshProUGUI>().text = "F" + item.frameIndex.ToString("00");

        // frames.Add(quadFrame);
        Renderer quadRenderer = frame.GetComponent<Renderer>();
        quadRenderer.enabled = true;
        frame.GetComponent<BoxCollider>().enabled = true;
        frame.GetComponent<MeshCollider>().enabled = true;
        if (quadRenderer != null)
        {
            quadRenderer.material.mainTexture = texture;
            Debug.Log("applied texture");
        }
        else
        {
            Debug.LogError("Quad prefab does not have a Renderer component.");
        }
    }
}
