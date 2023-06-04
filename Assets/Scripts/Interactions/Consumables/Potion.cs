using Player;
using UnityEngine;

namespace Interactions
{
    public class Potion : Consumable
    {
        [SerializeField] private int amountHealed;
        public override void Collect()
        {
            //if player has full health, doesn't consume
            if (HealthSystem.instance.health == HealthSystem.instance.maxHealth)
            {
                return;
            }
            
            //if amount healed superior than max allowed, adds only health needed
            if (HealthSystem.instance.health + amountHealed > HealthSystem.instance.maxHealth)
            {
                HealthSystem.instance.health = HealthSystem.instance.maxHealth;
                Destroy(gameObject);
                return;
            }
            
            //if health needed equals or is superior than granted, is consumed
            HealthSystem.instance.health += amountHealed;
            Destroy(gameObject);
        }
    }
}
