using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveData : ScriptableObject
{
    public int SaveID;

    // collection data
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
    public ProgressEnum Progress;
    public List<int> CurrentStatus;
}
