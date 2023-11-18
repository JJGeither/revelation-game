using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SwordAnimation : MonoBehaviour
{
    public Animator anim;
    public float backswingTime;
    public float cooldown;

    private enum PlayerState
    {
        Idle,
        Attacking,
        Cooldown
    }

    private PlayerState currentState = PlayerState.Idle;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            if (currentState == PlayerState.Idle)
            {
                    float attackAnimationLength = GetAnimationLength("Sword|Attack.001");
                    StartCoroutine(AttackSequence(attackAnimationLength));


            }
            else if (currentState == PlayerState.Attacking && !anim.GetBool("isBackswing"))
            {
                float remainingTime = GetRemainingAttackTime();
                if (remainingTime <= backswingTime)
                {
                    StartCoroutine(InitiateBackswing(remainingTime));
                }
            }
        }

    }

    IEnumerator InitiateBackswing(float remainingTime)
    {
        while (anim.IsInTransition(0))
        {
            yield return null;
        }
        yield return new WaitForSeconds(remainingTime);
        anim.SetBool("isBackswing", true);
        float backswingAnimationLength = GetAnimationLength("Sword|BackSwing.001");
        yield return new WaitForSeconds(backswingAnimationLength);
        anim.SetBool("isBackswing", false);
    }

    IEnumerator AttackSequence(float attackAnimationLength)
    {
        currentState = PlayerState.Attacking;
        anim.SetTrigger("isAttacking");
        yield return new WaitForSeconds(attackAnimationLength);

        while (anim.GetBool("isBackswing"))
        {
            yield return null;
        }

        endAttack();
    }

    void endAttack()
    {
        anim.SetTrigger("finishAttack");
        currentState = PlayerState.Idle;
    }

    float GetAnimationLength(string clipName)
    {
        float length = 0f;
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
            {
                length = clip.length;
                break;
            }
        }
        return length;
    }


    float GetRemainingAttackTime()
    {
        // Get the current state information of the Animator
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);


        // Check the normalized time (0 to 1) to see how long the current animation has been running
        float normalizedTime = stateInfo.normalizedTime;

        // Get the duration of the current animation clip
        float animationLength = stateInfo.length;

        // Calculate the actual time the animation has been running
        float currentTime = normalizedTime * animationLength;

        // Log or use currentTime as needed
        Debug.Log("Current Time: " + currentTime);

        float remainingTime = GetAnimationLength("Sword|Attack.001") - currentTime;
        return remainingTime;
    }

}