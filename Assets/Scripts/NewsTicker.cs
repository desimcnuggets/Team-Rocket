using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NewsTicker : MonoBehaviour
{
    [Header("Scroll Settings")]
    [SerializeField] private bool useCanvasWidth = true;
    [SerializeField] private float scrollSpeed = 100f;

    [Header("Ambient Messages")]
    [SerializeField] private List<string> ambientMessages = new List<string> {
        "LONDON METROPOLITAN POLICE: MONITORING FEED",
        "WEATHER: LIGHT RAIN OVER THE WEST END",
        "TUBE UPDATE: MINOR DELAYS ON THE DISTRICT LINE",
        "PUBLIC SERVICE: REPORT SUSPICIOUS MOVEMENT IMMEDIATELY"
    };

    private TextMeshProUGUI tickerText;
    private RectTransform rectTransform;
    private Queue<string> messageQueue = new Queue<string>();
    private int ambientIndex = 0;
    private bool isScrolling = false;
    private float resetPositionX;
    private float offscreenLeftX;
    private Canvas rootCanvas;

    void Awake()
    {
        if (tickerText == null) tickerText = GetComponent<TextMeshProUGUI>();
        if (tickerText == null) tickerText = GetComponentInChildren<TextMeshProUGUI>();
        
        rectTransform = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();

        // Force ticker-friendly text settings
        if (tickerText != null)
        {
            tickerText.enableWordWrapping = false;
            tickerText.overflowMode = TextOverflowModes.Overflow;
            tickerText.alignment = TextAlignmentOptions.Center;
        }
    }

    void Start()
    {
        // Force horizontal centering for reliable scrolling math
        // but preserve the user's vertical positioning
        rectTransform.anchorMin = new Vector2(0.5f, rectTransform.anchorMin.y);
        rectTransform.anchorMax = new Vector2(0.5f, rectTransform.anchorMax.y);
        rectTransform.pivot = new Vector2(0.5f, rectTransform.pivot.y);
        
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (isScrolling)
        {
            rectTransform.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;

            if (rectTransform.anchoredPosition.x < offscreenLeftX)
            {
                isScrolling = false;
            }
        }
        else
        {
            CheckQueue();
        }
    }

    public void AddMessage(string message)
    {
        messageQueue.Enqueue(message.ToUpper());
    }

    private void CheckQueue()
    {
        string nextMessage = "";

        if (messageQueue.Count > 0)
        {
            nextMessage = messageQueue.Dequeue();
        }
        else if (ambientMessages.Count > 0)
        {
            nextMessage = ambientMessages[ambientIndex];
            ambientIndex = (ambientIndex + 1) % ambientMessages.Count;
        }

        if (nextMessage != "")
        {
            StartScrolling(nextMessage);
        }
    }

    private void StartScrolling(string message)
    {
        tickerText.text = "◆ " + message;
        
        // Ensure layout is current before width check
        Canvas.ForceUpdateCanvases();
        tickerText.ForceMeshUpdate();
        
        float textWidth = tickerText.preferredWidth;
        
        // Safety for empty text or failed mesh update
        if (textWidth <= 0) textWidth = 500f;

        // Size the RectTransform to match the content so pivot 0.5 is accurate
        rectTransform.sizeDelta = new Vector2(textWidth, rectTransform.sizeDelta.y);

        float width = 1920f;
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
        
        if (useCanvasWidth && rootCanvas != null)
        {
            width = rootCanvas.GetComponent<RectTransform>().rect.width;
        }
        else if (transform.parent != null)
        {
            width = transform.parent.GetComponent<RectTransform>().rect.width;
        }

        if (width <= 0) width = 1920f; // Final safety

        // Calculate positions relative to the center
        // resetPositionX: Right edge of text starts at Left edge of screen
        resetPositionX = (width / 2f) + (textWidth / 2f);
        
        // offscreenLeftX: Left edge of text passes Right edge of screen
        // Added 100px padding to ensure it's fully gone before recycling
        offscreenLeftX = -(width / 2f) - (textWidth / 2f) - 100f;

        rectTransform.anchoredPosition = new Vector2(resetPositionX, rectTransform.anchoredPosition.y);
        isScrolling = true;

        Debug.Log($"Ticker Started: '{message}' | Width: {width} | TextWidth: {textWidth} | StartX: {resetPositionX}");
    }
}
