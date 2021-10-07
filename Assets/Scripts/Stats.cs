// Common script used by all agents/entities
// Provides easily configurable statistics

using UnityEngine;

public struct Stats
{
    [Header("Statistics")]

    // These are serialized so that we can set default values in the editor
    // Also helps with monitoring
    public float health;
    public float maxHealth;
    public float movementSpeed;
    public float energy; // Stamina for player, but could be something else for enemies?
    public float maxEnergy;
    public float damage; // Not sure if this should just be an enemy specific statistic?
    public float attackDelay;
    //[SerializeField] private protected float knockback;
    //[SerializeField] private protected float armor;

    public Stats(float health, float maxHealth, float movementSpeed, float energy, float maxEnergy, float damage, float attackDelay)
    {
        this.health = health;
        this.maxHealth = maxHealth;
        this.movementSpeed = movementSpeed;
        this.energy = energy;
        this.maxEnergy = maxEnergy;
        this.damage = damage;
        this.attackDelay = attackDelay;
    }
}
