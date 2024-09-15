using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
/// <summary>
/// Handles loading and saving game data to and from files.
/// </summary>
public class FileDataHandler
{
    // Directory path where data files are stored
    private string dataDirPath = "";
    // Name of the data file
    private string dataFileName = "";
    // Flag to indicate whether encryption is enabled
    private bool useEncryption = false;
    // Encryption code word for XOR encryption
    private readonly string encryptionCodeWord = "word";
    /// <summary>
    /// Initializes a new instance of the FileDataHandler class with the specified parameters.
    /// </summary>
    /// <param name="dataDirPath">The directory path where data files are stored.</param>
    /// <param name="dataFileName">The name of the data file.</param>
    /// <param name="useEncryption">If true, enables encryption for stored data.</param>
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }
    /// <summary>
    /// Loads game data from a file for the specified profile ID.
    /// </summary>
    /// <param name="profileId">The profile ID to load game data for.</param>
    /// <returns>The loaded GameData object.</returns>
    public GameData Load(string profileId)
    {
        if (profileId == null)
        {
            return null;
        }
        // Construct full path to the data file
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // Read data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                // Decrypt data if encryption is enabled
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Deserialize JSON data into GameData object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }
    /// <summary>
    /// Get the path for a specific save 
    /// </summary>
    /// <param name="profileId">The profile id of the save.</param>
    public string GetSavePath(string profileId)
    {
        return Path.Combine(dataDirPath, profileId, dataFileName);
    }
    /// <summary>
    /// Saves game data to a file for the specified profile ID.
    /// </summary>
    /// <param name="data">The GameData object to save.</param>
    /// <param name="profileId">The profile ID to save game data for.</param>
    public void Save(GameData data, string profileId)
    {
        if (profileId == null)
        {
            return;
        }
        // Construct full path to the data file
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        try
        {
            // Create directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            // Serialize GameData object into JSON
            string dataToStore = JsonUtility.ToJson(data, true);
            // Encrypt data if encryption is enabled
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            // Write data to the file
            using FileStream stream = new FileStream(fullPath, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
    /// <summary>
    /// Automatically saves the game data.
    /// </summary>
    /// <param name="saveData">The game data to be saved.</param>
    public void AutoSave(int maxAutoSaveSlots, GameData data)
    {
        // Shift existing save files
        for (int i = maxAutoSaveSlots - 1; i > 0; i--)
        {
            string oldPath = GetSavePath((i - 1).ToString());
            string newPath = GetSavePath(i.ToString());
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }

            if (File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
            }
        }

        // Save new data to the first slot
        string saveId = "0";
        Save(data, saveId);
    }
    /// <summary>
    /// Loads game data for all profiles.
    /// </summary>
    /// <returns>A dictionary containing profile IDs as keys and corresponding GameData objects as values.</returns>
    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();
        // Get all directories in the data directory
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            // Construct full path to the data file for each profile
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                // Skip if data file does not exist
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: "
                    + profileId);
                continue;
            }
            // Load game data for the profile
            GameData profileData = Load(profileId);
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong. ProfileId: " + profileId);
            }
        }
        return profileDictionary;
    }
    /// <summary>
    /// Retrieves the profile ID of the most recently updated profile.
    /// </summary>
    /// <returns>The profile ID of the most recently updated profile.</returns>
    public string GetMostRecentlyUpdatedProfileId()
    {
        string mostRecentProfileId = null;
        // Load game data for all profiles
        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            if (gameData == null)
            {
                continue;
            }
            // Compare lastUpdated field of each profile to find the most recent one
            if (mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);
                if (newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }
        return mostRecentProfileId;
    }
    // Performs XOR encryption/decryption on the data using the encryptionCodeWord
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }
}