using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    private static PoliceManager instance;
    public static PoliceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PoliceManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("PoliceManager");
                    instance = go.AddComponent<PoliceManager>();
                }
            }
            return instance;
        }
    }

    public int maxUnits = 3;
    public int currentUnits;
    public float regenInterval = 15f;
    public float regenTimer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        currentUnits = maxUnits;
    }

    void Update()
    {
        if (currentUnits < maxUnits)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenInterval)
            {
                currentUnits = maxUnits;
                regenTimer = 0f;
            }
        }
    }

    public bool HasUnits()
    {
        return currentUnits > 0;
    }

    public void ConsumeUnit()
    {
        if (currentUnits > 0)
        {
            currentUnits--;
            // If we were at max, start the timer now
            if (currentUnits == maxUnits - 1 && regenTimer == 0f)
            {
                regenTimer = 0f;
            }
        }
    }
}
