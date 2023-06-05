using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Generation
{
    public class ShopRoom : Room
    {
        //this room spawns a shop seller
        [SerializeField] private Shop shop;

        public override void OnGeneration()
        {
            Instantiate(shop, transform.position, Quaternion.identity);
            shop.OnGeneration();
        }
    }
}

