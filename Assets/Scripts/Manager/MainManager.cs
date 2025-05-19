using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MainManager : Singleton<MainManager>
{
    public delegate void OnSceneLoadDelegate();
    public event OnSceneLoadDelegate OnSceneLoad;

    public string GameSavePath = "GameSave";

    private GameObject m_mainMenu;
    private List<AnimalData> m_animalDataList;
    public List<AnimalData> AnimalDataList
    {
        get { return m_animalDataList; }
    }
    private Dictionary<int, AnimalData> m_animalDataDict;
    public Dictionary<int, AnimalData> AnimalData
    {
        get { return m_animalDataDict; }
    }

    void Start()
    {
        m_mainMenu = GameObject.Find("Canvas");
        if (m_mainMenu == null)
        {
            Debug.LogError("Main Menu not found");
            return;
        }

        // Attach the button click events
        var playButton = m_mainMenu.transform.Find("Play").GetComponent<UnityEngine.UI.Button>();
        // Find if there is a save, if yes, change the button to New Game
        if (PlayerPrefs.HasKey(GameSavePath))
        {
            playButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Continue";
            playButton.onClick.AddListener(NewPlay);
            Debug.Log("Game Save Found");
        }
        else
        {
            playButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "New Game";
            playButton.onClick.AddListener(Play);
            Debug.Log("No Game Save Found");
        }

        var collectionButton = m_mainMenu.transform.Find("Collection").GetComponent<UnityEngine.UI.Button>();
        var quitButton = m_mainMenu.transform.Find("Quit").GetComponent<UnityEngine.UI.Button>();
        quitButton.onClick.AddListener(Quit);

        // Load all the scriptable objects of type AnimalData
        m_animalDataList = new List<AnimalData>(
            Addressables.LoadAssetsAsync<AnimalData>(
                "Animals",
                null
            ).WaitForCompletion()
        );
        m_animalDataDict = new Dictionary<int, AnimalData>();
        foreach (var animalData in m_animalDataList)
        {
            m_animalDataDict.Add(animalData.AnimalID, animalData);
        }
    }

    public void Play()
    {
        Debug.Log("Game Start");
        var saveMgr = SaveManager.Instance;
        // Find if there is a save, if yes, load the save
        if (PlayerPrefs.HasKey(GameSavePath))
        {
            saveMgr.LoadSave(GameSavePath);
        }
        // If no save
        else
        {
            NewPlay();
            return;
        }

        // Load the game scene according to the game save
        var data = saveMgr.CurrentSaveData.Progress;
        var sceneName = Enum.GetName(typeof(ProgressEnum), data);
        Addressables.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single).Completed += (operation) =>
        {
            if (operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{sceneName} Scene Loaded");
                LoadCurrentSave();
                OnSceneLoad?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed to load Scene {sceneName}");
            }
        };
    }

    public void NewPlay()
    {
        Debug.Log("New Game");
        var saveMgr = SaveManager.Instance;
        // Find if there is a save, if yes, load the save
        if (PlayerPrefs.HasKey(GameSavePath))
        {
            Play();
            return;
        }
        // If no save
        else
        {
            saveMgr.NewSave(GameSavePath);

            // Initial data
            saveMgr.CurrentSaveData.Progress = ProgressEnum.ChallengeAccepting;
        }
        
        // Load the game scene according to the game save
        var data = saveMgr.CurrentSaveData.Progress;
        var sceneName = Enum.GetName(typeof(ProgressEnum), data);
        Addressables.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single).Completed += (operation) =>
        {
            if (operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{sceneName} Scene Loaded");
                // LoadCurrentSave();    no need to load cuz it's a new game
                OnSceneLoad?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed to load Scene {sceneName}");
            }
        };
    }

    public void ViewCollection()
    {
        Debug.Log("View Collection");
    }

    public void Quit()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }

    public void LoadCurrentSave()
    {
        Debug.Log("Loading Current Save");
        var saveMgr = SaveManager.Instance;
        // Load the current save data to the scene
    }

    public void SaveCurrentSave(int animalID=0, ProgressEnum progress=0, List<int> currentStatus=null)
    {
        Debug.Log("Saving Current Save");
        var saveMgr = SaveManager.Instance;
        // Save the current save data to the game save fild
        saveMgr.CurrentSaveData.CurrentCollectingID = animalID > 0 ? animalID : saveMgr.CurrentSaveData.CurrentCollectingID;
        saveMgr.CurrentSaveData.Progress = progress > 0 ? progress : saveMgr.CurrentSaveData.Progress;
        saveMgr.CurrentSaveData.CurrentStatus = currentStatus;
        saveMgr.SaveSave(GameSavePath);
    }

    public void NextLevel(ProgressEnum progress)
    {
        Debug.Log("Next Level");
        var saveMgr = SaveManager.Instance;

        // Load the next level data
        var progressName = Enum.GetName(typeof(ProgressEnum), progress);
        var progressDataType = Type.GetType($"{progressName}Data");
        saveMgr.CurrentSaveData.Progress = progress;
        
        // Load the game scene according to the game save
        var data = saveMgr.CurrentSaveData.Progress;
        Addressables.LoadSceneAsync(progressName, UnityEngine.SceneManagement.LoadSceneMode.Single).Completed += (operation) =>
        {
            if (operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"{progressName} Scene Loaded");
                // LoadCurrentSave();    no need to load cuz it's a new scene
                OnSceneLoad?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed to load Scene {progressName}");
            }
        };
    }
}
