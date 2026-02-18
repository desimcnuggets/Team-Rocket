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
        if (mood >= 70f)
            return new Color(0.3f, 1f, 0.3f, 1f);
        else if (mood >= 50f)
            return new Color(1f, 1f, 0.3f, 1f);
        else if (mood >= 30f)
            return new Color(1f, 0.7f, 0.3f, 1f);
        else
            return new Color(1f, 0.3f, 0.3f, 1f);
    }
}
