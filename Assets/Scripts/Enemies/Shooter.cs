using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Enemies
{
    public class Shooter : Enemy
    {
        [Header("--References--")]
        [SerializeField] private float projectileForce;
        [SerializeField] private Rigidbody bulletPrefab;
        private List<Rigidbody> _bullets;
        private int _bulletToUse;

        private void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                _bullets.Add(Instantiate(bulletPrefab, Vector3.back * 10000, quaternion.identity));
                _bullets[i].gameObject.SetActive(false);
            }
        }

        public override IEnumerator AttackBehaviour()
        {
            //prepares
            PlayAnimation(Attack);

            yield return new WaitForSeconds(attackPreparation);
            
            //shoots a bullet
            _bulletToUse = Utilities.Code.Maths.NextOfList(_bulletToUse, _bullets.Count);
            _bullets[_bulletToUse].gameObject.SetActive(true);
            _bullets[_bulletToUse].AddForce(playerDir * projectileForce);
            StartCoroutine(DisableBullet(_bulletToUse));

            yield return new WaitForSeconds(attackCooldown);
            //cooldown until moves or attacks
            isAttacking = false;

        }

        IEnumerator DisableBullet(int bullet)
        {
            //disables bullet after a while to avoid clutter and performance issues
            yield return new WaitForSeconds(10);
            _bullets[bullet].gameObject.SetActive(false);
        }
    }
}
