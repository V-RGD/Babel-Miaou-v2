using System;
using UnityEngine;

namespace Interactions
{
    public class Consumable : MonoBehaviour
    {
        public bool isLockedByShop;
        public int price;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (isLockedByShop)
                {
                    return;
                }
                Collect();
            }
        }

        public virtual void Collect()
        {
            
        }
    }
}
