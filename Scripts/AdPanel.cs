using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AdPanel : MonoBehaviour
{
    public int rowIndex; // Set this for each panel in the Inspector
    public string url;
    private Renderer panelRenderer;

    void Start()
    {
        panelRenderer = GetComponent<Renderer>();
    }

    public void SetAdURL(string url)
    {
        this.url = url;
        StartCoroutine(DownloadAndSetAd(this.url));
    }

    IEnumerator DownloadAndSetAd(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error downloading image: " + request.error);
        }
        else
        {
            Texture2D adTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            panelRenderer.material.mainTexture = adTexture; // Apply the downloaded texture to the 3D panel
        }
    }
}
