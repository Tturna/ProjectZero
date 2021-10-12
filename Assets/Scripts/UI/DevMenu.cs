using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevMenu : MonoBehaviour
{
    [SerializeField] GameObject menu;

    bool isOpen;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SwitchMenu();
        }
    }

    void SwitchMenu()
    {
        isOpen = !isOpen;
        menu.SetActive(isOpen);
    }
}
