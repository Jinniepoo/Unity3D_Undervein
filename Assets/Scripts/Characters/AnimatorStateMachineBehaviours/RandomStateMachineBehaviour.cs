using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diablo.Characters
{
    public class RandomStateMachineBehaviour : StateMachineBehaviour
    {
        public int numberOfStates = 2;
        public float minNormTime = 0f;
        public float maxNormTime = 5f;

        protected float randomNormalTime;

        readonly int hashRandomIdle = Animator.StringToHash("RandomIdle");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            randomNormalTime = Random.Range(minNormTime, maxNormTime);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 이 상태에서 전환될 경우, 랜덤 Idle 파라미터를 -1로 초기화
            if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
            {
                animator.SetInteger(hashRandomIdle, -1);
            }

            // 상태가 무작위로 결정된 Normalised Time)을 넘겼고 아직 전환 중이 아니라면 랜덤 Idle로 설정
            if (stateInfo.normalizedTime > randomNormalTime && !animator.IsInTransition(0))
            {
                animator.SetInteger(hashRandomIdle, Random.Range(0, numberOfStates));
            }
        }
    }

}
