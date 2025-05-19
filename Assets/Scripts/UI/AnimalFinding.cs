using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalFinding : MonoBehaviour
{
    private GameObject m_uiObject;
    private Dictionary<int, AnimalData> m_animalDatas = new Dictionary<int, AnimalData>();
    private List<int> m_animalIDs = new List<int>();
    private int m_correctAnimalID;
    private HabitatEnum m_currentHabitat;

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
        var buttons = m_uiObject.GetComponentsInChildren<UnityEngine.UI.Button>();
        var save = SaveManager.Instance.CurrentSaveData;
        var correctAnimal = MainManager.Instance.AnimalData[save.CurrentCollectingID];
        m_correctAnimalID = correctAnimal.AnimalID;
        m_currentHabitat = correctAnimal.AnimalHabitat;
        if (save.CurrentStatus != null)
        {
            m_animalIDs = save.CurrentStatus;
            foreach (var id in m_animalIDs)
            {
                MainManager.Instance.AnimalData.TryGetValue(id, out var animalData);
                if (animalData != null)
                {
                    m_animalDatas.Add(id, animalData);
                }
            }
        }
        else
        {
            GenerateAnimals(buttons.Length);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            if (button.name == "Animal")
            {
                int index = i;
                button.onClick.AddListener(() => AnimalFind(m_animalIDs[index]));
                // Adjust the button text
                var text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = m_animalDatas[m_animalIDs[index]].AnimalName;
                }
            }
        }

        // TODO: Update the UI with the Animal resources
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void AnimalFind(int animalID)
    {
        // Logic to handle animal finding
        Debug.Log($"Animal {m_animalDatas[animalID].AnimalName} found.");
        if (animalID == m_correctAnimalID)
        {
            // Correct animal found
            // Remove the status list
            MainManager.Instance.SaveCurrentSave(-1, ProgressEnum.AnimalFinding);
            MainManager.Instance.NextLevel(ProgressEnum.AnimalFeeding);
            // TODO: Show feedback to the player
        }
        else
        {
            // Incorrect animal found
            Debug.Log("Incorrect animal.");
            // TODO: Show feedback to the player
        }
    }

    private void GenerateAnimals(int count)
    {
        // Generate a list of animals
        m_animalDatas.Clear();
        m_animalDatas.Add(m_correctAnimalID, MainManager.Instance.AnimalData[m_correctAnimalID]);

        var animalPool = MainManager.Instance.AnimalDataList
            .FindAll(animal => animal.AnimalHabitat == m_currentHabitat && animal.AnimalID != m_correctAnimalID);
        for (int i = 0; i < count - 1; i++)
        {
            var randomIndex = Random.Range(0, animalPool.Count);
            var animalData = animalPool[randomIndex];
            m_animalDatas.Add(animalData.AnimalID, animalData);
            animalPool.RemoveAt(randomIndex);
        }

        // Shuffle the list
        m_animalIDs = new List<int>(m_animalDatas.Keys);
        for (int i = 0; i < m_animalIDs.Count; i++)
        {
            var randomIndex = Random.Range(0, m_animalIDs.Count);
            (m_animalIDs[i], m_animalIDs[randomIndex]) = (m_animalIDs[randomIndex], m_animalIDs[i]);
        }

        // Save the current annimals
        MainManager.Instance.SaveCurrentSave(m_correctAnimalID, ProgressEnum.AnimalFinding, new List<int>(m_animalIDs));
    }
}
