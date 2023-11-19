using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [field: SerializeField] public int maxHealth { get; private set; } = 1;
    [field: SerializeField] public int currentHealth { get; private set; }
    [SerializeField] float invincibilityTime = 0.1f;

    public Action onDeath;
    public Action onHit;

    float _invincibilityTimeLeft;

    public virtual void Start() => currentHealth = maxHealth;

    public virtual void LooseHealth(int amount)
    {
        if (currentHealth <= 0) return; //if already dead, dont do anything
        onHit?.Invoke();
        currentHealth -= amount;
        _invincibilityTimeLeft = invincibilityTime;
        if (currentHealth <= 0) Die();
    }

    public virtual void RestoreHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth) currentHealth = maxHealth;
    }

    public virtual void Die()
    {
        onDeath?.Invoke();
    }

    void Update()
    {
        _invincibilityTimeLeft.DecreaseTimerIfPositive();
    }
}