using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class ChallengeAccepting : MonoBehaviour
{
    private GameObject m_uiObject;
    private AnimalData m_currentAnimalData;
    private GameObject director;
    private Button goButton;
    public TextMeshProUGUI contentText;
    private AnimalData animalInf;
    private GameObject next;
    private Button nextInf;
    public GameObject diffChoose;

    // Start is called before the first frame update
    void Start()
    {
        m_uiObject = GameObject.Find("Canvas");
        if (m_uiObject == null)
        {
            Debug.LogError("UI not found.");
            return;
        }

        // Initialize the challenge
        InitChallenge();

        // Attach the button click events
        var buttons = m_uiObject.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            if (button.name == "Accept")
            {
                // Add the button click event
                button.onClick.AddListener(IntroducePlace);
            }
        }

        var saveData = SaveManager.Instance.CurrentSaveData;
        int id = saveData.CurrentCollectingID;
        MainManager.Instance.AnimalData.TryGetValue(id, out animalInf);

        director = m_uiObject.transform.Find("Director")?.gameObject;
        Debug.Log($"找到 Director? {(director != null)}");
        goButton = director.GetComponentInChildren<Button>();
        Debug.Log($"找到 GoButton? {(goButton != null)}  名称: {(goButton != null ? goButton.gameObject.name : "null")}");
        goButton.gameObject.SetActive(false);
        if (goButton != null)
        {
            goButton.onClick.AddListener(AcceptChallenge);
        }


        next = m_uiObject.transform.Find("Next")?.gameObject;
        nextInf = next.GetComponentInChildren<Button>();
        Debug.Log($"找到 Next? {(next != null)}");
        Debug.Log($"找到 NextInf Button? {(nextInf != null)}  名称: {(nextInf != null ? nextInf.gameObject.name : "null")}");
        nextInf.gameObject.SetActive(false);
        if (nextInf != null)
        {   
            nextInf.onClick.AddListener(IntroduceFood);
        }

            foreach (var button in buttons)
            {
                if (button.name == "Easy")
                {
                    // Add the button click event
                    button.onClick.AddListener(Easymode);
                }
                if (button.name == "Normal")
                {
                    // Add the button click event
                    button.onClick.AddListener(Normalmode);
                }
                if (button.name == "Hard")
                {
                    // Add the button click event
                    button.onClick.AddListener(Hardmode);
                }
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AcceptChallenge()
    {
        // Logic to accept the challenge
        Debug.Log("Challenge Accepted");
        MainManager.Instance.SaveCurrentSave(-1, 0, 0, ProgressEnum.HabitatChoosing);
        MainManager.Instance.NextLevel(ProgressEnum.HabitatChoosing);
    }

    private void Easymode()
    {
        diffChoose.SetActive(false);
        SaveManager.Instance.CurrentSaveData.SetAnimalDifficulty(1);
    }

    private void Normalmode()
    {
        diffChoose.SetActive(false);
        SaveManager.Instance.CurrentSaveData.SetAnimalDifficulty(2);
    }

    private void Hardmode()
    {
        diffChoose.SetActive(false);
        SaveManager.Instance.CurrentSaveData.SetAnimalDifficulty(3);
    }

    private void IntroducePlace()
    {
        Debug.Log("Introduce Place");
        nextInf.gameObject.SetActive(true);
        string name = animalInf.AnimalName;
        string habitat = $"{animalInf.AnimalHabitat}";
        string habitatConcat = " ";

        switch (habitat)
        {
            case "Ocean":
                habitatConcat = "the water is abundant";
                break;
        }

        string habitatDescription = string.Concat(name, " are creatures living in a place where ", habitatConcat, ".");
        contentText.text = habitatDescription;
    }

    private void IntroduceFood()
    {
        Debug.Log("Introduce Food");
        next.SetActive(false);
        goButton.gameObject.SetActive(true);

        string diet = $"{animalInf.AnimalDiet}";
        string dietConcat = " ";

        switch (diet)
        {
            case "Planktivore":
                dietConcat = "can float in water and move with the current";
                break;
        }

        string dietDescription = string.Concat("They feed on something that ", dietConcat, ".");
        contentText.text = dietDescription;
    }

    private void InitChallenge()
    {
        // Logic to show the challenge
        Debug.Log("Challenge Initialized");

        // remove hard difficulty animals
        var save = SaveManager.Instance.CurrentSaveData;
        var availableAnimals = MainManager.Instance.AnimalDataList.Where(a => save.GetAnimalDifficulty(a.AnimalID) != DifficultyEnum.Hard).ToList();
        bool isAllFinished = availableAnimals.Count == 0;
        var count = isAllFinished ? MainManager.Instance.AnimalDataList.Count : availableAnimals.Count;
        var randomIndex = UnityEngine.Random.Range(0, count);
        m_currentAnimalData = isAllFinished ? MainManager.Instance.AnimalDataList[randomIndex] : availableAnimals[randomIndex];

        var currentDifficulty = SaveManager.Instance.CurrentSaveData.GetAnimalDifficulty(m_currentAnimalData.AnimalID);
        var difficulty = currentDifficulty == DifficultyEnum.Unknown ? DifficultyEnum.Easy :
                        currentDifficulty == DifficultyEnum.Hard ? currentDifficulty : currentDifficulty + 1;

        // timer time
        var startTime = MainManager.Instance.GameDifficulties[(int)difficulty];
        
        Debug.Log($"Current Animal: {m_currentAnimalData.AnimalName}, Difficulty: {difficulty}");
        MainManager.Instance.SaveCurrentSave(m_currentAnimalData.AnimalID, difficulty, startTime, ProgressEnum.ChallengeAccepting);

        // TODO: Update the UI with the challenge details
    }
}
