using System.Collections.Generic;
using UnityEngine;

public class CrimeManager : MonoBehaviour
{
    public static CrimeManager Instance;
    
    [SerializeField] private float crimeRate = 65f;
    [SerializeField] private float naturalDrift = 0.5f;
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private GameObject crimeIconPrefab;
    
    private float nextSpawnTime;
    private float nextDriftTime;
    private float anarchyTimer = 0f;
    private bool gameOver = false;
    private List<GameObject> activeEvents = new List<GameObject>();
    private List<EscalationEntry> escalationQueue = new List<EscalationEntry>();
    
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
    
    void Update()
    {
        if (gameOver) return;

        if (Time.time >= nextDriftTime)
        {
            crimeRate += naturalDrift;
            crimeRate = Mathf.Clamp(crimeRate, 0f, 100f);
            nextDriftTime = Time.time + 5f;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateCrimeBar(crimeRate);
            }
        }

        // Sustained anarchy: crime >= 90% for 30 continuous seconds
        if (crimeRate >= 90f)
        {
            anarchyTimer += Time.deltaTime;
            if (anarchyTimer >= 30f)
            {
                gameOver = true;
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowLossScreen("anarchy");
            }
        }
        else
        {
            anarchyTimer = 0f;
        }
        
        if (Time.time >= nextSpawnTime)
        {
            SpawnRandomEvent();
            nextSpawnTime = Time.time + spawnInterval;
        }
        
        CheckEscalations();
    }

    public float GetCrimeRate() => crimeRate;

    public void SetGameOver() => gameOver = true;
    
    void SpawnRandomEvent()
    {
        if (BoroughManager.Instance == null || EventDatabase.Instance == null) return;
        
        List<Borough> unlocked = BoroughManager.Instance.GetUnlockedBoroughs();
        if (unlocked.Count == 0) return;
        
        Borough targetBorough = unlocked[Random.Range(0, unlocked.Count)];
        CrimeEvent evt = EventDatabase.Instance.GetRandomEvent(targetBorough.type);
        
        if (evt == null || targetBorough.boroughModel == null) return;
        
        Vector3 spawnPos = GetRandomPositionOnBorough(targetBorough);
        
        GameObject icon = Instantiate(crimeIconPrefab, spawnPos, Quaternion.identity);
        icon.GetComponent<CrimeIcon>().Initialize(evt);
        activeEvents.Add(icon);
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayEventSpawn();
    }
    
    Vector3 GetRandomPositionOnBorough(Borough borough)
    {
        // Use the dedicated ground object if it exists (Preferred)
        if (borough.boroughGround != null)
        {
            Renderer gr = borough.boroughGround.GetComponent<Renderer>();
            if (gr != null)
            {
                Bounds b = gr.bounds;
                float x = Random.Range(b.min.x, b.max.x);
                float z = Random.Range(b.min.z, b.max.z);
                return new Vector3(x, b.max.y + 40f, z);
            }
            // Fallback to collider if no renderer on ground
            Collider gc = borough.boroughGround.GetComponent<Collider>();
            if (gc != null)
            {
                Bounds b = gc.bounds;
                float x = Random.Range(b.min.x, b.max.x);
                float z = Random.Range(b.min.z, b.max.z);
                return new Vector3(x, b.max.y + 40f, z);
            }
        }

        // Fallback to the old "Borough Model" logic
        Vector3 boroughCenter = borough.boroughModel.transform.position;
        Bounds bounds = new Bounds(boroughCenter, Vector3.one * 3f); // Default fallback

        Renderer r = borough.boroughModel.GetComponent<Renderer>();
        if (r != null)
        {
            bounds = r.bounds;
        }
        else
        {
            Collider c = borough.boroughModel.GetComponent<Collider>();
            if (c != null)
            {
                bounds = c.bounds;
            }
        }
        
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        float spawnY = bounds.max.y + 40f; // Place slightly above the top
        
        return new Vector3(randomX, spawnY, randomZ);
    }
    
    public void ModifyCrimeRate(float change)
    {
        crimeRate += change;
        crimeRate = Mathf.Clamp(crimeRate, 0f, 100f);
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCrimeBar(crimeRate);
        }
    }
    
    public void AddToEscalationQueue(CrimeEvent evt)
    {
        EscalationEntry entry = new EscalationEntry
        {
            originalEvent = evt,
            ignoreTimestamp = Time.time,
            borough = evt.borough
        };
        escalationQueue.Add(entry);
    }
    
    public bool IsInEscalationQueue(CrimeEvent evt)
    {
        foreach (EscalationEntry entry in escalationQueue)
        {
            if (entry.originalEvent == evt)
            {
                return true;
            }
        }
        return false;
    }
    
    void CheckEscalations()
    {
        for (int i = escalationQueue.Count - 1; i >= 0; i--)
        {
            EscalationEntry entry = escalationQueue[i];
            
            if (Time.time - entry.ignoreTimestamp >= 20f)
            {
                if (Random.value <= entry.originalEvent.escalationChance)
                {
                    if (entry.originalEvent.escalatesTo != null)
                    {
                        if (AudioManager.Instance != null) AudioManager.Instance.PlayEscalationTrigger();
                        ForceSpawnEvent(entry.originalEvent.escalatesTo, entry.borough);
                        
                        if (UIManager.Instance != null)
                        {
                            UIManager.Instance.UpdateTicker("ESCALATION: " + entry.originalEvent.escalatesTo.eventName);
                        }
                    }
                }
                escalationQueue.RemoveAt(i);
            }
        }
    }
    
    void ForceSpawnEvent(CrimeEvent evt, BoroughType borough)
    {
        if (BoroughManager.Instance == null) return;
        
        Borough b = BoroughManager.Instance.GetBorough(borough);
        if (b == null || b.boroughModel == null) return;
        
        Vector3 pos = GetRandomPositionOnBorough(b);
        GameObject icon = Instantiate(crimeIconPrefab, pos, Quaternion.identity);
        icon.GetComponent<CrimeIcon>().Initialize(evt);
        activeEvents.Add(icon);
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayEventSpawn();
    }
}
