using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class HabitatChoosing : MonoBehaviour
{
    private GameObject m_uiObject;
    private List<HabitatEnum> m_habitats = new List<HabitatEnum>();
    private HabitatEnum m_correctHabitat;
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
        var habitatButtons = buttons.Where(b => b.name == "Habitat").ToArray();
        var save = SaveManager.Instance.CurrentSaveData;
        m_correctHabitat = MainManager.Instance.AnimalData[save.CurrentCollectingID].AnimalHabitat;
        if (save.CurrentStatus != null)
        {
            m_habitats = save.CurrentStatus.Select((i) => (HabitatEnum)Enum.ToObject(typeof(HabitatEnum), i)).ToList();
        }
        else
        {
            GenerateHabitats(habitatButtons.Length);
        }

        for (int i = 0; i < habitatButtons.Length; i++)
        {
            var button = habitatButtons[i];
            int index = i;
            button.onClick.AddListener(() => HabitatChoose(m_habitats[index]));
            // Adjust the button text
            var text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
            {
                text.text = Enum.GetName(typeof(HabitatEnum), m_habitats[index]);
            }

        }

        // TODO: Update the UI with the Habitat resources
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void HabitatChoose(HabitatEnum habitat)
    {
        // Logic to handle habitat choice
        Debug.Log($"Habitat {habitat} chosen.");
        if (habitat == m_correctHabitat)
        {
            // Correct habitat chosen
            // Remove the status list
            MainManager.Instance.SaveCurrentSave(-1, ProgressEnum.HabitatChoosing);
            MainManager.Instance.NextLevel(ProgressEnum.AnimalFinding);
            // TODO: Show feedback to the player
        }
        else
        {
            // Incorrect habitat chosen
            hint.SetActive(true);
            Debug.Log("Incorrect habitat.");
            // TODO: Show feedback to the player
        }
    }

    private void GenerateHabitats(int number)
    {
        // Generate a list of habitats
        m_habitats.Clear();
        m_habitats.Add(m_correctHabitat);

        // Randomly generate habitats
        var habitatPool = ((IEnumerable<HabitatEnum>)Enum.GetValues(typeof(HabitatEnum)))
            .Where(h => h != HabitatEnum.Unknown && h != m_correctHabitat)
            .ToList();
        for (int i = 0; i < number - 1; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, habitatPool.Count);
            m_habitats.Add(habitatPool[randomIndex]);
            habitatPool.RemoveAt(randomIndex);
        }

        // Shuffle the list
        for (int i = 0; i < m_habitats.Count; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, m_habitats.Count);
            (m_habitats[randomIndex], m_habitats[i]) = (m_habitats[i], m_habitats[randomIndex]);
        }

        // Save the current habitats
        MainManager.Instance.SaveCurrentSave(-1, ProgressEnum.HabitatChoosing, new List<int>(m_habitats.Select(h => (int)h)));
    }
}
