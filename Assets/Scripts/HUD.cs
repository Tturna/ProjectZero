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

    // Set color of weapon with given index to white and the other one to transparent
    // Also update weapon rect positions to be in the middle of the weapon slot
    public void UpdateSelectedWeaponUI(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == index) weapons[i].color = activeGunColor;
            else weapons[i].color = inactiveGunColor;

            weapons[i].rectTransform.anchoredPosition = weaponSlots[i].rectTransform.anchoredPosition;
        }
    }
}
