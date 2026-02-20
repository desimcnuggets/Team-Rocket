using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Clips")]
    // Soft typewriter clack
    [SerializeField] private AudioClip crimeSpawnClip;
    
    // File stamp thud
    [SerializeField] private AudioClip iconClickClip;
    
    // Radio crackle + brief static
    [SerializeField] private AudioClip raidActionClip;
    
    // Cash register ding
    [SerializeField] private AudioClip ignoreActionClip;
    
    // Urgent alarm stab
    [SerializeField] private AudioClip escalationSpawnClip;
    
    // Low tension drone begins
    [SerializeField] private AudioClip anarchyDroneClip;
    
    // Newspaper printing press SFX
    [SerializeField] private AudioClip gameOverClip;

    [Header("Music")]
    // Single low-tempo jazz/bureaucratic loop
    [SerializeField] private AudioClip backgroundMusicClip;

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

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.spatialBlend = 0f;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.spatialBlend = 0f;
            musicSource.loop = true;
        }

        sfxSource.ignoreListenerPause = true;
        musicSource.ignoreListenerPause = true;
    }

    void Start()
    {
        if (backgroundMusicClip != null)
        {
            PlayMusic(backgroundMusicClip);
        }
    }

    public void PlayCrimeSpawn() => PlaySFX(crimeSpawnClip);
    public void PlayIconClick() => PlaySFX(iconClickClip);
    public void PlayRaidAction() => PlaySFX(raidActionClip);
    public void PlayIgnoreAction() => PlaySFX(ignoreActionClip);
    public void PlayEscalationSpawn() => PlaySFX(escalationSpawnClip);
    public void PlayAnarchyDrone(bool play)
    {
        if (play)
        {
            if (musicSource.clip != anarchyDroneClip)
                PlayMusic(anarchyDroneClip);
        }
        else
        {
            if (musicSource.clip == anarchyDroneClip)
                PlayMusic(backgroundMusicClip);
        }
    }
    public void PlayGameOver() => PlaySFX(gameOverClip); 

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play a null SFX clip. Did you assign it in the Inspector?");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning($"AudioManager: sfxSource is null! Cannot play {clip.name}");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Attempted to play a null Music clip. Did you assign it in the Inspector?");
            return;
        }

        if (musicSource == null)
        {
            Debug.LogWarning($"AudioManager: musicSource is null! Cannot play {clip.name}");
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
}
