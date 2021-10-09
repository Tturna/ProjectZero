using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool canOpen;
    [SerializeField] private bool isOpen;

    private void Start()
    {
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
        isOpen = true;
        gameObject.name = string.Format("Door ({0})", isOpen ? "Open" : "Closed");
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void OnEnterNearDoor(GameObject door)
    {
        canOpen = true;
    }

    void OnExitNearDoor(GameObject door)
    {
        canOpen = false;
    }
}
