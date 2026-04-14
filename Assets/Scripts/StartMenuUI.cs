using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class StartMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject volumeSettings;
    [SerializeField] private GameObject controlSettings;

    [Header("First Selected")]
    [SerializeField] private Selectable firstMainMenuSelection;
    [SerializeField] private Selectable firstSettingsSelection;
    [SerializeField] private Selectable firstControlsSelection;

    private Coroutine selectionRoutine;

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        volumeSettings.SetActive(false);
        controlSettings.SetActive(false);
        SelectWithDelay(firstMainMenuSelection);
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        volumeSettings.SetActive(true);
        controlSettings.SetActive(false);
        SelectWithDelay(firstSettingsSelection);
    }

    public void OpenControls()
    {
        mainMenu.SetActive(false);
        volumeSettings.SetActive(false);
        controlSettings.SetActive(true);
        SelectWithDelay(firstControlsSelection);
    }

    public void StartGame()
    {
        gameObject.SetActive(false);
    }

    private void SelectWithDelay(Selectable target)
    {
        if (selectionRoutine != null)
            StopCoroutine(selectionRoutine);

        selectionRoutine = StartCoroutine(SelectNextFrame(target));
    }

    private IEnumerator SelectNextFrame(Selectable target)
    {
        EventSystem.current.SetSelectedGameObject(null);

        yield return null;
        yield return null;

        if (target != null && target.gameObject.activeInHierarchy)
            EventSystem.current.SetSelectedGameObject(target.gameObject);
    }
}