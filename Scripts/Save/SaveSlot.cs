using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a save slot in the save slots menu.
/// </summary>
public class SaveSlot : MonoBehaviour
{
    // Profile ID associated with this save slot
    [Header("Profile")]
    [SerializeField] private string profileId = "";
    // References to UI elements for displaying save slot content
    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI sceneNameText;
    [SerializeField] private GameData data;
    [SerializeField] private TextMeshProUGUI timeText;
    private Button saveSlotButton;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Get the Button component attached to this object
        saveSlotButton = this.GetComponent<Button>();
    }
    // Sets the data for this save slot
    public void SetData(GameData data)
    {
        // If there's no data for this profileId
        if (data == null)
        {
            // Show the "No Data" content and hide the "Has Data" content
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            this.data = null;
        }
        // If there is data for this profileId
        else
        {
            // Show the "Has Data" content and hide the "No Data" content
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            this.data = data;
        }
    }
    // Returns the profile ID associated with this save slot
    public string GetProfileId()
    {
        return this.profileId;
    }
    // Sets the interactability of the save slot button
    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }
    //Get the scene name of current slot
}
