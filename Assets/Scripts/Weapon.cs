using UnityEngine;

public class Weapon : MonoBehaviour
{
    // Class to save data for each carried weapon
    public class WeaponStats
    {
        public WeaponScriptableObject weaponSO;
        public int weaponAmmo;
        public int reserveAmmo;
    }

    // General
    private SpriteRenderer spriteRenderer;

    // Weapons
    [SerializeField] private WeaponScriptableObject[] weapons;
    private WeaponStats[] weaponStats = new WeaponStats[2];
    public WeaponStats currentWeapon;

    // Slot selection
    private int selectedSlotIdx;

    void Awake()
    {
        // Define variables
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        for (int i = 0; i < weapons.Length; i++)
        {
            weaponStats[i] = new WeaponStats();

            if (!weapons[i]) continue;
            weaponStats[i].weaponSO = weapons[i];
            weaponStats[i].weaponAmmo = weapons[i].ammoCapacity;
            weaponStats[i].reserveAmmo = weapons[i].defaultReserveAmmo;
        }

        // Put a gun in the player's hand when starting
        UpdateSelectedWeapon();
    }

    // Update statistics and visuals when for the selected weapon
    void UpdateSelectedWeapon()
    {
        currentWeapon = weaponStats[selectedSlotIdx];

        // Change weapon sprite
        spriteRenderer.sprite = currentWeapon.weaponSO.sprite;
    }

    // Select a given slot
    public void SelectSlot(int index)
    {
        selectedSlotIdx = index;
        UpdateSelectedWeapon();
    }

    // Swap between the 2 slots
    public void SwapSlot()
    {
        selectedSlotIdx = selectedSlotIdx == 0 ? 1 : 0;
        UpdateSelectedWeapon();
    }

    // Reduce ammo from the current weapon
    public void ReduceWeaponAmmo(int amount)
    {
        currentWeapon.weaponAmmo -= amount;
    }

    // Reload ammo from the current weapon's reserve to the weapon itself
    public void ReloadCurrentWeapon()
    {
        currentWeapon.reserveAmmo += currentWeapon.weaponAmmo; // Unload ammo from the gun to the reserve
        currentWeapon.weaponAmmo = Mathf.Clamp(currentWeapon.reserveAmmo, 0, currentWeapon.weaponSO.ammoCapacity); // Use reserve to load the gun
        currentWeapon.reserveAmmo -= currentWeapon.weaponAmmo; // Subtract loaded ammo from the reserve
    }

    public void Shoot(Vector2 target)
    {
        // Check if out of ammo
        if (currentWeapon.weaponAmmo <= 0) return;

        Vector2 startPoint = (Vector2)transform.position + currentWeapon.weaponSO.muzzlePoint;

        // Raycast from the weapon position towards the target (until infinity?) to find collisions
        RaycastHit2D hit = Physics2D.Raycast(startPoint, target - startPoint);

        // Get collision point or if there was no collision, get a point far in the direction of the target
        Vector2 hitPoint = Vector2.zero;
        float shotLength = 50f;
        if (hit)
        {
            hitPoint = hit.point;
            shotLength = (hitPoint - startPoint).magnitude;

            // Check if an enemy was hit
            if (hit.transform.tag == "Enemy")
            {
                // Damage the enemy
                hit.transform.GetComponent<Enemy>().ReceiveDamage(currentWeapon.weaponSO.damage, gameObject);
            }
        }
        else
        {
            hitPoint = (target - startPoint) * 100;
        }

        // Reduce ammo and update the ammo UI
        ReduceWeaponAmmo(1);

        // Instantiate and initialize a projectile prefab for VFX
        GameObject projectile = Instantiate(currentWeapon.weaponSO.projectilePrefab, (Vector2)transform.position + currentWeapon.weaponSO.muzzlePoint, transform.parent.rotation);
        Projectile projectileSettings = projectile.GetComponent<Projectile>();

        // Calculate projectile lifetime
        float lifeTime = shotLength / currentWeapon.weaponSO.projectileSpeed;
        //Debug.Log(string.Format("{0} / {1} = {2}",shotLength, currentWeapon.weaponSO.projectileSpeed, lifeTime));

        projectileSettings.Initialize(currentWeapon.weaponSO.projectileSpeed, lifeTime);
    }

    public int GetSelectedSlotIndex() { return selectedSlotIdx; }
}
