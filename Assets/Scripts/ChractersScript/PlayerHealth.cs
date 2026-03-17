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

    [Header("Lives")]
    [SerializeField] private int maxLives = 3;
    public int MaxLives => maxLives;

    public int CurrentLives { get; private set; }

    public System.Action<int, int> OnLivesChanged;
    [SerializeField] private PlayerLivesUI livesUI;

    [Header("Respawn")]
    [SerializeField] private SpawnPlatform respawnPlatform;
    [SerializeField] private float respawnDelay = 0.1f;

    [Header("Game")]
    [SerializeField] private GameManager gameManager;

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

    [Header("Hurtbox Control")]
    [Tooltip("Optional. If left empty, all Collider2D components on this object and children except HitBox layers will be used.")]
    [SerializeField] private Collider2D[] hurtboxColliders;

    private float globalSfxVolume = 1f;
    private bool isRespawning;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentLives = maxLives;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnLivesChanged?.Invoke(CurrentLives, maxLives);

        if (livesUI != null)
            livesUI.UpdateLives(CurrentLives);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
        }
        else
        {
            Debug.LogWarning($"PlayerHealth on '{gameObject.name}' has no AudioSource assigned.");
        }

        CacheHurtboxesIfNeeded();
        SetHurtboxesEnabled(true);

        LoadGlobalSfxVolume();
        HookupSliderIfPresent();
    }

    private void OnDestroy()
    {
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);
    }

    public bool TakeDamage(int amount)
    {
        if (amount <= 0) return false;
        if (CurrentHealth <= 0) return false;
        if (isRespawning) return false;
        if (CurrentLives <= 0) return false;

        int prev = CurrentHealth;
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth < prev)
            PlaySfx(GetRandomClip(hurtSfxClips));

        if (CurrentHealth == 0)
        {
            isRespawning = true;
            SetHurtboxesEnabled(false);
            StartCoroutine(RespawnRoutine());
        }

        return CurrentHealth < prev;
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        if (CurrentHealth <= 0) return;
        if (isRespawning) return;
        if (CurrentLives <= 0) return;

        int prev = CurrentHealth;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth > prev)
            PlaySfx(GetRandomClip(healSfxClips));
    }

    private IEnumerator RespawnRoutine()
    {
        LoseLife();

        if (CurrentLives <= 0)
        {
            if (gameManager != null)
                gameManager.HandlePlayerDefeated(this);

            gameObject.SetActive(false);
            yield break;
        }

        if (respawnDelay > 0f)
            yield return new WaitForSeconds(respawnDelay);

        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (respawnPlatform != null)
        {
            respawnPlatform.RespawnPlayerWithPlatform();
        }
        else
        {
            Debug.LogError("PlayerHealth: respawnPlatform not assigned.");
        }

        SetHurtboxesEnabled(true);
        isRespawning = false;
    }

    private void LoseLife()
    {
        CurrentLives = Mathf.Max(0, CurrentLives - 1);
        OnLivesChanged?.Invoke(CurrentLives, maxLives);

        if (livesUI != null)
            livesUI.UpdateLives(CurrentLives);
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
            AudioClip c = clips[Random.Range(0, clips.Length)];
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
            sfxVolumeSlider.SetValueWithoutNotify(globalSfxVolume);
    }

    public float GetGlobalSfxVolume()
    {
        return globalSfxVolume;
    }

    public void ResetPlayer()
    {
        StopAllCoroutines();
        isRespawning = false;

        CurrentLives = maxLives;
        CurrentHealth = maxHealth;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnLivesChanged?.Invoke(CurrentLives, maxLives);

        if (livesUI != null)
            livesUI.UpdateLives(CurrentLives);

        gameObject.SetActive(true);
        SetHurtboxesEnabled(true);

        if (respawnPlatform != null)
            respawnPlatform.RespawnPlayer();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.Sleep();
        }
    }

    private void CacheHurtboxesIfNeeded()
    {
        if (hurtboxColliders != null && hurtboxColliders.Length > 0)
            return;

        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>(true);
        int hitBoxLayer = LayerMask.NameToLayer("HitBox (Attack Collider)");
        int fallbackHitBoxLayer = LayerMask.NameToLayer("Hitbox");

        System.Collections.Generic.List<Collider2D> result = new System.Collections.Generic.List<Collider2D>();

        foreach (Collider2D col in allColliders)
        {
            if (col == null)
                continue;

            int layer = col.gameObject.layer;

            if (layer == hitBoxLayer || layer == fallbackHitBoxLayer)
                continue;

            result.Add(col);
        }

        hurtboxColliders = result.ToArray();
    }

    private void SetHurtboxesEnabled(bool enabled)
    {
        if (hurtboxColliders == null)
            return;

        for (int i = 0; i < hurtboxColliders.Length; i++)
        {
            if (hurtboxColliders[i] != null)
                hurtboxColliders[i].enabled = enabled;
        }
    }
}