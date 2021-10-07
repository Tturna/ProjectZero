using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player settings
    [Header("Movement Settings")]
    [SerializeField, Tooltip("Speed multiplier when sprinting")] private float sprintMultiplier;
    [SerializeField, Tooltip("Energy cost for sprinting each FIXED frame")] private float sprintCost;
    [SerializeField, Tooltip("Delay in seconds after sprinting before stamina start to regenerate")] private float staminaRegenDelay;
    [SerializeField, Tooltip("Energy regeneration amount per FIXED frame")] private float staminaRegenAmount;

    public int currency;

    // General
    private Camera mainCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private HUD hud;
    private Stats stats;

    [Header("General")]

    // Weapon
    [SerializeField] private Transform weaponAnchor;
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private LineRenderer gunRay;
    private SpriteRenderer weaponRenderer;
    private Weapon weapon;
    
    // Movement & Aiming
    private Rigidbody2D playerBody;
    private Vector2 moveDir;
    private bool sprinting;
    private bool lookingLeft;
    private bool regeneratingStamina;

    void Start()
    {
        // Define variables
        mainCamera = Camera.main;
        hud = FindObjectOfType<HUD>();
        stats = new Stats(100f, 100f, 1000f, 100f, 100f, 25f, 0f);
        playerBody = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
        weaponRenderer = weaponObject.GetComponentInChildren<SpriteRenderer>();

        // Update ammo UI
        hud.UpdateAmmoUI(weapon?.currentWeapon?.weaponAmmo ?? 0, weapon?.currentWeapon?.reserveAmmo ?? 0);
    }

    void Update()
    {
        #region Movement Controls

        // Basic movement
        moveDir.x = Input.GetAxis("Horizontal"); // TODO: Axis Raw?
        moveDir.y = Input.GetAxis("Vertical");

        // Sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // Start sprinting if the player has energy
            if (stats.energy > 0)
            {
                sprinting = true;
                regeneratingStamina = false; // Stop regenerating stamina when sprinting
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            sprinting = false;

            // When the player stops sprinting, start a delay after which energy will start to regenerate
            StartCoroutine(StartStaminaRegeneration(staminaRegenDelay));
        }

        #endregion Movement Controls

        #region Shooting, aiming and reloading

        // Mouse position
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Calculate angle between player and cursor position
        Vector2 diff = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        // Set weapon anchor angle
        Vector3 rot = weaponAnchor.localEulerAngles;
        rot.z = angle;
        weaponAnchor.localEulerAngles = rot;

        // Update player animations depending on point of aim
        lookingLeft = mousePos.x < transform.position.x;
        animator.SetBool("lookingLeft", lookingLeft);

        // Flip the weapon on the Y axis when aiming left
        weaponRenderer.flipY = lookingLeft;

        // Shooting (using a Line Renderer for monitoring)
        if (Input.GetMouseButtonDown(0))
        {
            // Fire
            weapon.Shoot(mousePos);
            hud.UpdateAmmoUI(weapon.currentWeapon.weaponAmmo, weapon.currentWeapon.reserveAmmo);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload
            weapon.ReloadCurrentWeapon();
            hud.UpdateAmmoUI(weapon.currentWeapon.weaponAmmo, weapon.currentWeapon.reserveAmmo);
        }

        #endregion

        #region Weapon Swapping

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weapon.SelectSlot(0);
            hud.UpdateSelectedWeaponUI(weapon.GetSelectedSlotIndex());
            hud.UpdateAmmoUI(weapon.currentWeapon.weaponAmmo, weapon.currentWeapon.reserveAmmo);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weapon.SelectSlot(1);
            hud.UpdateSelectedWeaponUI(weapon.GetSelectedSlotIndex());
            hud.UpdateAmmoUI(weapon.currentWeapon.weaponAmmo, weapon.currentWeapon.reserveAmmo);
        }

        if (Input.mouseScrollDelta.y != 0f)
        {
            weapon.SwapSlot();
            hud.UpdateSelectedWeaponUI(weapon.GetSelectedSlotIndex());
            hud.UpdateAmmoUI(weapon.currentWeapon.weaponAmmo, weapon.currentWeapon.reserveAmmo);
        }

        #endregion
    }

    private void FixedUpdate()
    {
        // Calculate movement direction and speed
        if (moveDir.magnitude > 0f)
        {
            // Sprint speed multiplier
            // Changes movement speed when holding the sprint key
            float multiplier = 1f;

            // Prevent sprinting if out of energy
            if (sprinting && stats.energy > 0f)
            {
                // Reduce energy when sprinting
                multiplier = sprintMultiplier;
                stats.energy = Mathf.Clamp(stats.energy - sprintCost, 0f, stats.maxEnergy);

                // Update stamina bar UI
                hud.UpdateStaminaUI(stats.energy / stats.maxEnergy);
            }
            else if (sprinting && stats.energy == 0f)
            {
                // If the player runs out of energy, start a delay after which it will regenerate
                sprinting = false;
                StartCoroutine(StartStaminaRegeneration(staminaRegenDelay));
            }

            playerBody.AddForce(moveDir.normalized * stats.movementSpeed * multiplier);
        }

        #region Updating Statistics (Health & Stamina regen...)

        // NOTE: This is done in Fixed Update so stats update at the same speed on different machines.
        // Alternatively, we could do this in Update() and multiply everything with Time.deltaTime, but I don't think it makes a difference.

        // Stamina regeneration
        if (regeneratingStamina)
        {
            if (stats.energy < stats.maxEnergy)
            {
                stats.energy = Mathf.Clamp(stats.energy + staminaRegenAmount, 0f, stats.maxEnergy);

                // Update stamina bar UI
                hud.UpdateStaminaUI(stats.energy / stats.maxEnergy);
            }
            else regeneratingStamina = false;
        }

        // Update player animations when moving and not moving
        animator.SetBool("walking", moveDir.magnitude > 0f);

        #endregion
    }

    // Temp coroutine for monitoring
    IEnumerator DeactivateGunRay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gunRay.enabled = false;
    }

    IEnumerator StartStaminaRegeneration(float delay)
    {
        yield return new WaitForSeconds(delay);
        regeneratingStamina = true;
    }

    public void ReceiveDamage(float amount)
    {
        // Reduce health from the player unless it's already 0
        if (stats.health > 0)
        {
            stats.health = Mathf.Clamp(stats.health - amount, 0f, stats.maxHealth);
            Debug.Log("Player took" + amount.ToString() + " damage. " + stats.health.ToString() + " HP left.");

            // Update health bar UI
            hud.UpdateHealthUI(stats.health / stats.maxHealth);

            // If health reaches 0, kill the player
            if (stats.health == 0) Death();
        }
        else Death();
    }

    public void AddCurrency(int amount)
    {
        currency += amount;
        hud.UpdateScoreUI(currency);
    }

    private void Death()
    {
        Debug.Log("Death");
    }
}
