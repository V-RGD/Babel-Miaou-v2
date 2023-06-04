using UnityEngine;

namespace Player
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory instance;
        [SerializeField] private int maxItemAmount;
        public int[] itemsEquipped;
        public int money;

        void ReplaceItem(int newItem, int itemReplaced)
        {
            itemsEquipped[itemReplaced] = newItem;
        }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        //link to percs
    }
}