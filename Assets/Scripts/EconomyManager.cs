using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;
    
    [SerializeField] private float budget = 10000f;
    private float totalBribesReceived = 0f;
    
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
    
    public void DeductFunds(float amount)
    {
        budget -= amount;
        UpdateBudgetUI();
        
        if (budget <= 0f)
        {
            TriggerLossCondition("bankruptcy");
        }
    }
    
    public void AddBribe(float amount)
    {
        budget += amount;
        totalBribesReceived += amount;
        UpdateBudgetUI();
    }
    
    public void TriggerFundingReview()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTicker("FUNDING REVIEW: Commissioner asked to justify budget allocation");
        }
    }
    
    void UpdateBudgetUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateBudget(budget);
        }
    }
    
    void TriggerLossCondition(string reason)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLossScreen(reason);
        }
    }
    
    public float GetBudget()
    {
        return budget;
    }
}
