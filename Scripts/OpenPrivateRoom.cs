using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class OpenPrivateRoom : MonoBehaviour
{
    string tokenId;
    string userName;
    public string userSendUrl;
    public string tokenFetchUrl;
    public string tokenSendUrl;
    public string worldFetchurl;
    public string worldSendUrl;
    public static OpenPrivateRoom Instance;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    public void SetPrivateDetails(string userName, string tokenId, bool transition = true)
    {
        this.tokenId = tokenId;
        this.userName = userName;
        //Set urls
        userSendUrl = $"https://fleastore.in/htm/accounts/{this.userName}/api.php";
        tokenSendUrl = $"https://fleastore.in/htm/accounts/{this.tokenId}/api.php";
        tokenFetchUrl = $"https://fleastore.in/htm/accounts/{this.tokenId}/data.json";
        URLManager.instance.SetUrls(this.tokenFetchUrl, this.tokenSendUrl, this.userSendUrl);
        if (transition) URLManager.instance.SetTransition();
    }
    public void SetWorldDetails(string userName, bool transition = true)
    {
        this.userName = userName;
        userSendUrl = $"https://fleastore.in/htm/accounts/{this.userName}/api.php";
        URLManager.instance.SetUrls(this.worldFetchurl, this.worldSendUrl, this.userSendUrl);
        URLManager.instance.globalMode = true;
        if (transition) URLManager.instance.SetTransition();
    }

}
