using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        var buttons = m_uiObject.GetComponentsInChildren<UnityEngine.UI.Button>();
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AcceptChallenge()
    {
        // Logic to accept the challenge
        Debug.Log("Challenge Accepted");
        
        MainManager.Instance.NextLevel(ProgressEnum.HabitatChoosing);
    }

    private void IntroducePlace()
    {
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
        var randomIndex = UnityEngine.Random.Range(0, MainManager.Instance.AnimalDataList.Count);
        m_currentAnimalData = MainManager.Instance.AnimalDataList[randomIndex];
        MainManager.Instance.SaveCurrentSave(m_currentAnimalData.AnimalID, ProgressEnum.ChallengeAccepting);

        // TODO: Update the UI with the challenge details
    }
}
