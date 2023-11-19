using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : HealthSystem
{
    [SerializeField] float timeUntilDeath = 0.1f;

    public override void LooseHealth(int amount)
    {
        //checks if the hit comes from behind
        base.LooseHealth(amount);
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(DestroyAfterSeconds());
    }

    IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(timeUntilDeath);
        Destroy(gameObject);
    }
}