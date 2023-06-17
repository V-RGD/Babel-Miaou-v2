using Generation.Components;
using Items;
using UnityEngine;

namespace Generation
{
    public class ShopRoom : Room
    {
        //this room spawns a shop seller
        [SerializeField] Shop shop;

        public override void OnGeneration()
        {
            Shop roomShop = Instantiate(shop, ComponentsGeneration.instance.shopParent);
            roomShop.transform.position = roomCenter;
            shop.OnGeneration();
        }
    }
}

