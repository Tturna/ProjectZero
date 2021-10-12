using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    SceneTransitionManager sceneManager;

    bool isPaused;

    private void Start()
    {
        sceneManager = GetComponentInChildren<SceneTransitionManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchPause();
        }
    }

    void SwitchPause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
    }

    public void ResumeGame()
    {
        SwitchPause();
    }

    public void ReturnToMenu()
    {
        sceneManager.TransitionToScene(0);
    }
}
