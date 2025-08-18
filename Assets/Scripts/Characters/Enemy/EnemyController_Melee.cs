using Diablo.AI;
using Diablo.Core;
using Diablo.UIs;
using System.Collections.Generic;
using UnityEngine;

namespace Diablo.Characters
{
    public class EnemyController_Melee : EnemyController, IAttackable, IDamageable
    {
        #region Variables
        [SerializeField]
        public Transform hitPoint;

        public Transform[] waypoints;

        [SerializeField]
        private NPCBattleUI healthBar;

        public float maxHealth => 100f;
        private float health;

        private int hitTriggerHash = Animator.StringToHash("Hit");
        private int isAliveHash = Animator.StringToHash("Alive");

        #endregion Variables

        #region Proeprties
        public override bool IsAvailableAttack
        {
            get
            {
                if (!Target)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, Target.position);
                return (distance <= AttackRange);
            }
        }

        #endregion Properties

        #region Unity Methods

        protected override void Start()
        {
            base.Start();

            stateMachine.AddState(new MoveState());
            stateMachine.AddState(new AttackState());
            stateMachine.AddState(new DeadState());

            health = maxHealth;

            if (healthBar)
            {
                healthBar.MinimumValue = 0.0f;
                healthBar.MaximumValue = maxHealth;
                healthBar.Value = health;
            }
        }

        private void OnAnimatorMove()
        {
            // Track CharacterController
            Vector3 position = transform.position;
            position.y = agent.nextPosition.y;

            animator.rootPosition = position;
            agent.nextPosition = position;
        }

        #endregion Unity Methods

        #region Helper Methods
        #endregion Helper Methods

        #region IDamagable interfaces

        public bool IsAlive => (health > 0);

        public void TakeDamage(int damage, GameObject hitEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }

            health -= damage;

            if (healthBar)
            {
                healthBar.Value = health;
            }

            if (hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, hitPoint);
            }

            if (IsAlive)
            {
                animator?.SetTrigger(hitTriggerHash);
            }
            else
            {
                if (healthBar != null)
                {
                    healthBar.enabled = false;
                }

                stateMachine.ChangeState<DeadState>();
            }
        }

        #endregion IDamagable interfaces

        #region IAttackable Interfaces

        public GameObject hitEffectPrefab = null;

        [SerializeField]
        private List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();

        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex)
        {

        }

        #endregion IAttackable Interfaces
    }
}
