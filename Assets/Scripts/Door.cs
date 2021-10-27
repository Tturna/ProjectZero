using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool canOpen;
    [SerializeField] private bool isOpen;
    [SerializeField] private int openCost;

    Player player;
    SpriteRenderer promptRenderer;

    private void Start()
    {
        // Find prompt renderer
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in renderers)
        {
            if (sr.gameObject.name == "Prompt")
            {
                promptRenderer = sr;
                break;
            }
        }

        promptRenderer.enabled = false;

        VicinityManager vm = FindObjectOfType<VicinityManager>();
        vm.OnEnterNearDoor += OnEnterNearDoor;
        vm.OnExitNearDoor += OnExitNearDoor;

        gameObject.name = string.Format("Door ({0})", isOpen ? "Open" : "Closed");
    }

    private void Update()
    {
        if (!isOpen && canOpen && Input.GetKeyDown(KeyCode.F))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        // Check if the player has sufficient currency
        // If so, remove currency from the player
        if (!player) player = FindObjectOfType<Player>();

        if (player.currency >= openCost) player.AddCurrency(-openCost);
        else return;

        isOpen = true;
        gameObject.name = string.Format("Door ({0})", isOpen ? "Open" : "Closed");
        promptRenderer.enabled = false;

        // Play animation
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("open");

        // Disable collider after animation
        StartCoroutine(DisableCollider(2));
    }

    IEnumerator DisableCollider(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void OnEnterNearDoor(GameObject door)
    {
        if (door != gameObject) return;

        canOpen = true;
        if (!isOpen)
        {
            promptRenderer.enabled = true;
        }
    }

    void OnExitNearDoor(GameObject door)
    {
        if (door != gameObject) return;

        canOpen = false;
        promptRenderer.enabled = false;
    }
}
