using Diablo.Characters;
using System;
using UnityEngine;

namespace Diablo.AI
{
    [Serializable]
    public class IdleState : State<EnemyController>
    {
        bool isPatrol = false;
        private float minIdleTime = 0.0f;
        private float maxIdleTime = 3.0f;
        private float idleTime = 0.0f;

        private Animator animator;
        private CharacterController controller;

        protected int isMoveHash = Animator.StringToHash("Move");
        protected int moveSpeedHash = Animator.StringToHash("MoveSpeed");
        protected int isAlive = Animator.StringToHash("Alive");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(isMoveHash, false);
            animator.SetFloat(moveSpeedHash, 0);
            animator.SetBool(isAlive, true);
            controller?.Move(Vector3.zero);

            if (context is EnemyController_Range)
            {
                isPatrol = true;
                idleTime = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
            }
        }

        public override void Update(float deltaTime)
        {
            // Target 발견시 [Move State]로 전환
            if (context.Target)
            {
                if (context.IsAvailableAttack)
                {
                    // Attack CoolTime확인 후 A[ttackState]로 전환
                    stateMachine.ChangeState<AttackState>();
                }
                else
                {
                    stateMachine.ChangeState<MoveState>();
                }
            }
            else if (isPatrol && stateMachine.ElapsedTimeInState > idleTime)
            {
                stateMachine.ChangeState<MoveToWaypointState>();
            }
        }

        public override void OnExit()
        {
        }
    }
}