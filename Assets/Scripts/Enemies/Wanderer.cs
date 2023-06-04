using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class Wanderer : Enemy
    {
        [SerializeField] private Collider attackHitbox;
        [SerializeField] private float attackForce;
        public override IEnumerator AttackBehaviour()
        {
            //prepares
            PlayAnimation(Attack);

            yield return new WaitForSeconds(attackPreparation);
            //attacks and rushes to the player
            attackHitbox.enabled = true;
            rb.AddForce(attackForce * playerDir);
            
            yield return new WaitForSeconds(attackLength);
            
            //then cools down until regaining control
            attackHitbox.enabled = false;
            
            yield return new WaitForSeconds(attackCooldown);
            //cooldown until moves or attacks
            isAttacking = false;
        }
    }
}  
