using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnimalFeeding : MonoBehaviour
{
    private GameObject m_uiObject;
    private List<DietEnum> m_diets = new List<DietEnum>();
    private DietEnum m_correctDiet;
    private int m_currentAnimalID;
    private Button hintButton;
    private GameObject hint;

    // Start is called before the first frame update
    void Start()
    {
        m_uiObject = GameObject.Find("Canvas");
        if (m_uiObject == null)
        {
            Debug.LogError("UI not found.");
            return;
        }

        hint = m_uiObject.transform.Find("Hint")?.gameObject;
        hintButton = hint.GetComponentInChildren<Button>();
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(() =>
            {
                hint.SetActive(false);
            });
        }

        // Attach the button click events
        var buttons = m_uiObject.GetComponentsInChildren<Button>();
        var foodButtons = buttons.Where(b => b.name == "Food").ToArray();
        var save = SaveManager.Instance.CurrentSaveData;
        m_currentAnimalID = save.CurrentCollectingID;
        m_correctDiet = MainManager.Instance.AnimalData[save.CurrentCollectingID].AnimalDiet;
        if (save.CurrentStatus != null)
        {
            m_diets = save.CurrentStatus.Select((i) => (DietEnum)Enum.ToObject(typeof(DietEnum), i)).ToList();
        }
        else
        {
            GenerateDiets(foodButtons.Length);
        }

        for (int i = 0; i < foodButtons.Length; i++)
        {
            var button = foodButtons[i];
            if (button.name == "Food")
            {
                int index = i;
                button.onClick.AddListener(() => FoodChoose(m_diets[index]));
                // Adjust the button text
                var text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = Enum.GetName(typeof(DietEnum), m_diets[index]);
                }
            }
        }

        // TODO: Update the UI with the Food resources
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FoodChoose(DietEnum diet)
    {
        // Logic to handle food choice
        Debug.Log($"Diet {diet} chosen.");
        if (diet == m_correctDiet)
        {
            // Correct diet chosen
            SaveManager.Instance.CurrentSaveData.CollectionIDs = m_currentAnimalID;
            // Remove the status list
            MainManager.Instance.SaveCurrentSave(-1, ProgressEnum.AnimalFeeding);
            MainManager.Instance.NextLevel(ProgressEnum.ChallengeAccepting);
            // TODO: Show feedback to the player
        }
        else
        {
            // Incorrect diet chosen
            hint.SetActive(true);
            Debug.Log("Incorrect diet chosen.");
            // TODO: Show feedback to the player
        }
    }

    private void GenerateDiets(int count)
    {
        // Generate a list of random diets
        m_diets.Clear();
        m_diets.Add(m_correctDiet);

        // Randomly generate diets
        var dietPool = ((IEnumerable<DietEnum>)Enum.GetValues(typeof(DietEnum)))
            .Where(d => d != DietEnum.Unknown && d != m_correctDiet)
            .ToList();
        for (int i = 0; i < count - 1; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, dietPool.Count);
            m_diets.Add(dietPool[randomIndex]);
            dietPool.RemoveAt(randomIndex);
        }

        // Shuffle the list
        for (int i = 0; i < m_diets.Count; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, m_diets.Count);
            (m_diets[i], m_diets[randomIndex]) = (m_diets[randomIndex], m_diets[i]);
        }

        // Save the current foods
        MainManager.Instance.SaveCurrentSave(-1, ProgressEnum.AnimalFeeding, new List<int>(m_diets.Select(d => (int)d)));
    }
}
