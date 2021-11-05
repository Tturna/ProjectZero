using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool canOpen;
    [SerializeField] private bool isOpen;
    [SerializeField] private int openCost;
    [SerializeField] List<int> adjacentZones = new List<int>();

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
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("open");

        // Disable collider after animation
        StartCoroutine(DisableCollider(2));

        // Add adjacent zones to unlocked zones in the game manager
        // This allows enemies to spawn there
        foreach (int zone in adjacentZones)
        {
            if (!GameManager.unlockedZones.Contains(zone))
            {
                GameManager.unlockedZones.Add(zone);
            }
        }
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

    IEnumerator DisableRigidbody(float delay)
    {
        yield return new WaitForSeconds(delay);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        rb.sleepMode = RigidbodySleepMode2D.StartAsleep;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Find adjacent zones and "disable" rigidbody
        if (collision.gameObject.tag == "Zone")
        {
            if (adjacentZones.Count == 0)
            {
                StartCoroutine(DisableRigidbody(1f));
            }

            int index = collision.gameObject.GetComponent<MapZone>().zoneIndex;

            if (!adjacentZones.Contains(index)) adjacentZones.Add(index);
        }
    }
}
