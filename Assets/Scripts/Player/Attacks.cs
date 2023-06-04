using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Attacks : MonoBehaviour
    {
        public static Attacks instance;
        [Serializable] public class Attack
        {
            public string name;
            [Header("Timings")]
            public float activeLength;
            public float cooldown;
            [Header("Values")] 
            public float force;
            [Header("References")]
            public Collider hitbox;
            public AnimationClip animation;
        }

        [SerializeField] private List<Attack> baseComboAttacks;
        
        [Header("AttackSystem")]
        private int _comboCounter;
        private float _comboTimer;
        private float _timeToCombo;
        private float _cooldownTimer;
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        void InputAttack(int index)
        {
            //blocks attacking if an attack is pending
            if (_cooldownTimer > 0) return;
            //if everything is good to go, starts attack asked
            StartCoroutine(TriggerAttack(baseComboAttacks[_comboCounter]));
        }

        IEnumerator TriggerAttack(Attack attack)
        {
            //----------------------------ON PLAYER INPUT------------------------------------------
            //stops player
            Controller.instance.canMove = false;
            
            //plays corresponding animation
            Components.instance.animator.CrossFade(attack.animation.name, 0, 0);

            //enables hitbox
            Collider hitbox = attack.hitbox;
            hitbox.enabled = true;
            
            //sets counter to reflect attack used
            _comboCounter = Utilities.Code.Maths.NextOfList(_comboCounter, baseComboAttacks.Count);
            //sets combo counter for the possibility to combo if the player inputs fast enough
            _comboTimer = _timeToCombo;
            
            Components.instance.rb.AddForce(Controller.instance.direction * attack.force, ForceMode.Impulse);
            
            //----------------------------WHEN HITBOX STOPS------------------------------------------
            yield return new WaitForSeconds(attack.activeLength);
            hitbox.enabled = false;
            
            //----------------------------WHEN ATTACK ENDS------------------------------------------
            yield return new WaitForSeconds(attack.cooldown);
            //player regains control
            Controller.instance.canMove = true;
        }

        private void Update()
        {
            Timers();
        }

        void Timers()
        {
            //attack cooldown
            if (_cooldownTimer > 0) _cooldownTimer -= Time.deltaTime;
            _comboTimer -= Time.deltaTime;
            
            //combo management
            if (_comboTimer > 0) return;
            //if combo timer depletes, next attack will be the first of the combo
            _comboTimer = 0;
            _comboCounter = 0;
        }
    }
}