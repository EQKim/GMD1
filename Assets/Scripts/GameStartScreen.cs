using UnityEngine;
using TMPro;

public class GameStartScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject volumeSettings;

    [Header("Players")]
    [SerializeField] private PlayerController2D player1;
    [SerializeField] private PlayerController2D player2;
    [SerializeField] private PlayerHealth player1Health;
    [SerializeField] private PlayerHealth player2Health;

    [Header("Spawn Platforms")]
    [SerializeField] private SpawnPlatform spawnPlatformLeft;
    [SerializeField] private SpawnPlatform spawnPlatformRight;

    [Header("Title")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private string defaultTitle = "NutCracker";

    private void Start()
    {
        ShowMainMenuState();

        if (player1 != null)
            player1.SetControllable(false);

        if (player2 != null)
            player2.SetControllable(false);
    }

    public void StartGame()
    {
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
    }

    public void ReturnToStartScreen()
    {
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
    }
}