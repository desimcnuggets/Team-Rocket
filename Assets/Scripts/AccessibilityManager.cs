using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that manages global accessibility settings.
/// Currently handles the Dyslexic Mode toggle, which swaps all
/// active TMP_Text components to a user-specified dyslexia-friendly font.
///
/// HOW TO USE:
///   1. Attach this component to a persistent GameObject (or let it
///      create its own via the lazy-init pattern below).
///   2. Assign a TMP_FontAsset (e.g. OpenDyslexic SDF) to the
///      "Dyslexic Font" field in the Inspector.
///   3. Call SetDyslexicMode(true/false) from the pause menu toggle.
/// </summary>
public class AccessibilityManager : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────────────────
    public static AccessibilityManager Instance { get; private set; }

    // ── Inspector fields ─────────────────────────────────────────────────────
    [Header("Dyslexic Mode")]
    [Tooltip("A dyslexia-friendly TMP Font Asset (e.g. OpenDyslexic SDF). " +
             "Generate one via Window → TextMeshPro → Font Asset Creator.")]
    [SerializeField] private TMP_FontAsset dyslexicFont;

    // ── Runtime state ─────────────────────────────────────────────────────────
    private bool isDyslexicModeOn = false;
    private bool isStaticTickerModeOn = false;

    // Stores all per-component state we touch so we can restore it exactly.
    private struct TextSnapshot
    {
        public TMP_FontAsset font;
        public bool          autoSizing;
        public float         fontSize;
        public float         fontSizeMin;
        public float         fontSizeMax;
    }

    private readonly Dictionary<TMP_Text, TextSnapshot> snapshots =
        new Dictionary<TMP_Text, TextSnapshot>();

    private const string PrefKey = "DyslexicMode";
    private const string TickerPrefKey = "StaticTickerMode";

    // ── Public read-only accessor ─────────────────────────────────────────────
    public bool IsDyslexicModeOn => isDyslexicModeOn;
    public bool IsStaticTickerModeOn => isStaticTickerModeOn;

    // ── Lifecycle ─────────────────────────────────────────────────────────────
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
            return;
        }

        // Restore saved preference
        bool saved = PlayerPrefs.GetInt(PrefKey, 0) == 1;
        if (saved)
        {
            // Apply after a frame so all Start() calls in the scene have run
            isDyslexicModeOn = true;
        }

        isStaticTickerModeOn = PlayerPrefs.GetInt(TickerPrefKey, 0) == 1;
    }

    void Start()
    {
        if (isDyslexicModeOn)
            ApplyDyslexicFont();

        // Re-apply whenever a new scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isDyslexicModeOn)
        {
            // Clear stale references from previous scene, then reapply
            snapshots.Clear();
            ApplyDyslexicFont();
        }
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Enable or disable dyslexic font mode.
    /// Connected to the pause menu Toggle's onValueChanged event.
    /// </summary>
    public void SetDyslexicMode(bool enabled)
    {
        if (isDyslexicModeOn == enabled) return;

        isDyslexicModeOn = enabled;
        PlayerPrefs.SetInt(PrefKey, enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (enabled)
            ApplyDyslexicFont();
        else
            RestoreOriginalFonts();
    }

    /// <summary>
    /// Enable or disable static news ticker mode.
    /// Connected to the pause menu Toggle's onValueChanged event.
    /// </summary>
    public void SetStaticTickerMode(bool enabled)
    {
        Debug.Log($"[AccessibilityManager] Setting Static Ticker Mode: {enabled}");
        if (isStaticTickerModeOn == enabled) return;

        isStaticTickerModeOn = enabled;
        PlayerPrefs.SetInt(TickerPrefKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Finds every TMP_Text in the scene (including inactive objects), snapshots
    /// its font and size settings, swaps to the dyslexic font, then enables
    /// Auto Size so TMP shrinks the text to fit its container rather than overflow.
    /// </summary>
    private void ApplyDyslexicFont()
    {
        if (dyslexicFont == null)
        {
            Debug.LogWarning("[AccessibilityManager] Dyslexic Font is not assigned! " +
                             "Please assign a TMP_FontAsset in the Inspector.");
            return;
        }

        // FindObjectsByType includes inactive objects (Unity 2023+ API)
        TMP_Text[] allTexts = FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (TMP_Text t in allTexts)
        {
            // Snapshot original settings only the first time
            if (!snapshots.ContainsKey(t))
            {
                snapshots[t] = new TextSnapshot
                {
                    font        = t.font,
                    autoSizing  = t.enableAutoSizing,
                    fontSize    = t.fontSize,
                    fontSizeMin = t.fontSizeMin,
                    fontSizeMax = t.fontSizeMax
                };
            }

            // Swap font
            t.font = dyslexicFont;

            // Enable Auto Size: ceiling = original size, floor = 40% of that (min 8pt)
            float originalSize = snapshots[t].fontSize;
            t.enableAutoSizing = true;
            t.fontSizeMax      = originalSize;
            t.fontSizeMin      = Mathf.Max(8f, originalSize * 0.4f);
        }

        Debug.Log($"[AccessibilityManager] Dyslexic Mode ON — swapped {allTexts.Length} TMP_Text components.");
    }

    /// <summary>
    /// Restores every TMP_Text to the exact state it was in before dyslexic
    /// mode was enabled (font, font size, and auto-sizing settings).
    /// </summary>
    private void RestoreOriginalFonts()
    {
        int restored = 0;
        foreach (var kvp in snapshots)
        {
            TMP_Text t = kvp.Key;
            if (t == null) continue; // guard against destroyed objects

            TextSnapshot snap = kvp.Value;
            t.font             = snap.font;
            t.enableAutoSizing = snap.autoSizing;
            t.fontSizeMin      = snap.fontSizeMin;
            t.fontSizeMax      = snap.fontSizeMax;
            // Restore the explicit size last so TMP doesn't re-clamp it
            t.fontSize         = snap.fontSize;
            restored++;
        }
        snapshots.Clear();

        Debug.Log($"[AccessibilityManager] Dyslexic Mode OFF — restored {restored} TMP_Text components.");
    }
}
