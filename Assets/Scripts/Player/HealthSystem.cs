using UI;
using UnityEngine;

namespace Player
{
    public class HealthSystem : MonoBehaviour
    {
        public static HealthSystem instance;
        public int health;
        public int maxHealth;
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void TakeDamage(int amount)
        {
            //if this is a deadly hit, health capped at zero and player dies
            if (health - amount <= 0)
            {
                health = 0;
                Death();
            }
            else
            {
                //in any other case, decreases health by desired amount
                health -= amount;
            }
            //updates hud
            HUD.instance.UpdateHealth();
        }
        

        void Death()
        {
            //player death
        }
    }
}
