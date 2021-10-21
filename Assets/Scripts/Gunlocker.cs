using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunlocker : MonoBehaviour
{
    private bool canOpen;
    private bool canGetGun;
    [SerializeField] private bool isOpen;
    [SerializeField] private float gunPickupDelay;

    // Weapon
    public WeaponScriptableObject weaponSO;
    int ammo;
    int reserveAmmo;

    // General
    Player player;
    [SerializeField] SpriteRenderer promptRenderer;
    SpriteRenderer gunRenderer;

    void Start()
    {
        // Initializing variables
        promptRenderer.enabled = false;
        ammo = weaponSO.ammoCapacity;
        reserveAmmo = weaponSO.defaultReserveAmmo;

        // Find gun renderer
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs) { if (sr.gameObject.name == "Gun") { gunRenderer = sr; break; } }
        gunRenderer.sprite = weaponSO.sprite;

        // Subscribe to events
        VicinityManager vm = FindObjectOfType<VicinityManager>();
        vm.OnEnterNearLocker += OnEnterNearLocker;
        vm.OnExitNearLocker += OnExitNearLocker;

        gameObject.name = string.Format("Gun Locker ({0})", isOpen ? "Open" : "Closed");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isOpen && canOpen)
            {
                OpenLocker();
            }
            else if (canOpen && canGetGun)
            {
                GetGun();
            }
        }
    }

    private void OpenLocker()
    {
        // Check if the player has a key
        if (!player) player = FindObjectOfType<Player>();

        if (player.lockerKeys > 0) player.lockerKeys--;
        else return; // TODO: Indication that the player has no keys
        
        // Open locker
        isOpen = true;
        gameObject.name = string.Format("Gun Locker ({0})", isOpen ? "Open" : "Closed");
        GetComponent<Animator>().SetTrigger("open");
        promptRenderer.enabled = false;

        // Update values on the player so the HUD updates when locker keys are used
        player.UpdateValues();

        // Allow the gun to be picked up after a given delay
        StartCoroutine(EnableGunPickup(gunPickupDelay));
    }

    // Give the weapon in the locker to the player
    // Put the player's old weapon in the locker
    private void GetGun()
    {
        Weapon.WeaponStats oldWeapon = player.GetComponentInChildren<Weapon>().AddWeapon(new Weapon.WeaponStats(weaponSO, ammo, reserveAmmo));
        gunRenderer.sprite = null;
        canGetGun = false;
        promptRenderer.enabled = false;

        // Put the old weapon into the locker
        if (oldWeapon != null)
        {
            gunRenderer.sprite = oldWeapon.weaponSO.sprite;
            weaponSO = oldWeapon.weaponSO;
            ammo = oldWeapon.weaponAmmo;
            reserveAmmo = oldWeapon.reserveAmmo;

            // Allow the old gun to be picked up again
            StartCoroutine(EnableGunPickup(gunPickupDelay));
        }
    }

    IEnumerator EnableGunPickup(float delay)
    {
        yield return new WaitForSeconds(delay);

        canGetGun = true;
        if (canOpen)
        {
            promptRenderer.enabled = true;
        }
    }

    void OnEnterNearLocker(GameObject locker)
    {
        // Make sure the locker only registers events meant for it and not the other lockers
        if (locker != gameObject) return;

        canOpen = true;
        if (!isOpen || canGetGun) promptRenderer.enabled = true;
    }

    void OnExitNearLocker(GameObject locker)
    {
        // Make sure the locker only registers events meant for it and not the other lockers
        if (locker != gameObject) return;

        canOpen = false;
        promptRenderer.enabled = false;
    }
}
