using UnityEngine;

namespace Player
{
    public class Controller : MonoBehaviour
    {
        public static Controller instance;

        [SerializeField] private float maxSpeed;
        [SerializeField] private float walkSpeed;
        public Vector2 inputDir;
        public Vector3 direction;

        private Rigidbody _rb;

        [SerializeField] private float dashRange;
        [SerializeField] private float dashLength;
        [SerializeField] private float dashForce;
        private float _dashTimer;

        public bool canMove;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_dashTimer > 0) _dashTimer -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            Movement();
        }

        public void InputDash()
        {
            if (_dashTimer > 0) return;
            Dash();
        }

        void Movement()
        {
            //clamps speed to match maximum allowed
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxSpeed);

            //if the player isn't allowed to move, do not calculate direction nor do apply velocity
            if (!canMove) return;

            //converts input to world dir
            direction = new Vector3(direction.x, 0, direction.y);

            //if no input, don't move
            if (inputDir == Vector2.zero) return;
            
            //moves body in the desired direction
            _rb.AddForce(direction * walkSpeed);
        }

        void Dash()
        {
            _dashTimer = dashLength;
            _rb.AddForce(direction * dashForce);
        }
    }
}