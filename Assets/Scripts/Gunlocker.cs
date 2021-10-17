using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunlocker : MonoBehaviour
{
    private bool canOpen;
    private bool canGetGun;
    [SerializeField] private bool isOpen;
    [SerializeField] private float gunPickupDelay;

    public WeaponScriptableObject weaponSO;

    Player player;
    MeshRenderer promptRenderer;
    SpriteRenderer gunRenderer;

    void Start()
    {
        // Initializing variables
        promptRenderer = GetComponentInChildren<MeshRenderer>();
        promptRenderer.enabled = false;

        // Find gun renderer
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs) { if (sr.gameObject != gameObject) { gunRenderer = sr; break; } }
        gunRenderer.sprite = weaponSO.sprite;

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
            else if (canGetGun)
            {
                GetGun();
            }
        }
    }

    private void OpenLocker()
    {
        // Check if the player has a key
        if (!player) player = FindObjectOfType<Player>();

        // Check if player has key

        isOpen = true;
        gameObject.name = string.Format("Gun Locker ({0})", isOpen ? "Open" : "Closed");
        GetComponent<Animator>().SetTrigger("open");
        promptRenderer.enabled = false;

        // Allow the gun to be picked up after a given delay
        StartCoroutine(EnableGunPickup(gunPickupDelay));
    }

    private void GetGun()
    {
        player.GetComponentInChildren<Weapon>().AddWeapon(new Weapon.WeaponStats(weaponSO, weaponSO.ammoCapacity, weaponSO.defaultReserveAmmo));
        gunRenderer.sprite = null;
        canGetGun = false;
        promptRenderer.enabled = false;
    }

    IEnumerator EnableGunPickup(float delay)
    {
        yield return new WaitForSeconds(delay);

        promptRenderer.enabled = true;
        canGetGun = true;
    }

    void OnEnterNearLocker(GameObject locker)
    {
        canOpen = true;
        if (!isOpen || canGetGun) promptRenderer.enabled = true;
    }

    void OnExitNearLocker(GameObject locker)
    {
        canOpen = false;
        promptRenderer.enabled = false;
    }
}
