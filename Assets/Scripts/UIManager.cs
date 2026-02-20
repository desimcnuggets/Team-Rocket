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
    [SerializeField] private Image crimeBarFill;
    [SerializeField] private TextMeshProUGUI crimeRateText;
    [SerializeField] private TextMeshProUGUI budgetText;
    [SerializeField] private TextMeshProUGUI dayCounterText;
    
    [Header("News Ticker")]
    [SerializeField] private TextMeshProUGUI newsTickerText;
    [SerializeField] private NewsTicker newsTicker;
    
    [Header("Secondary Stats")]
    [SerializeField] private TextMeshProUGUI economyText;
    [SerializeField] private TextMeshProUGUI publicTrustText;
    [SerializeField] private TextMeshProUGUI coolFactorText;
    
    [Header("Loss Screen")]
    [SerializeField] private GameObject lossScreenPanel;
    [SerializeField] private TextMeshProUGUI lossReasonText;

    [Header("Police Units")]
    [SerializeField] private Image[] policeUnitIcons;
    [SerializeField] private Slider policeRegenSlider;
    [SerializeField] private Color unitAvailableColor = Color.blue;
    [SerializeField] private Color unitUsedColor = Color.gray;
    
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

    public bool IsDecisionPanelOpen
    {
        get { return decisionCardPanel != null && decisionCardPanel.activeSelf; }
    }
    

    public void OnRaidClicked()
    {
        Debug.Log("Raid Clicked");
        if (AudioManager.Instance != null) AudioManager.Instance.PlayRaidClick();
        if (EconomyManager.Instance == null) return;
        
        // CHECK POLICE UNITS
        if (PoliceManager.Instance == null || !PoliceManager.Instance.HasUnits())
        {
             UpdateTicker("NO UNITS AVAILABLE: Wait for reinforcements!");
             return;
        }
        
        if (EconomyManager.Instance.GetBudget() < 1000f)
        {
            UpdateTicker("INSUFFICIENT FUNDS: Raid cancelled");
            return;
        }
        
        EconomyManager.Instance.DeductFunds(1000);
        PoliceManager.Instance.ConsumeUnit();
        
        if (BoroughManager.Instance != null && currentEvent != null)
        {
            BoroughManager.Instance.ProcessRaidMechanics(currentEvent);
        }
        
        if (currentEvent != null)
        {
            UpdateTicker(currentEvent.tickerRaid);
            
            // Check Trust Penalty: Civil Unrest RAID increases crime
            if (SecondaryStatsManager.Instance != null && SecondaryStatsManager.Instance.GetTrustTier() == StatTier.Low && currentEvent.category == CrimeCategory.CivilUnrest)
            {
                if (CrimeManager.Instance != null) CrimeManager.Instance.ModifyCrimeRate(5); // Increase rather than decrease
            }
            else
            {
                if (CrimeManager.Instance != null) CrimeManager.Instance.ModifyCrimeRate(-3); // Normal reduce
            }
            
            // Apply Secondary Stats
            if (SecondaryStatsManager.Instance != null)
            {
                SecondaryStatsManager.Instance.ModifyEconomy(currentEvent.raidEconomyChange);
                SecondaryStatsManager.Instance.ModifyTrust(currentEvent.raidTrustChange);
                SecondaryStatsManager.Instance.ModifyCool(currentEvent.raidCoolChange);
            }
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

    void Update()
    {
        UpdatePoliceUI();
        
        if (isAnarchy && crimeBarFill != null)
        {
            float t = Mathf.PingPong(Time.time * 2f, 1f);
            crimeBarFill.color = Color.Lerp(Color.red, new Color(0.5f, 0f, 0f), t);
        }
    }

    void UpdatePoliceUI()
    {
        if (PoliceManager.Instance == null) return;

        if (policeRegenSlider != null)
        {
            if (PoliceManager.Instance.currentUnits >= PoliceManager.Instance.maxUnits)
            {
                 policeRegenSlider.value = policeRegenSlider.maxValue;
            }
            else
            {
                 float progress = PoliceManager.Instance.regenTimer / PoliceManager.Instance.regenInterval;
                 policeRegenSlider.value = progress * policeRegenSlider.maxValue;
            }
        }

        if (policeUnitIcons != null)
        {
            for (int i = 0; i < policeUnitIcons.Length; i++)
            {
                if (policeUnitIcons[i] != null)
                {
                    if (i < PoliceManager.Instance.currentUnits)
                    {
                        policeUnitIcons[i].color = unitAvailableColor;
                    }
                    else
                    {
                        policeUnitIcons[i].color = unitUsedColor;
                    }
                }
            }
        }
    }
    
    public void OnIgnoreClicked()
    {
        if (currentEvent == null) return;
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayIgnoreClick();
        
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
            BoroughManager.Instance.ProcessIgnoreMechanics(currentEvent);
        }
        
        UpdateTicker(currentEvent.tickerIgnore);
        
        // Apply Secondary Stats
        if (SecondaryStatsManager.Instance != null)
        {
            SecondaryStatsManager.Instance.ModifyEconomy(currentEvent.ignoreEconomyChange);
            SecondaryStatsManager.Instance.ModifyTrust(currentEvent.ignoreTrustChange);
            SecondaryStatsManager.Instance.ModifyCool(currentEvent.ignoreCoolChange);
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
    
    private bool isAnarchy = false;
    
    public void UpdateCrimeBar(float crimeRate)
    {
        if (crimeRateBar != null)
        {
            crimeRateBar.value = crimeRate;
        }
        
        if (crimeBarFill != null)
        {
            isAnarchy = false;
            if (crimeRate < 30f)
            {
                crimeBarFill.color = Color.blue;
            }
            else if (crimeRate < 70f)
            {
                crimeBarFill.color = Color.green;
            }
            else if (crimeRate < 90f)
            {
                // Amber
                crimeBarFill.color = new Color(1f, 0.75f, 0f);
            }
            else
            {
                isAnarchy = true;
            }
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
        if (newsTicker != null)
        {
            newsTicker.AddMessage(message);
        }
        else if (newsTickerText != null)
        {
            newsTickerText.text = message;
        }
    }
    
    public void UpdateSecondaryStatsUI()
    {
        if (SecondaryStatsManager.Instance == null) return;
        
        if (economyText != null) economyText.text = $"Economy: {SecondaryStatsManager.Instance.economyStat}";
        if (publicTrustText != null) publicTrustText.text = $"Trust: {SecondaryStatsManager.Instance.publicTrustStat}";
        if (coolFactorText != null) coolFactorText.text = $"Cool: {SecondaryStatsManager.Instance.coolFactorStat}";
    }
    
    public void ShowLossScreen(string reason)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOver();
        
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
