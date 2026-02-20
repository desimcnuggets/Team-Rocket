using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance;

    [Header("Pause Panel")]
    [SerializeField] private GameObject pausePanel;

    [Header("Audio Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Accessibility")]
    [SerializeField] private Toggle dyslexicToggle;

    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // Initialise sliders to current volume
        if (AudioManager.Instance != null)
        {
            if (musicSlider != null)
            {
                musicSlider.value = AudioManager.Instance.GetMusicVolume();
                musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = AudioManager.Instance.GetSFXVolume();
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
        }

        if (pausePanel != null) pausePanel.SetActive(false);

        // Initialise dyslexic toggle
        if (dyslexicToggle != null)
        {
            bool isDyslexic = AccessibilityManager.Instance != null &&
                              AccessibilityManager.Instance.IsDyslexicModeOn;
            dyslexicToggle.SetIsOnWithoutNotify(isDyslexic);
            dyslexicToggle.onValueChanged.AddListener(OnDyslexicToggled);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(isPaused);
    }

    public void OnResumeClicked()
    {
        if (isPaused) TogglePause();
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    public bool IsPaused => isPaused;

    public void OnDyslexicToggled(bool value)
    {
        if (AccessibilityManager.Instance != null)
            AccessibilityManager.Instance.SetDyslexicMode(value);
    }
}
