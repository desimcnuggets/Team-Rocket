using System.Collections.Generic;
using UnityEngine;

public class EventDatabase : MonoBehaviour
{
    public static EventDatabase Instance;
    
    [SerializeField] private List<CrimeEvent> allEvents = new List<CrimeEvent>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public CrimeEvent GetRandomEvent(BoroughType borough)
    {
        List<CrimeEvent> boroughEvents = GetEventsByBorough(borough);
        
        if (boroughEvents.Count == 0)
        {
            return null;
        }
        
        return boroughEvents[Random.Range(0, boroughEvents.Count)];
    }
    
    public List<CrimeEvent> GetEventsByBorough(BoroughType borough)
    {
        List<CrimeEvent> filtered = new List<CrimeEvent>();
        
        foreach (CrimeEvent evt in allEvents)
        {
            if (evt.borough == borough)
            {
                filtered.Add(evt);
            }
        }
        
        return filtered;
    }
}
