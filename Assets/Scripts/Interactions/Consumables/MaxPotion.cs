using Player;
using UnityEngine;

namespace Interactions
{
    public class MaxPotion : Consumable
    {
        [SerializeField] private int amountAdded;
        public override void Collect()
        {
            HealthSystem.instance.maxHealth += amountAdded;
            Destroy(gameObject);
        }
    }
}
