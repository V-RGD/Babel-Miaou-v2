using Player;
using UnityEngine;

namespace Interactions
{
    public class Money : Consumable
    {
        [SerializeField] private int amountEarned;

        public override void Collect()
        {
            Inventory.instance.money += amountEarned;
            Destroy(gameObject);
        }
    }
}
