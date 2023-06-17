using Generation.Components;
using Generation.Level;
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
            Shop roomShop = Instantiate(shop, LevelBuilder.instance.shopParent);
            roomShop.transform.position = roomCenter;
            shop.OnGeneration();
        }
    }
}

