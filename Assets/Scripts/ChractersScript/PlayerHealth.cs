using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
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

    [Header("Audio Clips (drag & drop)")]
    [SerializeField] private AudioClip hurtSfx;
    [SerializeField] private AudioClip healSfx;

    [Tooltip("Extra multiplier just for this character (0..1).")]
    [Range(0f, 1f)]
    [SerializeField] private float localSfxVolume = 1f;

    [Header("Global SFX Volume (saved)")]
    [Tooltip("If assigned, this slider will control global SFX volume (saved). Optional.")]
    [SerializeField] private Slider sfxVolumeSlider;

    [Tooltip("PlayerPrefs key used to save the global SFX volume.")]
    [SerializeField] private string sfxVolumePrefKey = "SFX_VOLUME";

    private AudioSource audioSource;

    // Cached global volume (0..1)
    private float globalSfxVolume = 1f;

    private bool isRespawning;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D sound by default

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
            PlaySfx(hurtSfx);
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
            PlaySfx(healSfx);
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (respawnDelay > 0f)
            yield return new WaitForSeconds(respawnDelay);

        // Restore health + update UI
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        // Respawn on platform (platform will re-enable + fade again)
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

        float volume = Mathf.Clamp01(globalSfxVolume * localSfxVolume);
        if (volume <= 0f) return;

        audioSource.PlayOneShot(clip, volume);
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

        // Initialize UI without triggering the callback twice
        sfxVolumeSlider.SetValueWithoutNotify(globalSfxVolume);

        sfxVolumeSlider.onValueChanged.AddListener(OnSfxSliderChanged);
    }

    private void OnSfxSliderChanged(float value)
    {
        globalSfxVolume = Mathf.Clamp01(value);
        SaveGlobalSfxVolume();
    }

    // Optional: call this from other scripts if you want to set volume without UI
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