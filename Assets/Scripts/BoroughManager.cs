using System.Collections.Generic;
using UnityEngine;

public class BoroughManager : MonoBehaviour
{
    public static BoroughManager Instance;
    
    [SerializeField] private Material boroughActiveMaterial;
    [SerializeField] private GameObject unlockParticlePrefab;
    
    private const float BOROUGH_RADIUS = 15f;
    private List<Borough> boroughs = new List<Borough>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeBoroughs();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeBoroughs()
    {
        boroughs.Add(new Borough
        {
            type = BoroughType.Westminster,
            displayName = "Westminster",
            isUnlocked = true,
            unlockDay = 1,
            mood = 50f,
            cameraAngle = 0f
        });
        
        boroughs.Add(new Borough
        {
            type = BoroughType.TheCity,
            displayName = "The City",
            isUnlocked = false,
            unlockDay = 3,
            mood = 50f,
            cameraAngle = 40f
        });
        
        boroughs.Add(new Borough
        {
            type = BoroughType.Camden,
            displayName = "Camden",
            isUnlocked = false,
            unlockDay = 3,
            mood = 50f,
            cameraAngle = -40f
        });
        
        boroughs.Add(new Borough
        {
            type = BoroughType.EastEnd,
            displayName = "East End",
            isUnlocked = false,
            unlockDay = 5,
            mood = 50f,
            cameraAngle = 80f
        });
        
        boroughs.Add(new Borough
        {
            type = BoroughType.Brixton,
            displayName = "Brixton",
            isUnlocked = false,
            unlockDay = 5,
            mood = 50f,
            cameraAngle = -80f
        });
        
        boroughs.Add(new Borough
        {
            type = BoroughType.Kingston,
            displayName = "Kingston",
            isUnlocked = false,
            unlockDay = 7,
            mood = 50f,
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
        b.mood = 50f;
        
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
            case BoroughType.TheCity:
                return "BREAKING: Crime spreads to The City — Bankers nervously check exits";
            case BoroughType.Camden:
                return "Camden unlocked — Roadmen rejoice, artisanal coffee shops brace";
            case BoroughType.EastEnd:
                return "East End now active — Local geezers unimpressed";
            case BoroughType.Brixton:
                return "Brixton enters the fray — Community leaders demand action";
            case BoroughType.Kingston:
                return "Kingston unlocked — Suburban chaos ensues";
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
