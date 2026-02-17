using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public int currentDay = 1;
    private const float DAY_DURATION = 60f;
    private float dayStartTime;
    
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
    
    void Start()
    {
        dayStartTime = Time.time;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateDayCounter(currentDay);
        }
    }
    
    void Update()
    {
        if (Time.time - dayStartTime >= DAY_DURATION)
        {
            AdvanceDay();
        }
    }
    
    void AdvanceDay()
    {
        currentDay++;
        dayStartTime = Time.time;
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateDayCounter(currentDay);
        }
        
        if (BoroughManager.Instance != null)
        {
            BoroughManager.Instance.CheckUnlocks(currentDay);
        }
        
        if (currentDay % 2 == 0 && EconomyManager.Instance != null)
        {
            EconomyManager.Instance.TriggerFundingReview();
        }
    }
    
    public void StartGame()
    {
        currentDay = 1;
        dayStartTime = Time.time;
    }
    
    public void EndGame()
    {
        Time.timeScale = 0f;
    }
}
