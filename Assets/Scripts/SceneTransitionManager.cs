// This script assumes that it's a component of the Canvas object

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    // TODO: Singleton(?)

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TransitionToScene(int index)
    {
        // Load scene after fading to black
        StartCoroutine(FadeToScene(index));
    }

    IEnumerator FadeToScene(int index)
    {
        // Fade the screen to black, wait for the animation to complete and then load scene
        animator.SetTrigger("StartFade");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        SceneManager.LoadScene(index);
    }
}
