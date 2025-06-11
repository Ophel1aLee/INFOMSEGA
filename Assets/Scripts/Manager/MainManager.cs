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
    public List<float> GameDifficulties = new List<float> { 0.0f, 0.0f, 300.0f, 180.0f }; // in seconds

    private GameObject m_mainMenu;
    private SceneLoader m_sceneLoader;
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

        m_sceneLoader = FindObjectOfType<SceneLoader>();
        if (m_sceneLoader == null)
        {
            Debug.LogError("Scene Loader not found");
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

        var resetButton = m_mainMenu.transform.Find("Restart").GetComponent<UnityEngine.UI.Button>();
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetProgress);
            resetButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Reset!";
            Debug.Log("Reset");
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

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        m_sceneLoader.FadeIn();
        yield return new WaitForSeconds(1f);

        var operation = Addressables.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        yield return operation;

        if (operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"{sceneName} Scene Loaded");
            m_sceneLoader = FindObjectOfType<SceneLoader>();
            if (m_sceneLoader != null)
            {
                m_sceneLoader.FadeOut();
            }
            OnSceneLoad?.Invoke();
        }
        else
        {
            Debug.LogError($"Failed to load Scene {sceneName}");
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
        StartCoroutine(LoadSceneWithTransition(sceneName));
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
        StartCoroutine(LoadSceneWithTransition(sceneName));
    }

    private void ResetProgress()
    {
        var saveMgr = SaveManager.Instance;
        if (PlayerPrefs.HasKey(GameSavePath) && saveMgr.DeleteSave(GameSavePath))
        {
            Debug.Log("clear progress");
            var playButton = m_mainMenu.transform.Find("Play").GetComponent<UnityEngine.UI.Button>();
            playButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "New Game";
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(Play);
            saveMgr.CurrentSaveData = ScriptableObject.CreateInstance<SaveData>();
        }
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

    public void SaveCurrentSave(int animalID=0, DifficultyEnum difficulty=DifficultyEnum.Easy, float time=0.0f, ProgressEnum progress=0, List<int> currentStatus=null)
    {
        Debug.Log("Saving Current Save");
        var saveMgr = SaveManager.Instance;
        // Save the current save data to the game save fild
        saveMgr.CurrentSaveData.CurrentCollectingID = animalID > 0 ? animalID : saveMgr.CurrentSaveData.CurrentCollectingID;
        saveMgr.CurrentSaveData.CurrentDifficulty = difficulty > 0 ? difficulty : saveMgr.CurrentSaveData.CurrentDifficulty;
        saveMgr.CurrentSaveData.CurrentTime = saveMgr.CurrentSaveData.CurrentDifficulty > DifficultyEnum.Easy ?
                                            time > 0 ? time : saveMgr.CurrentSaveData.CurrentTime : 0.0f;
        saveMgr.CurrentSaveData.Progress = progress > 0 ? progress : saveMgr.CurrentSaveData.Progress;
        saveMgr.CurrentSaveData.CurrentStatus = currentStatus;
        saveMgr.SaveSave(GameSavePath);
    }

    public void NextLevel(ProgressEnum progress)
    {
        Debug.Log("Next Level");
        var saveMgr = SaveManager.Instance;

        // Load the next level data
        if (progress == ProgressEnum.Unknown)
        {
            StartCoroutine(LoadSceneWithTransition("MainMenu"));
            return;
        }

        var progressName = Enum.GetName(typeof(ProgressEnum), progress);
        var progressDataType = Type.GetType($"{progressName}Data");
        saveMgr.CurrentSaveData.Progress = progress;
        
        // Load the game scene according to the game save
        StartCoroutine(LoadSceneWithTransition(progressName));
    }
}
