using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GameStartScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject volumeSettings;

    [Header("Menu Selection")]
    [SerializeField] private Selectable firstMainMenuSelection;

    [Header("Players")]
    [SerializeField] private PlayerController2D player1;
    [SerializeField] private PlayerController2D player2;
    [SerializeField] private PlayerHealth player1Health;
    [SerializeField] private PlayerHealth player2Health;
    [SerializeField] private PlayerWeaponHolder player1WeaponHolder;
    [SerializeField] private PlayerWeaponHolder player2WeaponHolder;

    [Header("Spawn Platforms")]
    [SerializeField] private SpawnPlatform spawnPlatformLeft;
    [SerializeField] private SpawnPlatform spawnPlatformRight;

    [Header("Endless Platforms")]
    [SerializeField] private EndlessPlatformManager endlessPlatformManager;

    [Header("Enemies")]
    [SerializeField] private FlyingDemonSpawner flyingDemonSpawner;

    [Header("Winner Images")]
    [SerializeField] private Image winnerImage;
    [SerializeField] private Sprite homelessManWinsSprite;
    [SerializeField] private Sprite graffitiWomanWinsSprite;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip matchMusic;
    [SerializeField] private bool loopMusic = true;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    private Coroutine reselectionRoutine;

    private void Start()
    {
        ShowMainMenuState();
        ForceResetRoundState();

        if (endlessPlatformManager != null)
            endlessPlatformManager.StopRun();

        if (flyingDemonSpawner != null)
            flyingDemonSpawner.StopAndClear();

        if (player1 != null)
            player1.SetControllable(false);

        if (player2 != null)
            player2.SetControllable(false);

        PlayMusic(menuMusic);
    }

    public void StartGame()
    {
        ForceResetRoundState();

        if (startScreen != null)
            startScreen.SetActive(false);

        if (winnerImage != null)
            winnerImage.gameObject.SetActive(false);

        if (player1Health != null)
            player1Health.ResetPlayer();

        if (player2Health != null)
            player2Health.ResetPlayer();

        if (player1 != null)
            player1.SetControllable(true);

        if (player2 != null)
            player2.SetControllable(true);

        if (spawnPlatformLeft != null)
            spawnPlatformLeft.StartPlatformSequence();

        if (spawnPlatformRight != null)
            spawnPlatformRight.StartPlatformSequence();

        if (endlessPlatformManager != null)
            endlessPlatformManager.BeginRun();

        PlayMusic(matchMusic);

        if (flyingDemonSpawner != null)
            flyingDemonSpawner.BeginMatch();
    }

    public void ShowWinnerScreen(string winnerName)
    {
        if (startScreen != null)
            startScreen.SetActive(true);

        if (mainMenu != null)
            mainMenu.SetActive(false);

        if (volumeSettings != null)
            volumeSettings.SetActive(false);

        ShowWinnerImage(winnerName);

        if (player1 != null)
            player1.SetControllable(false);

        if (player2 != null)
            player2.SetControllable(false);

        ForceStopCombatOnly();

        if (endlessPlatformManager != null)
            endlessPlatformManager.StopRun();

        if (flyingDemonSpawner != null)
            flyingDemonSpawner.StopAndClear();
    }

    public void ReturnToStartScreen()
    {
        ForceResetRoundState();

        if (endlessPlatformManager != null)
            endlessPlatformManager.StopRun();

        if (flyingDemonSpawner != null)
            flyingDemonSpawner.StopAndClear();

        ShowMainMenuState();

        if (player1 != null)
            player1.SetControllable(false);

        if (player2 != null)
            player2.SetControllable(false);

        if (spawnPlatformLeft != null)
            spawnPlatformLeft.ResetPlatform();

        if (spawnPlatformRight != null)
            spawnPlatformRight.ResetPlatform();

        if (player1Health != null)
            player1Health.ResetPlayer();

        if (player2Health != null)
            player2Health.ResetPlayer();

        PlayMusic(menuMusic);
    }

    private void ShowMainMenuState()
    {
        if (startScreen != null)
            startScreen.SetActive(true);

        if (mainMenu != null)
            mainMenu.SetActive(true);

        if (volumeSettings != null)
            volumeSettings.SetActive(false);

        if (winnerImage != null)
            winnerImage.gameObject.SetActive(false);

        ReselectMainMenuButton();
    }

    private void ShowWinnerImage(string winnerName)
    {
        if (winnerImage == null)
            return;

        if (winnerName == "HomelessMan")
            winnerImage.sprite = homelessManWinsSprite;
        else if (winnerName == "GraffitiWoman")
            winnerImage.sprite = graffitiWomanWinsSprite;
        else
        {
            winnerImage.gameObject.SetActive(false);
            return;
        }

        winnerImage.gameObject.SetActive(true);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = loopMusic;
        musicSource.volume = Mathf.Clamp01(musicVolume);
        musicSource.Play();
    }

    private void DestroyAllBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");

        for (int i = 0; i < bullets.Length; i++)
        {
            if (bullets[i] != null)
                Destroy(bullets[i]);
        }
    }

    private void ForceStopCombatOnly()
    {
        DestroyAllBullets();

        if (player1 != null)
            player1.ResetCombatState();

        if (player2 != null)
            player2.ResetCombatState();

        if (player1WeaponHolder != null)
            player1WeaponHolder.StopRangedAttack();

        if (player2WeaponHolder != null)
            player2WeaponHolder.StopRangedAttack();
    }

    private void ForceResetRoundState()
    {
        DestroyAllBullets();

        if (player1 != null)
            player1.ResetCombatState();

        if (player2 != null)
            player2.ResetCombatState();

        if (player1WeaponHolder != null)
            player1WeaponHolder.RemoveWeapon();

        if (player2WeaponHolder != null)
            player2WeaponHolder.RemoveWeapon();
    }

    private void ReselectMainMenuButton()
    {
        if (reselectionRoutine != null)
            StopCoroutine(reselectionRoutine);

        reselectionRoutine = StartCoroutine(ReselectMainMenuButtonRoutine());
    }

    private IEnumerator ReselectMainMenuButtonRoutine()
    {
        yield return null;

        if (EventSystem.current == null || firstMainMenuSelection == null)
        {
            reselectionRoutine = null;
            yield break;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstMainMenuSelection.gameObject);

        reselectionRoutine = null;
    }
}