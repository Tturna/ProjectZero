using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using System.Collections;

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
    private Animator animator;

    // Weapons
    [SerializeField] private WeaponScriptableObject[] weapons;
    private WeaponStats[] weaponStats = new WeaponStats[2];
    public WeaponStats currentWeapon;

    // Slot selection
    private int selectedSlotIdx;

    // Monitoring
    [SerializeField] private bool showDebug;

    void Awake()
    {
        // Define variables
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

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

        // Change weapon animations
        if (currentWeapon.weaponSO.animationController) animator.runtimeAnimatorController = currentWeapon.weaponSO.animationController;
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

        WeaponScriptableObject so = currentWeapon.weaponSO;

        Vector2 startPoint = (Vector2)transform.position + so.muzzlePoint;

        // Raycast from the weapon position towards the target (until infinity?) to find collisions
        RaycastHit2D hit = Physics2D.Raycast(startPoint, target - startPoint);

        // Get collision point or if there was no collision, get a point far in the direction of the target
        Vector2 hitPoint = Vector2.zero;
        float shotLength = 50f;
        if (hit)
        {
            if (showDebug) Debug.Log("Hit: " + hit.collider.gameObject.name);

            hitPoint = hit.point;
            shotLength = (hitPoint - startPoint).magnitude;

            // Check if an enemy was hit
            if (hit.transform.tag == "Enemy")
            {
                // Damage the enemy
                hit.transform.GetComponent<Enemy>().ReceiveDamage(so.damage, transform.root.gameObject);
            }
        }
        else
        {
            hitPoint = (target - startPoint) * 100;
        }

        // Reduce ammo and update the ammo UI
        ReduceWeaponAmmo(1);

        // Instantiate and initialize a projectile prefab and a muzzle flash prefab for VFX
        GameObject projectile = Instantiate(so.projectilePrefab, (Vector2)transform.position, transform.parent.rotation);
        GameObject muzzleFlash = Instantiate(so.muzzleFlashPrefab, (Vector2)transform.position, transform.parent.rotation);

        // Projectile position to muzzle point
        projectile.transform.localPosition += Vector3.up * so.muzzlePoint.y;
        projectile.transform.Translate(Vector3.right * so.muzzlePoint.x, Space.Self);

        // Muzzle flash position to muzzle point
        muzzleFlash.transform.localPosition += Vector3.up * so.muzzlePoint.y;
        muzzleFlash.transform.Translate(Vector3.right * so.muzzlePoint.x, Space.Self);
        muzzleFlash.transform.SetParent(transform);

        // Projectile colors
        projectile.GetComponent<SpriteRenderer>().color = so.projectileColor;
        projectile.GetComponentInChildren<Light2D>().color = so.projectileColor;

        // Muzzle flash color
        muzzleFlash.GetComponent<SpriteRenderer>().color = so.muzzleFlashColor;
        muzzleFlash.GetComponent<Light2D>().color = so.projectileColor;

        // Destroy muzzle flash after given time
        StartCoroutine(DelayDestroyObject(muzzleFlash, so.muzzleFlashLifetime));

        Projectile projectileSettings = projectile.GetComponent<Projectile>();

        // Calculate projectile lifetime
        float lifeTime = shotLength / so.projectileSpeed;
        //Debug.Log(string.Format("{0} / {1} = {2}",shotLength, currentWeapon.weaponSO.projectileSpeed, lifeTime));

        projectileSettings.Initialize(so.projectileSpeed, lifeTime);

        // Play weapon animation
        animator.SetTrigger("attack");
    }

    public int GetSelectedSlotIndex() { return selectedSlotIdx; }

    private IEnumerator DelayDestroyObject(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(target);
    }
}
