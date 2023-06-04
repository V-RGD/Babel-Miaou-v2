using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        //basic properties that every enemy shares
        [Header("HealthSystem")]
        public int maxHealth;
        [SerializeField] private int health;
        
        [Header("Attack Values")]
        [HideInInspector] public int attackValue;
        [SerializeField] private float attackRange;
        public float attackPreparation;
        public float attackLength;
        public float attackCooldown;
        public bool isAttacking;
        private Transform _playerTransform;
        [HideInInspector] public Vector3 playerDir;
        
        [Header("Movement Values")] 
        [SerializeField] private float movementSpeed;
        private bool _canMove;
        private float _playerDist;
        protected bool _isActive;

        [Header("References")]
        [HideInInspector] public Animator animator;
        [HideInInspector] public Rigidbody rb;
        private Collider _collider;
        private NavMeshAgent _navMeshAgent;
        private ParticleSystem _spawnFx;
        private SpriteRenderer _spriteRenderer;
        
        [Header("Animation")]
        [HideInInspector] public int currentAnimatorState;
        public static readonly int Walk = Animator.StringToHash("Walk");
        public static readonly int Attack = Animator.StringToHash("Attack");

        public virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public virtual void Update()
        {
            if (!_isActive)
            {
                return;
            }
            
            Behaviour();
            Targeting();
        }
        
        public virtual void FixedUpdate()
        {
            Movement();
        }

        #region HealthSystem

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlayerAttack"))
            {
                health -= other.GetComponent<PlayerAttackHitbox>().damage;
            }
        }

        #endregion

        #region Movement

        public void Movement()
        {
            //movement is applied when the enemy isn't attacking
            if (isAttacking) return;
            
            //animates walking
            if (currentAnimatorState != Walk) PlayAnimation(Walk);

            //if it is too far from the player, gets closer
            if (_playerDist >= attackRange + 1)
            {
                _navMeshAgent.SetDestination(_playerTransform.position);
            }
            else
            {
                _navMeshAgent.SetDestination(transform.position);
            }
        }

        public void Targeting()
        {
            //manages direction and target
        }

        #endregion

        #region IA/Attacks

        public void Behaviour()
        {
            playerDir = _playerTransform.position - transform.position;
            _playerDist = playerDir.magnitude;
            //if an attack isn't pending and the enemy is close enough, starts attacking
            if (!isAttacking && _playerDist <= attackRange + 1)
            {
                isAttacking = true;
                StartCoroutine(AttackBehaviour());
            }
        }
        
        //this is called whenever the player enters the room where the enemy is located
        public IEnumerator Spawn()
        {
            //vfx is called, then appears and activates
            _spawnFx.Play();
            yield return new WaitForSeconds(2);
            _isActive = true;
            _collider.enabled = true;
            _spriteRenderer.enabled = true;
            _navMeshAgent.speed = movementSpeed;
        }

        public virtual IEnumerator AttackBehaviour()
        {
            yield return null;
        }
        #endregion

        #region Animator

        public void PlayAnimation(int animation)
        {
            if (currentAnimatorState != animation)
            {
                currentAnimatorState = animation;
                animator.CrossFade(animation, 0, 0);
            }
        }

        #endregion
    }
}

