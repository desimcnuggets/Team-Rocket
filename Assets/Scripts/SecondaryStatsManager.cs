using UnityEngine;

public class SecondaryStatsManager : MonoBehaviour
{
    public static SecondaryStatsManager Instance;

    [Header("Secondary Stats")]
    [Range(0, 100)] public int economyStat = 50;
    [Range(0, 100)] public int publicTrustStat = 50;
    public int publicTrustCeiling = 100;
    [Range(0, 100)] public int coolFactorStat = 50;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initial checks to apply any starting penalties
            CheckTrustEffects();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Immediately fetch and setup the starting UI text
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateSecondaryStatsUI();
        }
    }

    public StatTier GetEconomyTier() => DetermineTier(economyStat);
    public StatTier GetTrustTier() => DetermineTier(publicTrustStat);
    public StatTier GetCoolTier() => DetermineTier(coolFactorStat);

    private StatTier DetermineTier(int value)
    {
        if (value < 25) return StatTier.Low;
        if (value < 75) return StatTier.Med;
        return StatTier.High;
    }

    public void ModifyEconomy(int amount)
    {
        if (amount == 0) return;
        
        economyStat = Mathf.Clamp(economyStat + amount, 0, 100);
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateSecondaryStatsUI();
            
        // No direct immediate function to call for Economy (effects are passive on triggered events)
    }

    public void ModifyTrust(int amount)
    {
        publicTrustStat = Mathf.Clamp(publicTrustStat + amount, 0, publicTrustCeiling);
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateSecondaryStatsUI();
            
        CheckTrustEffects();
    }

    public void ModifyCool(int amount)
    {
        if (amount == 0) return;
        
        coolFactorStat = Mathf.Clamp(coolFactorStat + amount, 0, 100);
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateSecondaryStatsUI();
            
        // Effects handled passively in CrimeManager random event spawning
    }

    public void CheckTrustEffects()
    {
        // Low Trust Penalty: Max AP reduced to 2
        if (GetTrustTier() == StatTier.Low)
        {
            if (PoliceManager.Instance != null)
            {
                PoliceManager.Instance.maxUnits = 2;
                
                // If they have more units than the new max, reduce their current units
                if (PoliceManager.Instance.currentUnits > 2)
                {
                    PoliceManager.Instance.currentUnits = 2;
                }
            }
        }
        else
        {
             // Resume to 3
             if (PoliceManager.Instance != null)
             {
                 PoliceManager.Instance.maxUnits = 3;
             }
        }
    }
}
