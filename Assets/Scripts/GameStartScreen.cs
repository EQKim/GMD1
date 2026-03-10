using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameStartScreen : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private PlayerController2D player1;
    [SerializeField] private PlayerController2D player2;
    [SerializeField] private SpawnPlatform spawnPlatformLeft;
    [SerializeField] private SpawnPlatform spawnPlatformRight;
    [SerializeField] private PlayerHealth player1Health;
    [SerializeField] private PlayerHealth player2Health;

    [Header("Start Screen Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text pressStartText;
    [SerializeField] private string defaultTitle = "NutCracker";

    private bool gameStarted = false;

    private void Start()
    {
        startScreen.SetActive(true);

        if (titleText != null)
            titleText.text = defaultTitle;

        if (pressStartText != null)
            pressStartText.gameObject.SetActive(true);

        player1.SetControllable(false);
        player2.SetControllable(false);
    }

    private void Update()
    {
        if (gameStarted)
            return;

        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StartGame();
            return;
        }

        if (Gamepad.all.Count > 0)
        {
            foreach (var pad in Gamepad.all)
            {
                if (pad.buttonSouth.wasPressedThisFrame ||
                    pad.buttonEast.wasPressedThisFrame ||
                    pad.startButton.wasPressedThisFrame)
                {
                    StartGame();
                    return;
                }
            }
        }
    }

    public void StartGame()
    {
        gameStarted = true;

        startScreen.SetActive(false);

        if (titleText != null)
            titleText.text = defaultTitle;

        if (pressStartText != null)
            pressStartText.gameObject.SetActive(true);

        if (player1Health != null)
            player1Health.ResetPlayer();

        if (player2Health != null)
            player2Health.ResetPlayer();

        player1.SetControllable(true);
        player2.SetControllable(true);

        if (spawnPlatformLeft != null)
            spawnPlatformLeft.StartPlatformSequence();

        if (spawnPlatformRight != null)
            spawnPlatformRight.StartPlatformSequence();
    }

    public void ShowWinnerScreen(string winnerName)
    {
        gameStarted = true;

        startScreen.SetActive(true);

        if (titleText != null)
            titleText.text = winnerName + " Wins";

        if (pressStartText != null)
            pressStartText.gameObject.SetActive(false);

        player1.SetControllable(false);
        player2.SetControllable(false);
    }

    public void ReturnToStartScreen()
    {
        gameStarted = false;

        startScreen.SetActive(true);

        if (titleText != null)
            titleText.text = defaultTitle;

        if (pressStartText != null)
            pressStartText.gameObject.SetActive(true);

        player1.SetControllable(false);
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
}