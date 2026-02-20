using UnityEngine;
using TMPro;

public class BoroughMoodUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moodText;
    [SerializeField] private BoroughType boroughType;
    
    void Update()
    {
        if (BoroughManager.Instance == null || moodText == null) return;
        
        Borough borough = BoroughManager.Instance.GetBorough(boroughType);
        if (borough != null && borough.isUnlocked)
        {
            moodText.text = $"{borough.displayName}: {borough.mood:F0}%";
            
            Color moodColor = GetMoodColor(borough.mood);
            moodText.color = moodColor;
            
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    Color GetMoodColor(float mood)
    {
        Color color = Color.white;
        if (mood >= 66f)
            ColorUtility.TryParseHtmlString("#2FDE67", out color);
        else if (mood >= 33f)
            ColorUtility.TryParseHtmlString("#FA9A0A", out color);
        else
            ColorUtility.TryParseHtmlString("#ED1A24", out color);
            
        return color;
    }
}
