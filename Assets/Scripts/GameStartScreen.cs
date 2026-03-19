using UnityEngine;
using TMPro;
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

    [Header("Title")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private string defaultTitle = "NutCracker";

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

        PlayMusic(matchMusic);
    }

    public void ShowWinnerScreen(string winnerName)
    {
        if (startScreen != null)
            startScreen.SetActive(true);

        if (mainMenu != null)
            mainMenu.SetActive(false);

        if (volumeSettings != null)
            volumeSettings.SetActive(false);

        if (titleText != null)
            titleText.text = winnerName + " Wins";

        if (player1 != null)
            player1.SetControllable(false);

        if (player2 != null)
            player2.SetControllable(false);

        ForceStopCombatOnly();
    }

    public void ReturnToStartScreen()
    {
        ForceResetRoundState();
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

        if (titleText != null)
            titleText.text = defaultTitle;

        ReselectMainMenuButton();
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
            Destroy(bullets[i]);
    }

    private void ForceStopCombatOnly()
    {
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