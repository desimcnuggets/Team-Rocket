using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")]
    //A quiet police radio crackle
    [SerializeField] private AudioClip eventSpawnClip;
    
    //A stamp sound. Satisfying. Final
    [SerializeField] private AudioClip raidClickClip;
    
    //A file drawer closing. Dismissive
    [SerializeField] private AudioClip ignoreClickClip;
    
    //A distant siren getting closer
    [SerializeField] private AudioClip escalationTriggerClip;
    
    //A letter being opened
    [SerializeField] private AudioClip fundingReviewClip;
    
    //The BBC News opening theme, wrong
    [SerializeField] private AudioClip gameOverClip;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayEventSpawn() => PlaySFX(eventSpawnClip);
    public void PlayRaidClick() => PlaySFX(raidClickClip);
    public void PlayIgnoreClick() => PlaySFX(ignoreClickClip);
    public void PlayEscalationTrigger() => PlaySFX(escalationTriggerClip);
    public void PlayFundingReview() => PlaySFX(fundingReviewClip);
    
    // Using PlaySFX or PlayMusic for Game Over depending on how you want it, SFX default
    public void PlayGameOver() => PlaySFX(gameOverClip); 

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else if (clip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play a null SFX clip.");
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }
}
