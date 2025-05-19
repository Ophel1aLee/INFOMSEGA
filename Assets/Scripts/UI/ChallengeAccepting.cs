using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeAccepting : MonoBehaviour
{
    private GameObject m_uiObject;
    private AnimalData m_currentAnimalData;

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
                button.onClick.AddListener(AcceptChallenge);
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
        
        MainManager.Instance.NextLevel(ProgressEnum.HabitatChoosing);
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
