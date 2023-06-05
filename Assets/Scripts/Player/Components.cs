using UnityEngine;

namespace Player
{
    public class Components : MonoBehaviour
    {
        public static Components instance;

        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public Animator animator;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }
    }
}
