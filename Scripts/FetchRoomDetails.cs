using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class RoomDetails
{
    public string username;
    public string token_id;

}

public class FetchRoomDetails : MonoBehaviour
{
    public string url = "https://fleastore.in/htm/accounts.json";
    public List<RoomDetails> roomDetailsList;
    public GameObject roomItemPrefab;
    public Transform roomItemParent;
    public Button inputButton;
    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRoomDetails());
        inputButton.onClick.AddListener(() =>
        {
            OnRoomButtonClicked("", inputField.text.ToUpper());
        });
    }

    IEnumerator GetRoomDetails()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            // Parse JSON array
            string jsonResponse = www.downloadHandler.text;
            roomDetailsList = JsonUtility.FromJson<RoomDetailsList>("{\"rooms\":" + jsonResponse + "}").rooms;

            // Display room details
            foreach (RoomDetails room in roomDetailsList)
            {
                CreateRoomItem(room);
            }
        }
    }
    void CreateRoomItem(RoomDetails room)
    {
        // Instantiate the prefab
        GameObject roomItem = Instantiate(roomItemPrefab, roomItemParent);

        // Get the UI components (adjust to your actual prefab structure)
        TMP_Text usernameText = roomItem.transform.Find("username").GetComponent<TMP_Text>();
        TMP_Text codeText = roomItem.transform.Find("code").GetComponent<TMP_Text>();
        Button roomButton = roomItem.transform.Find("EnterButton").GetComponent<Button>();

        // Update the UI components with data
        usernameText.text = room.username;
        codeText.text = room.token_id;

        // Add a button listener to log username and code
        roomButton.onClick.AddListener(() => OnRoomButtonClicked(room.username, room.token_id));
    }

    void OnRoomButtonClicked(string username, string code)
    {
        OpenPrivateRoom.Instance.SetPrivateDetails(username, code);
    }
}

[System.Serializable]
public class RoomDetailsList
{
    public List<RoomDetails> rooms;
}
