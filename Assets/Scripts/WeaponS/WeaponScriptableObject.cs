using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponScriptableObject", order = 1)]
public class WeaponScriptableObject : ScriptableObject
{
    public string weaponName;
    public float damage;
    public int ammoCapacity;
    public int defaultReserveAmmo;
    public float projectileSpeed;
    public float cameraShakeTime;
    public float cameraShakeMultiplier;
    public float muzzleFlashLifetime;
    public Vector2 muzzlePoint;
    public Sprite sprite;
    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;
    public Color projectileColor;
    public Color muzzleFlashColor;
    public bool isTwoHanded;
    public Vector2[] handPositions = new Vector2[2];
    public AnimatorOverrideController animationController;
    public AudioClip attackSound;
}
