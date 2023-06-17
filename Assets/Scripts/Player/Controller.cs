using UnityEngine;

namespace Player
{
    public class Controller : MonoBehaviour
    {
        public static Controller instance;

        [SerializeField] float maxSpeed;
        [SerializeField] float walkSpeed;
        public Vector2 inputDir;
        public Vector3 direction;

        Rigidbody _rb;

        [SerializeField] float dashRange;
        [SerializeField] float dashLength;
        [SerializeField] float dashForce;
        float _dashTimer;

        public bool canMove;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            _rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (_dashTimer > 0) _dashTimer -= Time.deltaTime;
        }

        void FixedUpdate()
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
            Vector3 clampedVelocity = Vector3.ClampMagnitude(_rb.velocity, maxSpeed);
            _rb.velocity = new Vector3(clampedVelocity.x, _rb.velocity.y, clampedVelocity.z);

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