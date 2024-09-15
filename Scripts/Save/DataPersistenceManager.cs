using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// Manages data persistence for the game, including saving and loading game data to and from files.
/// </summary>
public class DataPersistenceManager : MonoBehaviour
{
    /// <summary>
    /// If true, disables data persistence functionality.
    /// </summary>
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    /// <summary>
    /// If true, initializes data if no data is found for debugging purposes.
    /// </summary>
    [SerializeField] private bool initializeDataIfNull = false;
    /// <summary>
    /// If true, overrides the selected profile ID with a test ID for debugging.
    /// </summary>
    [SerializeField] private bool overrideSelectedProfileId = false;
    /// <summary>
    /// The test selected profile ID used for debugging if override is enabled.
    /// </summary>
    [SerializeField] private string testSelectedProfileId = "test";
    /// <summary>
    /// The file name for storing game data.
    /// </summary>
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    /// <summary>
    /// If true, enables encryption for stored game data.
    /// </summary>
    [SerializeField] private bool useEncryption;
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    /// <summary>
    /// The currently selected profile ID for saving and loading game data.
    /// </summary>
    [SerializeField] private string selectedProfileId = "";
    /// <summary>
    /// Maximum number of autosave slots
    /// </summary>
    [SerializeField] private int maxAutoSaveSlots = 3;
    private bool loadNewGame;
    public bool saveOnExit;
    /// <summary>
    /// The singleton instance of the DataPersistenceManager.
    /// </summary>
    public static DataPersistenceManager instance { get; private set; }
    private void Awake()
    {
        // Ensure only one instance of DataPersistenceManager exists
        if (instance != null)
        {
            Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        // Initialize file data handler
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        // Get the most recently updated profile ID from saved data
        // this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        // Override selected profile ID for debugging if needed
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
        }

        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

    }
    private void OnApplicationQuit()
    {
        if (saveOnExit) this.SaveGame();
    }
    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        LoadGame();
    }
    /// <summary>
    /// Starts a new game with default data.
    /// </summary>
    public void NewGame()
    {
        loadNewGame = true;
        this.gameData = new GameData();
        Debug.Log("New game started");
    }
    /// <summary>
    /// Loads game data from file and distributes it to relevant components.
    /// </summary>
    public void LoadGame()
    {
        if (disableDataPersistence)
        {
            return;
        }

        if (loadNewGame)
        {
            this.gameData = new GameData();
            loadNewGame = false;
        }
        else
            this.gameData = dataHandler.Load(selectedProfileId);
        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }
    /// <summary>
    /// Saves current game data to file.
    /// </summary>
    public void SaveGame()
    {
        if (disableDataPersistence)
        {
            return;
        }
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }
        gameData.lastUpdated = System.DateTime.Now.ToBinary();
        dataHandler.Save(gameData, selectedProfileId);
    }
    /// <summary>
    /// Find all the game objects that implement the lDataPersistence interface
    /// </summary>
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
    /// <summary>
    /// Checks if game data exists.
    /// </summary>
    public bool HasGameData()
    {
        return gameData != null;
    }
    /// <summary>
    /// Retrieves game data for all profiles.
    /// </summary>
    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }
    /// <summary>
    /// Automatically saves the game if data persistence is enabled.
    /// </summary>
    public void AutoSaveGame()
    {
        if (disableDataPersistence)
        {
            return;
        }
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }
        gameData.lastUpdated = System.DateTime.Now.ToBinary();
        dataHandler.AutoSave(maxAutoSaveSlots, this.gameData);
        Debug.Log("saved");
    }

}
