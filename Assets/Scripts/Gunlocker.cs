using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunlocker : MonoBehaviour
{
    private bool canOpen;
    [SerializeField] private bool isOpen;

    Player player;
    MeshRenderer promptRenderer;

    void Start()
    {
        promptRenderer = GetComponentInChildren<MeshRenderer>();
        promptRenderer.enabled = false;

        VicinityManager vm = FindObjectOfType<VicinityManager>();
        vm.OnEnterNearLocker += OnEnterNearLocker;
        vm.OnExitNearLocker += OnExitNearLocker;

        gameObject.name = string.Format("Gun Locker ({0})", isOpen ? "Open" : "Closed");
    }

    void Update()
    {
        if (!isOpen && canOpen && Input.GetKeyDown(KeyCode.F))
        {
            OpenLocker();
        }
    }

    public void OpenLocker()
    {
        // Check if the player has a key
        if (!player) player = FindObjectOfType<Player>();

        // Check if player has key

        isOpen = true;
        gameObject.name = string.Format("Gun Locker ({0})", isOpen ? "Open" : "Closed");
        GetComponent<Animator>().SetTrigger("open");
        promptRenderer.enabled = false;
    }

    void OnEnterNearLocker(GameObject locker)
    {
        canOpen = true;
        if (!isOpen) promptRenderer.enabled = true;
    }

    void OnExitNearLocker(GameObject locker)
    {
        canOpen = false;
        promptRenderer.enabled = false;
    }
}
