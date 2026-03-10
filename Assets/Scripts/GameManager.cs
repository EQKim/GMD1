using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private PlayerHealth player1;
    [SerializeField] private PlayerHealth player2;

    [Header("Flow")]
    [SerializeField] private GameStartScreen gameStartScreen;
    [SerializeField] private float winnerScreenDuration = 5f;

    private bool matchEnded;

    public void HandlePlayerDefeated(PlayerHealth defeatedPlayer)
    {
        if (matchEnded)
            return;

        matchEnded = true;

        PlayerHealth winner = defeatedPlayer == player1 ? player2 : player1;

        var controller1 = player1.GetComponent<PlayerController2D>();
        var controller2 = player2.GetComponent<PlayerController2D>();

        if (controller1 != null)
            controller1.SetControllable(false);

        if (controller2 != null)
            controller2.SetControllable(false);

        if (gameStartScreen != null)
        {
            gameStartScreen.ShowWinnerScreen(winner.gameObject.name);
        }

        Time.timeScale = 0f;
        StartCoroutine(ReturnToStartMenuRoutine());
    }

    private IEnumerator ReturnToStartMenuRoutine()
    {
        yield return new WaitForSecondsRealtime(winnerScreenDuration);

        Time.timeScale = 1f;

        if (gameStartScreen != null)
        {
            gameStartScreen.ReturnToStartScreen();
        }

        matchEnded = false;
    }
}