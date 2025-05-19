using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public SaveData CurrentSaveData;

    public bool SaveSave(string path)
    {
        // Convert this GameState instance to a JSON string
        var josn = JsonUtility.ToJson(CurrentSaveData);

        // Save the converted JSON into the PlayerPrefs
        PlayerPrefs.SetString(path, josn);
        PlayerPrefs.Save();
        return true;
    }

    public bool LoadSave(string path)
    {
        if(!PlayerPrefs.HasKey(path))
        {
            Debug.LogError("No save of key: " + path);
            return false;
        }

        // Retrieve the saved JSON string from the player prefs
        string json = PlayerPrefs.GetString(path);

        // Deserialize the JSON string into a new GameState object and return it
        CurrentSaveData = ScriptableObject.CreateInstance<SaveData>();
        JsonUtility.FromJsonOverwrite(json, CurrentSaveData);
        return true;
    }

    public bool NewSave(string path)
    {
        if (PlayerPrefs.HasKey(path))
        {
            Debug.LogError("Save data already exists: " + path);
            return false;
        }
        CurrentSaveData = ScriptableObject.CreateInstance<SaveData>();
        return true;
    }

    public bool DeleteSave(string path)
    {
        if (!PlayerPrefs.HasKey(path))
        {
            Debug.LogError("No save of key: " + path);
            return false;
        }

        // Delete the save
        PlayerPrefs.DeleteKey(path);
        return true;
    }
}
