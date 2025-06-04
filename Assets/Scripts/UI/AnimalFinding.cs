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

    private Timer m_timer;
    private float m_timerTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_uiObject = GameObject.Find("Canvas");
        if (m_uiObject == null)
        {
            Debug.LogError("UI not found.");
            return;
        }


        var save = SaveManager.Instance.CurrentSaveData;
        var correctAnimal = MainManager.Instance.AnimalData[save.CurrentCollectingID];
        m_currentHabitat = correctAnimal.AnimalHabitat;

        string habitatName = m_currentHabitat.ToString().ToLower();

        // var bgTransform = m_uiObject.transform.Find(habitatName);
        // if (bgTransform != null)
        // {
        //     bgTransform.gameObject.SetActive(true);
        // }
        // else
        // {
        //     Debug.LogWarning($"Background for habitat \"{habitatName}\" not found under Canvas.");
        // }
        // directly find CameraDrag
        var cameraDrags = m_uiObject.GetComponentsInChildren<CameraDrag>();
        GameObject bgObject = null;
        foreach (var cameraDrag in cameraDrags)
        {
            if (cameraDrag.gameObject.name == habitatName)
            {
                cameraDrag.gameObject.SetActive(true);
                bgObject = cameraDrag.gameObject;
            }
            else
            {
                cameraDrag.gameObject.SetActive(false);
            }
        }
        // ————————————

        // Attach the button click events
        var buttons = bgObject.GetComponentsInChildren<UnityEngine.UI.Button>();
        var animalButtons = buttons.Where(b => b.name == "Animal").ToArray();

        m_correctAnimalID = correctAnimal.AnimalID;
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
            GenerateAnimals(animalButtons.Length);
        }

        // 给每个 Animal 按钮绑定对应的 animalID 及文字
        for (int i = 0; i < animalButtons.Length; i++)
        {
            var button = animalButtons[i];
            int animalID = m_animalIDs[i];
            button.onClick.AddListener(() => AnimalFind(animalID));

            var text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
                text.text = m_animalDatas[animalID].AnimalName;
        }

        // Start the timer
        m_timer = save.CurrentDifficulty > DifficultyEnum.Easy
                  ? m_uiObject.GetComponentInChildren<Timer>()
                  : null;
        if (m_timer != null)
        {
            m_timer.OnTimerStop += (time) =>
            {
                Debug.Log($"Timer stopped at {time} seconds.");
                m_timerTime = time;
            };
            m_timer.StartTimer(save.CurrentTime);
        }
        else
        {
            Debug.Log("Timer not found in the UI.");
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
            // Stop the timer
            m_timer?.StopTimer();
            // Remove the status list
            MainManager.Instance.SaveCurrentSave(-1, 0, m_timerTime, ProgressEnum.AnimalFeeding);
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

        // Save the current animals
        MainManager.Instance.SaveCurrentSave(-1, 0, 0, ProgressEnum.AnimalFinding, new List<int>(m_animalIDs));
    }
}
