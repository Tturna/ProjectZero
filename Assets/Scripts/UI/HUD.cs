using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text currency;
    [SerializeField] private Text ammo;
    [SerializeField] private Image[] weaponSlots;
    [SerializeField, Tooltip("Weapon Images")] private Image[] weapons;

    private Color inactiveGunColor = new Color(1f, 1f, 1f, .5f);
    private Color activeGunColor = Color.white;

    public void UpdateStaminaUI(float value)
    {
        staminaBar.fillAmount = value;
    }

    public void UpdateHealthUI(float value)
    {
        healthBar.fillAmount = value;
    }

    public void UpdateScoreUI(int value)
    {
        currency.text = value.ToString();
    }

    public void UpdateAmmoUI(int weaponAmmo, int reserveAmmo)
    {
        ammo.text = string.Format("{0} / {1}", weaponAmmo, reserveAmmo);
    }

    // update weapon rect positions to be in the middle of the weapon slot
    public void UpdateWeaponUIPositions()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            // Calculate the length of the difference between the top left corner of the weapon and the slot
            Vector2 slotTopLeft = weaponSlots[i].rectTransform.rect.position + Vector2.up * weaponSlots[i].rectTransform.rect.height;
            Vector2 weaponTopLeft = weapons[i].rectTransform.rect.position + Vector2.up * weapons[i].rectTransform.rect.height;
            Vector2 deltaDiagonal = slotTopLeft - weaponTopLeft;

            // Add half of the diagonal difference to the weapon position to center it in the slot
            weapons[i].rectTransform.anchoredPosition += deltaDiagonal / 2f;
        }
    }

    // Update slot with given index with given sprite. If the sprite is null, the image will become transparent
    public void UpdateWeaponUI(int index, Sprite sprite)
    {
        weapons[index].sprite = sprite;
        weapons[index].color = sprite == null ? Color.clear : inactiveGunColor;
    }

    // Set color of weapon with given index to white and the other one to 50% transparency
    public void UpdateSelectedWeaponUI(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == index) weapons[i].color = activeGunColor;
            else if (weapons[i].sprite) weapons[i].color = inactiveGunColor;
        }
    }
}
