using System.Collections.Generic;
using UnityEngine;

public class BoroughManager : MonoBehaviour
{
    public static BoroughManager Instance;
    
    [SerializeField] private Material boroughActiveMaterial;
    [SerializeField] private GameObject unlockParticlePrefab;
    
    private const float BOROUGH_RADIUS = 15f;
    [SerializeField] private List<Borough> boroughs = new List<Borough>(); // Exposed to Inspector
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Only initialize defaults if the list is empty (Inspector not used)
            if (boroughs.Count == 0)
            {
                InitializeBoroughs();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeBoroughs()
    {
        // ... (Keep existing hardcoded logic as fallback) ...
        // 1. Greenwich (Starting area)
        boroughs.Add(new Borough
        {
            type = BoroughType.Greenwich,
            displayName = "Greenwich",
            isUnlocked = true,
            unlockDay = 1,
            mood = 70f,
            cameraAngle = 0f
        });
        
        // 2. Westminster
        boroughs.Add(new Borough
        {
            type = BoroughType.Westminster,
            displayName = "Westminster",
            isUnlocked = false,
            unlockDay = 3,
            mood = 65f,
            cameraAngle = 40f
        });
        
        // 3. Lambeth
        boroughs.Add(new Borough
        {
            type = BoroughType.Lambeth,
            displayName = "Lambeth",
            isUnlocked = false,
            unlockDay = 5,
            mood = 50f,
            cameraAngle = -40f
        });
        
        // 4. Hillingdon
        boroughs.Add(new Borough
        {
            type = BoroughType.Hillingdon,
            displayName = "Hillingdon",
            isUnlocked = false,
            unlockDay = 7,
            mood = 45f,
            cameraAngle = 80f
        });
        
        // 5. Kensington
        boroughs.Add(new Borough
        {
            type = BoroughType.Kensington,
            displayName = "Kensington",
            isUnlocked = false,
            unlockDay = 7,
            mood = 40f,
            cameraAngle = -80f
        });
        
        // 6. Camden
        boroughs.Add(new Borough
        {
            type = BoroughType.Camden,
            displayName = "Camden",
            isUnlocked = false,
            unlockDay = 9,
            mood = 60f,
            cameraAngle = 130f
        });
    }
    
    public List<Borough> GetUnlockedBoroughs()
    {
        List<Borough> unlocked = new List<Borough>();
        foreach (Borough b in boroughs)
        {
            if (b.isUnlocked)
            {
                unlocked.Add(b);
            }
        }
        return unlocked;
    }
    
    public Borough GetBorough(BoroughType type)
    {
        foreach (Borough b in boroughs)
        {
            if (b.type == type)
            {
                return b;
            }
        }
        return null;
    }
    
    public List<Borough> GetAllBoroughs()
    {
        return boroughs;
    }
    
    public void CheckUnlocks(int day)
    {
        foreach (Borough b in boroughs)
        {
            if (!b.isUnlocked && day >= b.unlockDay)
            {
                UnlockBorough(b);
            }
        }
    }
    
    void UnlockBorough(Borough b)
    {
        b.isUnlocked = true;
        
        // Disable the "Locked" visual (Low opacity cube)
        if (b.lockedVisual != null)
        {
            b.lockedVisual.SetActive(false);
        }
        
        if (b.boroughModel != null)
        {
            Renderer renderer = b.boroughModel.GetComponent<Renderer>();
            if (renderer != null && boroughActiveMaterial != null)
            {
                renderer.material = boroughActiveMaterial;
            }
            
            if (unlockParticlePrefab != null)
            {
                Instantiate(unlockParticlePrefab, b.boroughModel.transform.position, Quaternion.identity);
            }
        }
        
        string tickerLine = GetUnlockTickerLine(b.type);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTicker(tickerLine);
        }
    }
    
    string GetUnlockTickerLine(BoroughType type)
    {
        switch (type)
        {
            case BoroughType.Westminster:
                return "BREAKING: Crime spreads to Westminster — Politicians nervously check exits";
            case BoroughType.Lambeth:
                return "Lambeth unlocked — Local businesses brace for impact";
            case BoroughType.Hillingdon:
                return "Hillingdon now active — Suburban tranquility disrupted";
            case BoroughType.Kensington:
                return "Kensington enters the fray — Property values threatened";
            case BoroughType.Camden:
                return "Camden unlocked — Alternative scene meets alternative crime";
            default:
                return "New area unlocked";
        }
    }
    
    public void IncreaseMood(BoroughType type, float amount)
    {
        Borough b = GetBorough(type);
        if (b != null)
        {
            b.mood += amount;
            b.mood = Mathf.Clamp(b.mood, 0f, 100f);
        }
    }
    
    public void DecreaseMood(BoroughType type, float amount)
    {
        Borough b = GetBorough(type);
        if (b != null)
        {
            b.mood -= amount;
            b.mood = Mathf.Clamp(b.mood, 0f, 100f);
        }
    }
    
    public void PositionBorough(Borough borough, float angleInDegrees, float radius)
    {
        if (borough.boroughModel == null) return;
        
        float angleRad = angleInDegrees * Mathf.Deg2Rad;
        float x = Mathf.Sin(angleRad) * radius;
        float z = Mathf.Cos(angleRad) * radius;
        borough.boroughModel.transform.position = new Vector3(x, 0, z);
        borough.boroughModel.transform.LookAt(Vector3.zero);
    }
}
