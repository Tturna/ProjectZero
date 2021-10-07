using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponScriptableObject", order = 1)]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    public float damage;
    public int ammoCapacity;
    public int defaultReserveAmmo;
    public float projectileSpeed;
    public Vector2 muzzlePoint;
    public Sprite sprite;
    public GameObject projectilePrefab;
    public AudioClip attackSound;
}
