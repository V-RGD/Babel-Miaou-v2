using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class Marksman : Enemy
    {
        [Header("Laser")]
        public static readonly int Knock = Animator.StringToHash("Knock");
        private bool _isShootingLaser;
        private Vector3 _laserDir;
        private Vector3 _laserTarget;
        private Vector3 _laserReplaceSpeed;
        private LineRenderer _lineRenderer;

        public override void Awake()
        {
            animator = GetComponent<Animator>();
            _lineRenderer = GetComponent<LineRenderer>();
        }
        
        public IEnumerator Shoot()
        {
            //prepares
            Vector3 dir = playerDir;

            yield return new WaitForSeconds(attackPreparation);

            //shoots laser
            _isShootingLaser = true;

            yield return new WaitForSeconds(attackLength);

            //stops
            _isShootingLaser = false;

            yield return new WaitForSeconds(attackCooldown);
            //after cooldown, regains control
            isAttacking = false;
        }
        
        public override void Update()
        {
            if (!_isActive)
            {
                return;
            }
            
            Behaviour();
            Targeting();
            Laser();
        }

        public override void FixedUpdate()
        {
            Movement();
        }
        
        void Laser()
        {
            //don't manage laser if not shooting
            if (!_isShootingLaser) return;
            
            //checks if player is in laser range
            if (Physics.Raycast(transform.position, _laserDir, 1000, LayerMask.NameToLayer("Player")))
            {
                //deals damage
                Player.HealthSystem.instance.TakeDamage(attackValue);
            }
        }
    }
}
