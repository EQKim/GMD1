using UnityEngine;
using UnityEngine.UI;

public class PlayerLivesUI : MonoBehaviour
{
    [SerializeField] private Image[] lives;

    [SerializeField] private Color aliveColor = Color.blue;
    [SerializeField] private Color deadColor = Color.black;

    public void UpdateLives(int currentLives)
    {
        for (int i = 0; i < lives.Length; i++)
        {
            lives[i].color = i < currentLives ? aliveColor : deadColor;
        }
    }
}