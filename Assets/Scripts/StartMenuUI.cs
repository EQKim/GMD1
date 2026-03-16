using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject volumeSettings;

    [Header("First Selected")]
    [SerializeField] private Selectable firstMainMenuSelection;
    [SerializeField] private Selectable firstSettingsSelection;

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        volumeSettings.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        if (firstMainMenuSelection != null)
            EventSystem.current.SetSelectedGameObject(firstMainMenuSelection.gameObject);
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        volumeSettings.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        if (firstSettingsSelection != null)
            EventSystem.current.SetSelectedGameObject(firstSettingsSelection.gameObject);
    }

    public void StartGame()
    {
        gameObject.SetActive(false);
    }
}