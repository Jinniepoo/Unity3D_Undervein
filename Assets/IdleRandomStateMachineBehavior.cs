using UnityEngine;

public class IdleRandomStateMachineBehavior : StateMachineBehaviour
{
    public int iNumOfStates = 2; 
    public float fMinNormTime = 2f; /* 최소 실행 시간 */
    public float fMaxNormTime = 5f; /* 최대 실행 시간 */

    public float fRandNormTime; /* 계산용 */

    readonly int iHashRandIdle = Animator.StringToHash("RandomIdle");
    /* string hash 값 :  string값 그대로 사용하게 되면 string 비교 연산.
      string비교연산보다 int 비교연산이 훨씬 작기 때문에 되도록이면 string hash사용 권장 */

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Randomly decide a time at which to apply transition
        fRandNormTime = Random.Range(fMinNormTime, fMaxNormTime);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If transitioningaway fromt this tate reset the random idle to paramter to -1
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
        {
            animator.SetInteger(iHashRandIdle, -1);
        }

        // If the state is beyong t he radomly decide normalised time and not transitioned yet
        if (stateInfo.normalizedTime > fRandNormTime && !animator.IsInTransition(0))
        {
            animator.SetInteger(iHashRandIdle, Random.Range(0, iNumOfStates));
        }
    }
}
