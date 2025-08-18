using UnityEngine;

public class IdleRandomStateMachineBehavior : StateMachineBehaviour
{
    public int iNumOfStates = 2; 
    public float fMinNormTime = 2f; /* �ּ� ���� �ð� */
    public float fMaxNormTime = 5f; /* �ִ� ���� �ð� */

    public float fRandNormTime; /* ���� */

    readonly int iHashRandIdle = Animator.StringToHash("RandomIdle");
    /* string hash �� :  string�� �״�� ����ϰ� �Ǹ� string �� ����.
      string�񱳿��꺸�� int �񱳿����� �ξ� �۱� ������ �ǵ����̸� string hash��� ���� */

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
