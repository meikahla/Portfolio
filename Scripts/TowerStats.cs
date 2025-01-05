using UnityEngine;

/// <summary>
/// Manages the stats and upgrade system for towers in the game. Allows for upgrading various stats such as damage, health, fire rate, 
/// range, and more. Handles critical hit calculations, damage reduction based on armor, and resource/experience generation multipliers.
/// </summary>
public class TowerStats : MakeSingleton<TowerStats>
{
    // Tower stats
    public float damage, health, fireRate, range, projectileSpeed, armor;
    public float resourceGenerate = 1.0f, expGenerate = 1.0f;
    public float critChance = 0.01f, critDamageMultiplier = 1.5f;

    // Upgrade stats with a single method
    public void UpgradeStat(ref float stat, float amount) => stat += amount;

    // Apply damage to the tower
    public void TakeDamage(float damageAmount) => health = Mathf.Max(0, health - ReduceDamage(damageAmount));

    // Perform attack (normal or critical hit)
    public void Attack(NewEnemy enemy)
    {
        float damageToDeal = Random.value <= critChance ? Mathf.RoundToInt(damage * critDamageMultiplier) : damage;
        enemy.TakeDamage(damageToDeal);
    }

    // Reduce incoming damage based on armor
    private float ReduceDamage(float incomingDamage) => Mathf.Max(0, incomingDamage - armor);
}
