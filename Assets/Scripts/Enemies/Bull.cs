using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class Bull : Enemy
    {
        [Header("Dashing")]
        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashMaxSpeed;
        [SerializeField] private GameObject visuals;
        [SerializeField] private GameObject blobFx;
        
        public static readonly int Knock = Animator.StringToHash("Knock");
        private bool _isDashing;
        private bool _wallCollision;
        private Vector3 _dashDir;
        
        public IEnumerator Dash()
        {
            //prepares
            Vector3 dir = playerDir;
            _dashDir = -dir;

            yield return new WaitForSeconds(attackPreparation);

            //disappears
            visuals.SetActive(false);
            blobFx.SetActive(true);
            
            //rushes
            _isDashing = true;
            _dashDir = dir;
            rb.velocity = Vector3.zero;

            yield return new WaitUntil(()=> _wallCollision);
            //hits wall
            
            //reappears
            visuals.SetActive(true);
            blobFx.SetActive(false);

            //knocks itself
            _isDashing = false;
            rb.velocity = Vector3.zero;
            PlayAnimation(Knock);

            yield return new WaitForSeconds(attackCooldown);
            //after cooldown, regains control
            isAttacking = false;
        }

        public override void FixedUpdate()
        {
            Movement();
            DashMovement();
        }
        
        void DashMovement()
        {
            //don't apply any force if isn't dashing
            if (!_isDashing) return;
            
            //clamps speed to match maximum allowed
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, dashMaxSpeed);

            //moves body in the desired direction
            rb.AddForce(_dashDir * dashSpeed);
        }
    }
}
