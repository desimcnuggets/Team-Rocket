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
        
        // Get stat tiers
        bool blockTourism = false;
        bool boostCommunity = false;
        
        if (SecondaryStatsManager.Instance != null)
        {
             StatTier coolTier = SecondaryStatsManager.Instance.GetCoolTier();
             if (coolTier == StatTier.Low) blockTourism = true;
             
             if (SecondaryStatsManager.Instance.GetTrustTier() == StatTier.Low) boostCommunity = true;
        }

        foreach (CrimeEvent evt in allEvents)
        {
            if (evt.borough == borough)
            {
                // Penalty: Tourism events stop spawning if Cool is Low
                bool isTourism = (evt.category == CrimeCategory.Tourism);
                
                // Safety fallback: Check name or description for "Tourist" in case category is misconfigured
                if (!isTourism && (evt.eventName.Contains("Tourist") || evt.victimDescription.Contains("Tourist")))
                {
                    isTourism = true;
                }

                if (blockTourism && isTourism)
                    continue;

                filtered.Add(evt);
                
                // Penalty: Community events spawn 50% more frequently if Trust is Low
                // (Adding it an extra time into the random selection pool effectively boosts its probability)
                if (boostCommunity && evt.category == CrimeCategory.Community)
                {
                    // Adding it half as many times more, or simpler: just add it a second time.
                    // Wait, 50% more frequently: 1.5x weight. We can add 1 extra entry for every 2, 
                    // or just add it a second time to boost its chances noticeably. 
                    if (Random.value < 0.5f) // 50% chance to add a duplicate, increasing weight by 50%
                    {
                        filtered.Add(evt);
                    }
                }
            }
        }
        
        return filtered;
    }
}
