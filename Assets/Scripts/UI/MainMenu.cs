// This script assumes that it's a component of the Canvas object
// This script also assumes that the SceneTransitionManager script is a component of the same Canvas object

using UnityEngine;

public class MainMenu : MonoBehaviour
{
    SceneTransitionManager sceneManager;

    private void Start()
    {
        sceneManager = GetComponentInChildren<SceneTransitionManager>();
    }

    public void StartGame()
    {
        // TODO: Make singleton pattern maybe?
        sceneManager.TransitionToScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
