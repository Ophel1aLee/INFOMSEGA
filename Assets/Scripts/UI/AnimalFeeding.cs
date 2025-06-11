using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AnimalFeeding : MonoBehaviour
{
    private GameObject m_uiObject;
    private List<DietEnum> m_diets = new List<DietEnum>();
    private DietEnum m_correctDiet;
    private int m_currentAnimalID;

    private GameObject hint;
    private Button hintButton;

    // Added: GameObject and button for correct feeding hint
    private GameObject correctHint;
    private Button correctHintButton;

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

        hint = m_uiObject.transform.Find("Hint")?.gameObject;
        hintButton = hint?.GetComponentInChildren<Button>();
        if (hintButton != null)
        {
            hintButton.onClick.AddListener(() =>
            {
                hint.SetActive(false);
            });
        }
        hint?.SetActive(false);

        // Added: Initialize correctHint
        correctHint = m_uiObject.transform.Find("CorrectHint")?.gameObject;
        correctHintButton = correctHint?.GetComponentInChildren<Button>();
        if (correctHintButton != null)
        {
            correctHintButton.onClick.AddListener(() =>
            {
                correctHint.SetActive(false);
                SaveManager.Instance.CurrentSaveData.CollectionIDs = m_currentAnimalID;
                SaveManager.Instance.CurrentSaveData.SetAnimalDifficulty(m_currentAnimalID);
                MainManager.Instance.SaveCurrentSave(-1, 0, m_timerTime, ProgressEnum.ChallengeAccepting);
                MainManager.Instance.NextLevel(ProgressEnum.Unknown);
            });
        }
        correctHint?.SetActive(false);

        // Attach the button click events
        var buttons = m_uiObject.GetComponentsInChildren<Button>();
        var foodButtons = buttons.Where(b => b.name == "Food").ToArray();
        var save = SaveManager.Instance.CurrentSaveData;
        m_currentAnimalID = save.CurrentCollectingID;
        m_correctDiet = MainManager.Instance.AnimalData[save.CurrentCollectingID].AnimalDiet;
        if (save.CurrentStatus != null)
        {
            m_diets = save.CurrentStatus
                       .Select(i => (DietEnum)Enum.ToObject(typeof(DietEnum), i))
                       .ToList();
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
                DietEnum thisDiet = m_diets[index];

                var draggableOnBtn = button.gameObject.AddComponent<DraggableFood>();
                draggableOnBtn.dietType = thisDiet;


                var parent = button.transform.parent;
                if (parent != null)
                {
                    var siblingImageGO = parent.Find("Image")?.gameObject;
                    if (siblingImageGO != null)
                    {
                        var draggableOnSibling = siblingImageGO.AddComponent<DraggableFood>();
                        draggableOnSibling.dietType = thisDiet;

                        // set the sibling image
                        var image = siblingImageGO.GetComponent<Image>();
                        if (image != null)
                        {
                            image.sprite = Addressables.LoadAssetAsync<Sprite>($"{thisDiet}").WaitForCompletion();
                        }
                    }
                }


                button.onClick.AddListener(() => FoodChoose(thisDiet));

                var text = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = Enum.GetName(typeof(DietEnum), thisDiet);
                }
            }
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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FoodChoose(DietEnum diet)
    {
        // Logic to handle food choice
        Debug.Log($"Diet {diet} chosen.");
        if (diet == m_correctDiet)
        {
            // Correct diet chosen
            m_timer?.StopTimer();

            // Added: Show correct hint popup
            correctHint?.SetActive(true);


            Debug.Log("Correct diet chosen.");
        }
        else
        {
            // Incorrect diet chosen
            hint?.SetActive(true);

            Debug.Log("Incorrect diet chosen.");
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
        MainManager.Instance.SaveCurrentSave(
            -1,
            0,
            0,
            ProgressEnum.AnimalFeeding,
            new List<int>(m_diets.Select(d => (int)d))
        );
    }
}
