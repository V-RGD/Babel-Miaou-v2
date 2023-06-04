using Player;
using UnityEngine;

namespace Interactions
{
    public class MaxPotion : Consumable
    {
        [SerializeField] private int amountGranted;
        public override void Collect()
        {
            HealthSystem.instance.maxHealth += amountGranted;
            Destroy(gameObject);
        }
    }
}
