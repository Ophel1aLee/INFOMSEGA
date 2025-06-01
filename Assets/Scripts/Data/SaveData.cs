using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveData : ScriptableObject
{
    public int SaveID;

    // collection data
    public SerializableDic<int, DifficultyEnum> m_animalDifficulties;
    public DifficultyEnum GetAnimalDifficulty(int id)
    {
        m_animalDifficulties ??= new SerializableDic<int, DifficultyEnum>(new Dictionary<int, DifficultyEnum>());
        if (m_animalDifficulties.ToDictionary().TryGetValue(id, out var difficulty))
        {
            return difficulty;
        }
        return DifficultyEnum.Unknown;
    }
    public void SetAnimalDifficulty(int id)
    {
        m_animalDifficulties ??= new SerializableDic<int, DifficultyEnum>(new Dictionary<int, DifficultyEnum>());
        var animalDifficulties = m_animalDifficulties.ToDictionary();
        var difficulty = CurrentDifficulty;
        if (animalDifficulties.ContainsKey(id))
        {
            animalDifficulties[id] = difficulty;
        }
        else
        {
            animalDifficulties.Add(id, difficulty);
        }
    }

    public List<int> m_collectionIDs;
    public int? CollectionIDs
    {
        get => m_collectionIDs?.Last();
        set
        {
            m_collectionIDs ??= new List<int>();
            if (value.HasValue)
            {
                m_collectionIDs.Add(value.Value);
            }
        }
    }
    public List<int> GetCollectionIDs()
    {
        return m_collectionIDs;
    }

    // current game status
    public int CurrentCollectingID;
    public DifficultyEnum CurrentDifficulty;
    public float CurrentTime;
    public ProgressEnum Progress;
    public List<int> CurrentStatus;
}
