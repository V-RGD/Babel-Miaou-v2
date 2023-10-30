using Sirenix.OdinInspector;
using UnityEngine;

namespace Player
{
    public class Controller : MonoBehaviour
    {
        public static Controller instance;

        [Title("Values", "", TitleAlignments.Centered)]
        [SerializeField] float walkSpeed;
        [SerializeField] float dashLength;
        [SerializeField] float dashForce;
        [SerializeField] float dashCooldown = 1;
        [SerializeField] float maxSpeed;

        [Title("Debug", "", TitleAlignments.Centered)]
        public Vector2 inputDir;
        public Vector2 lastInputDir;
        public Vector3 direction;
        [ShowInInspector] float _dashTimer;
        [ShowInInspector] float _dashCooldownTimer;
        public bool canMove;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void Update()
        {
            if (_dashTimer > 0) _dashTimer -= Time.deltaTime;
            if (_dashCooldownTimer > 0) _dashCooldownTimer -= Time.deltaTime;
        }

        void FixedUpdate()
        {
            if (_dashTimer > 0) return;
            Movement();
        }

        public void InputDash()
        {
            if (_dashCooldownTimer > 0) return;
            Dash();
        }

        void Movement()
        {
            //clamps speed to match maximum allowed
            Vector3 velocity = Components.instance.rb.velocity;
            // Vector3 clampedVelocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            // Components.instance.rb.velocity = new Vector3(clampedVelocity.x, velocity.y, clampedVelocity.z);
            
            //if the player isn't allowed to move, do not calculate direction nor do apply velocity
            if (!canMove) return;

            //converts input to world dir
            direction = Quaternion.AngleAxis(45, Vector3.up) * new Vector3(inputDir.x, 0, inputDir.y);

            //if no input, don't move
            if (inputDir == Vector2.zero)
            {
                Components.instance.rb.velocity = new Vector3(0, velocity.y, 0);
                return;
            }

            //moves body in the desired direction
            Components.instance.rb.velocity = new Vector3(direction.x, velocity.y, direction.z) * walkSpeed;
        }

        void Dash()
        {
            _dashTimer = dashLength;
            _dashCooldownTimer = dashCooldown;
            Components.instance.rb.AddForce(direction * dashForce);
        }
    }
}