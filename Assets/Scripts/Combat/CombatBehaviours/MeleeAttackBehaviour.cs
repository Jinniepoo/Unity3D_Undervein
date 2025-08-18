using Diablo.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diablo.Characters
{
    public class MeleeAttackBehaviour : AttackBehaviour
    {
        public ManualCollision attackCollision;

        public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
        {
            Collider[] colliders = attackCollision?.CheckOverlapBox(targetMask);

            foreach (Collider col in colliders)
            {
                col.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, effectPrefab);
            }
            
            calcCoolTime = 0.0f;
        }
    }
}