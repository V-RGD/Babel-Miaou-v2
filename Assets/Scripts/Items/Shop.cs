using Interactions;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items
{
    public class Shop : MonoBehaviour
    {
        public int basePotionPrice;
        public int[] potionIncreasedPrices;
        public int maxPotionPrice;
        public int itemPrice;
        private int _lastConsumedPotion;

        public Potion healthPotion;
        public MaxPotion maxPotion;
        public Interactions.Item item;
        
        public Transform[] itemSpawnPoints;
        public ItemInfo[] itemProfiles;

        public void OnGeneration()
        {
            //when generated, setups 3 item slots for future use
            //always generates a health potion and a max potion, then generates a random item in the third slot
            Potion newPotion = Instantiate(healthPotion, itemSpawnPoints[0].position, quaternion.identity);
            MaxPotion newMaxPotion = Instantiate(maxPotion, itemSpawnPoints[1].position, quaternion.identity);
            Interactions.Item newItem = Instantiate(item, itemSpawnPoints[2].position, quaternion.identity);
            
            //assigns prices
            newPotion.price = basePotionPrice;
            newMaxPotion.price = maxPotionPrice;
            newItem.price = itemPrice;
            
            //the object is randomized
            newItem.id = Random.Range(0, itemProfiles.Length);
            Debug.Log("The shop has actually no way of knowing what items are held by the player, so it can and will drop useless items sometimes");
            Debug.Log("The shop has actually no way of knowing what items are held by the player, so it can and will drop useless items sometimes");
        }

        //when a potion is consumed, creates another one but with an increased price
        void AddNewPotion()
        {
            _lastConsumedPotion++;
        }
    }
}
