using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Decision Card")]
    [SerializeField] private GameObject decisionCardPanel;
    [SerializeField] private TextMeshProUGUI eventNameText;
    [SerializeField] private TextMeshProUGUI boroughText;
    [SerializeField] private Image victimPortrait;
    [SerializeField] private TextMeshProUGUI victimDescText;
    [SerializeField] private GameObject escalationWarning;
    
    [Header("HUD")]
    [SerializeField] private Slider crimeRateBar;
    [SerializeField] private TextMeshProUGUI crimeRateText;
    [SerializeField] private TextMeshProUGUI budgetText;
    [SerializeField] private TextMeshProUGUI dayCounterText;
    
    [Header("News Ticker")]
    [SerializeField] private TextMeshProUGUI newsTickerText;
    
    [Header("Loss Screen")]
    [SerializeField] private GameObject lossScreenPanel;
    [SerializeField] private TextMeshProUGUI lossReasonText;
    
    private CrimeEvent currentEvent;
    private GameObject currentIcon;
    
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
    
    public void ShowDecisionCard(CrimeEvent evt, GameObject iconObject)
    {
        if (decisionCardPanel == null) return;
        
        decisionCardPanel.SetActive(true);
        currentEvent = evt;
        currentIcon = iconObject;
        
        if (eventNameText != null) eventNameText.text = evt.eventName;
        if (boroughText != null) boroughText.text = evt.borough.ToString();
        if (victimPortrait != null && evt.victimPortrait != null) victimPortrait.sprite = evt.victimPortrait;
        if (victimDescText != null) victimDescText.text = evt.victimDescription;
        
        if (escalationWarning != null && CrimeManager.Instance != null)
        {
            bool alreadyIgnored = CrimeManager.Instance.IsInEscalationQueue(evt);
            escalationWarning.SetActive(alreadyIgnored);
        }
    }
    
    public void OnRaidClicked()
    {
        if (EconomyManager.Instance == null) return;
        
        if (EconomyManager.Instance.GetBudget() < 1000f)
        {
            UpdateTicker("INSUFFICIENT FUNDS: Raid cancelled");
            return;
        }
        
        EconomyManager.Instance.DeductFunds(1000);
        
        if (CrimeManager.Instance != null)
        {
            CrimeManager.Instance.ModifyCrimeRate(-3);
        }
        
        if (BoroughManager.Instance != null && currentEvent != null)
        {
            BoroughManager.Instance.IncreaseMood(currentEvent.borough, 1);
        }
        
        if (currentEvent != null)
        {
            UpdateTicker(currentEvent.tickerRaid);
        }
        
        if (currentIcon != null)
        {
            Destroy(currentIcon);
        }
        
        if (decisionCardPanel != null)
        {
            decisionCardPanel.SetActive(false);
        }
    }
    
    public void OnIgnoreClicked()
    {
        if (currentEvent == null) return;
        
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.AddBribe(currentEvent.bribeValue);
        }
        
        if (CrimeManager.Instance != null)
        {
            CrimeManager.Instance.ModifyCrimeRate(currentEvent.crimeRateChange);
            CrimeManager.Instance.AddToEscalationQueue(currentEvent);
        }
        
        if (BoroughManager.Instance != null)
        {
            BoroughManager.Instance.DecreaseMood(currentEvent.borough, 2);
        }
        
        UpdateTicker(currentEvent.tickerIgnore);
        
        if (currentIcon != null)
        {
            Destroy(currentIcon);
        }
        
        if (decisionCardPanel != null)
        {
            decisionCardPanel.SetActive(false);
        }
    }
    
    public void UpdateCrimeBar(float crimeRate)
    {
        if (crimeRateBar != null)
        {
            crimeRateBar.value = crimeRate;
        }
        
        if (crimeRateText != null)
        {
            crimeRateText.text = $"{crimeRate:F0}%";
        }
        
        if (crimeRate <= 0f)
        {
            ShowLossScreen("crime_zero");
        }
        else if (crimeRate >= 100f)
        {
            ShowLossScreen("crime_max");
        }
    }
    
    public void UpdateBudget(float budget)
    {
        if (budgetText != null)
        {
            budgetText.text = $"£{budget:F0}";
        }
    }
    
    public void UpdateDayCounter(int day)
    {
        UpdateDayTime(day, 0f);
    }
    
    public void UpdateDayTime(int day, float timeSeconds)
    {
        if (dayCounterText != null)
        {
            // 60 seconds = 24 hours = 1440 minutes
            // 1 second constant = 24 minutes game time
            float totalMinutes = timeSeconds * 24f;
            
            int totalHours = Mathf.FloorToInt(totalMinutes / 60f);
            int displayMinutes = Mathf.FloorToInt(totalMinutes % 60f);
            
            // Handle 24h wrapping for display logic
            int hourOfDay = totalHours % 24;
            
            string period = "AM";
            int displayHour = hourOfDay;
            
            if (hourOfDay >= 12)
            {
                period = "PM";
                if (hourOfDay > 12) displayHour -= 12;
            }
            if (displayHour == 0) displayHour = 12;
            
            dayCounterText.text = $"Day {day} - {displayHour}:{displayMinutes:D2} {period}";
        }
    }

    
    public void UpdateTicker(string message)
    {
        if (newsTickerText != null)
        {
            newsTickerText.text = message;
        }
    }
    
    public void ShowLossScreen(string reason)
    {
        if (lossScreenPanel != null)
        {
            lossScreenPanel.SetActive(true);
        }
        
        if (lossReasonText != null)
        {
            switch (reason)
            {
                case "bankruptcy":
                    lossReasonText.text = "BUDGET DEPLETED - Commissioner Fired";
                    break;
                case "crime_zero":
                    lossReasonText.text = "CRIME ELIMINATED - You're TOO Good (Suspicious)";
                    break;
                case "crime_max":
                    lossReasonText.text = "TOTAL ANARCHY - London Has Fallen";
                    break;
                default:
                    lossReasonText.text = "GAME OVER";
                    break;
            }
        }
    }
}
