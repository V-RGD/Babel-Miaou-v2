using System.Collections;
using UnityEngine;
using Utilities.Code;

namespace Enemies
{
    public class Wanderer : Enemy
    {
        [SerializeField] private Transform attackAnchor;
        [SerializeField] private float attackForce;
        public override IEnumerator AttackBehaviour()
        {
            //prepares
            PlayAnimation(Attack);

            yield return new WaitForSeconds(attackPreparation);
            //orientates itself towards the player
            attackAnchor.transform.LookAt(Maths.IgnoreY(playerTransform.position, transform.position));
            //attacks and rushes towards the player
            attackAnchor.gameObject.SetActive(true);
            rb.AddForce(attackForce * playerDir);
            
            yield return new WaitForSeconds(attackLength);
            
            //then cools down until regaining control
            attackAnchor.gameObject.SetActive(false);
            
            yield return new WaitForSeconds(attackCooldown);
            //cooldown until moves or attacks
            isAttacking = false;
        }
    }
}  
