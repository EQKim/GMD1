using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth;

    public int CurrentHealth { get; private set; }

    public System.Action<int, int> OnHealthChanged;

    [Header("Respawn")]
    [SerializeField] private SpawnPlatform respawnPlatform;
    [SerializeField] private float respawnDelay = 0.1f;

    [Header("Audio Source")]
    [Tooltip("Drag the AudioSource you want this health script to use.")]
    [SerializeField] private AudioSource audioSource;

    [Header("Audio Clips (drag & drop)")]
    [Tooltip("List of hurt SFX. One will be chosen at random.")]
    [SerializeField] private AudioClip[] hurtSfxClips;

    [Tooltip("List of heal SFX. One will be chosen at random.")]
    [SerializeField] private AudioClip[] healSfxClips;

    [Tooltip("Extra multiplier just for this character (0..1).")]
    [Range(0f, 1f)]
    [SerializeField] private float localSfxVolume = 1f;

    [Header("Global SFX Volume (saved)")]
    [Tooltip("If assigned, this slider will control global SFX volume (saved). Optional.")]
    [SerializeField] private Slider sfxVolumeSlider;

    [Tooltip("PlayerPrefs key used to save the global SFX volume.")]
    [SerializeField] private string sfxVolumePrefKey = "SFX_VOLUME";

    // Cached global volume (0..1)
    private float globalSfxVolume = 1f;

    private bool isRespawning;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f; // 2D sound by default
        }
        else
        {
            Debug.LogWarning($"PlayerHealth on '{gameObject.name}' has no AudioSource assigned.");
        }

        LoadGlobalSfxVolume();
        HookupSliderIfPresent();
    }

    private void OnDestroy()
    {
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        if (CurrentHealth <= 0) return;
        if (isRespawning) return;

        int prev = CurrentHealth;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth < prev)
        {
            PlaySfx(GetRandomClip(hurtSfxClips));
        }

        if (CurrentHealth == 0)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        if (CurrentHealth <= 0) return;
        if (isRespawning) return;

        int prev = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth > prev)
        {
            PlaySfx(GetRandomClip(healSfxClips));
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (respawnDelay > 0f)
            yield return new WaitForSeconds(respawnDelay);

        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (respawnPlatform != null)
        {
            respawnPlatform.RespawnPlayer();
        }
        else
        {
            Debug.LogError("PlayerHealth: respawnPlatform not assigned.");
        }

        isRespawning = false;
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource == null) return;

        float volume = Mathf.Clamp01(globalSfxVolume * localSfxVolume);
        if (volume <= 0f) return;

        audioSource.PlayOneShot(clip, volume);
    }

    private static AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;

        int attempts = clips.Length;
        while (attempts-- > 0)
        {
            var c = clips[Random.Range(0, clips.Length)];
            if (c != null) return c;
        }

        return null;
    }

    private void LoadGlobalSfxVolume()
    {
        globalSfxVolume = PlayerPrefs.GetFloat(sfxVolumePrefKey, 1f);
        globalSfxVolume = Mathf.Clamp01(globalSfxVolume);
    }

    private void SaveGlobalSfxVolume()
    {
        PlayerPrefs.SetFloat(sfxVolumePrefKey, globalSfxVolume);
        PlayerPrefs.Save();
    }

    private void HookupSliderIfPresent()
    {
        if (sfxVolumeSlider == null) return;

        sfxVolumeSlider.minValue = 0f;
        sfxVolumeSlider.maxValue = 1f;
        sfxVolumeSlider.wholeNumbers = false;

        sfxVolumeSlider.SetValueWithoutNotify(globalSfxVolume);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    private void OnSfxSliderChanged(float value)
    {
        globalSfxVolume = Mathf.Clamp01(value);
        SaveGlobalSfxVolume();
    }

    public void SetGlobalSfxVolume(float value01)
    {
        globalSfxVolume = Mathf.Clamp01(value01);
        SaveGlobalSfxVolume();

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.SetValueWithoutNotify(globalSfxVolume);
        }
    }

    public float GetGlobalSfxVolume()
    {
        return globalSfxVolume;
    }
}