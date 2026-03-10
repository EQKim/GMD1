using UnityEngine;
using UnityEngine.InputSystem;

public class GameStartScreen : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private PlayerController2D player1;
    [SerializeField] private PlayerController2D player2;
    [SerializeField] private SpawnPlatform spawnPlatformLeft;
    [SerializeField] private SpawnPlatform spawnPlatformRight;

    private bool gameStarted = false;

    private void Start()
    {
        startScreen.SetActive(true);

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

    private void StartGame()
    {
        gameStarted = true;

        startScreen.SetActive(false);

        player1.SetControllable(true);
        player2.SetControllable(true);

        if (spawnPlatformLeft != null)
            spawnPlatformLeft.StartPlatformSequence();

        if (spawnPlatformRight != null)
            spawnPlatformRight.StartPlatformSequence();
    }
}