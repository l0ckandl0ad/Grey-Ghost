using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private PlayerInput playerInput;

    private bool originalIsPausedState = true;
    private string gameplayActionMapName = "Main";
    private string pauseMenuActionMapName = "PauseMenu";

    private void Awake()
    {
        pauseMenu.SetActive(false);
    }
    public void OpenPauseMenu(PlayerInput playerInput)
    {
        pauseMenu.SetActive(true);
        this.playerInput = playerInput;
        playerInput.SwitchCurrentActionMap(pauseMenuActionMapName);
        Time.timeScale = 0f;
    }

    public void Resume(bool resumePaused)
    {
        pauseMenu.SetActive(false);
        playerInput.SwitchCurrentActionMap(gameplayActionMapName);
        Time.timeScale = 1f;
    }

    public void Save()
    {
        Debug.LogWarning("SAVE IS NOT IMPLEMENTED YET!");
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        playerInput.SwitchCurrentActionMap(gameplayActionMapName);
        Time.timeScale = 1f;
    }
    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
