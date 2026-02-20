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
        "◆ LONDON METROPOLITAN POLICE: MONITORING FEED",
        "◆ WEATHER: LIGHT RAIN OVER THE WEST END",
        "◆ TUBE UPDATE: MINOR DELAYS ON THE DISTRICT LINE",
        "◆ PUBLIC SERVICE: REPORT SUSPICIOUS MOVEMENT IMMEDIATELY"
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
        tickerText = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        // Force horizontal centering for reliable scrolling math
        // but preserve the user's vertical positioning
        rectTransform.anchorMin = new Vector2(0.5f, rectTransform.anchorMin.y);
        rectTransform.anchorMax = new Vector2(0.5f, rectTransform.anchorMax.y);
        rectTransform.pivot = new Vector2(0.5f, rectTransform.pivot.y);
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
        messageQueue.Enqueue("◆ " + message.ToUpper());
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
        tickerText.text = message;
        tickerText.ForceMeshUpdate();
        float textWidth = tickerText.preferredWidth;
        if (textWidth <= 0) textWidth = 500f;

        float width = 1920f;
        if (useCanvasWidth && rootCanvas != null)
        {
            width = rootCanvas.GetComponent<RectTransform>().rect.width;
        }
        else if (transform.parent != null)
        {
            width = transform.parent.GetComponent<RectTransform>().rect.width;
        }

        // Calculate positions relative to the center
        resetPositionX = (width / 2f) + (textWidth / 2f);
        offscreenLeftX = -(width / 2f) - (textWidth / 2f);

        rectTransform.anchoredPosition = new Vector2(resetPositionX, rectTransform.anchoredPosition.y);
        isScrolling = true;
    }
}
