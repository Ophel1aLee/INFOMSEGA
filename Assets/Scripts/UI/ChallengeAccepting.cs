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
    private List<AnimalData> m_currentAnimalDatas;
    private AnimalData m_currentAnimalData;
    private GameObject director;
    private Button goButton;
    public TextMeshProUGUI contentText;
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

        // Attach the button click events
        var buttons = m_uiObject.GetComponentsInChildren<Button>();
        var animalButtons = buttons.ToList().Where(b => b.name == "Accept").ToArray();

        // Initialize the challenge
        InitChallenge(animalButtons.Length);

        for (int i = 0; i < animalButtons.Length; i++)
        {
            var button = animalButtons[i];
            int index = i;
            // Add the button click event
            button.onClick.AddListener(() => SelectAnimal(index));

            var animalInf = m_currentAnimalDatas[index];
            // update the image and name of the animal
            var imageComponent = button.transform.Find("Image").GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = Resources.Load<Sprite>(animalInf.AnimalPicture);
            }
            var textComponent = button.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = animalInf.AnimalName;
            }
            Debug.Log($"Animal Name: {animalInf.AnimalName}, Picture: {animalInf.AnimalPicture}");
        }

        director = m_uiObject.transform.Find("Director")?.gameObject;
        goButton = director.GetComponentInChildren<Button>();
        goButton.gameObject.SetActive(false);
        if (goButton != null)
        {
            goButton.onClick.AddListener(AcceptChallenge);
        }

        next = m_uiObject.transform.Find("Next")?.gameObject;
        nextInf = next.GetComponentInChildren<Button>();
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

    private void SelectAnimal(int index)
    {
        // Logic to select the animal
        Debug.Log($"Animal {index} selected");

        m_currentAnimalData = m_currentAnimalDatas[index];
        Debug.Log($"Current Animal: {m_currentAnimalData.AnimalName}");
        MainManager.Instance.SaveCurrentSave(m_currentAnimalData.AnimalID, 0, 0, ProgressEnum.ChallengeAccepting);

        IntroducePlace();
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
        // SaveManager.Instance.CurrentSaveData.SetAnimalDifficulty(1);
        var startTime = MainManager.Instance.GameDifficulties[1];
        MainManager.Instance.SaveCurrentSave(-1, DifficultyEnum.Easy, startTime, ProgressEnum.ChallengeAccepting);
    }

    private void Normalmode()
    {
        diffChoose.SetActive(false);
        // SaveManager.Instance.CurrentSaveData.SetAnimalDifficulty(2);
        var startTime = MainManager.Instance.GameDifficulties[2];
        MainManager.Instance.SaveCurrentSave(-1, DifficultyEnum.Normal, startTime, ProgressEnum.ChallengeAccepting);
    }

    private void Hardmode()
    {
        diffChoose.SetActive(false);
        // SaveManager.Instance.CurrentSaveData.SetAnimalDifficulty(3);
        var startTime = MainManager.Instance.GameDifficulties[3];
        MainManager.Instance.SaveCurrentSave(-1, DifficultyEnum.Hard, startTime, ProgressEnum.ChallengeAccepting);
    }

    private void IntroducePlace()
    {
        Debug.Log("Introduce Place");
        nextInf.gameObject.SetActive(true);
        string name = m_currentAnimalData.AnimalName;
        string habitat = $"{m_currentAnimalData.AnimalHabitat}";
        string habitatConcat = " ";

        switch (habitat)
        {
            case "Ocean":
                habitatConcat = "the water is abundant";
                break;
            case "Savanna":
                habitatConcat = "the grass is tall and the trees are sparse";
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

        string diet = $"{m_currentAnimalData.AnimalDiet}";
        string dietConcat = " ";

        switch (diet)
        {
            case "Planktivore":
                dietConcat = "can float in water and move with the current";
                break;
            case "Herbivore":
                dietConcat = "is green";
                break;
        }

        string dietDescription = string.Concat("They feed on something that ", dietConcat, ".");
        contentText.text = dietDescription;
    }

    private void InitChallenge(int number)
    {
        // Logic to show the challenge
        Debug.Log("Challenge Initialized");

        // remove hard difficulty animals
        var save = SaveManager.Instance.CurrentSaveData;
        var availableAnimals = MainManager.Instance.AnimalDataList.ToList().Where(a => save.GetAnimalDifficulty(a.AnimalID) != DifficultyEnum.Hard).ToList();

        var animalPool = availableAnimals.Count >= number ? availableAnimals : MainManager.Instance.AnimalDataList.ToList();
        for (int i = 0; i < number; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, animalPool.Count);
            m_currentAnimalDatas ??= new List<AnimalData>();
            m_currentAnimalDatas.Add(animalPool[randomIndex]);
            animalPool.RemoveAt(randomIndex);
        }
        // TODO: Update the UI with the challenge details
    }
}
