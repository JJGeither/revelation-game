using System.Collections;
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
    private bool isBackswing = false;

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
                currentState = PlayerState.Attacking;
                anim.SetTrigger("isAttacking");
                float attackAnimationLength = GetAnimationLength("Sword|Attack");
                StartCoroutine(AttackSequence(attackAnimationLength));
            }
            else if (currentState == PlayerState.Attacking && !isBackswing)
            {
                float remainingTime = GetRemainingAttackTime();
                if (remainingTime <= backswingTime)
                {
                    anim.SetTrigger("isBackswing");
                    StartCoroutine(InitiateBackswing(remainingTime));
                }
            }
        }
    }

    IEnumerator InitiateBackswing(float remainingTime)
    {
        isBackswing = true;
        yield return new WaitForSeconds(backswingTime);

        float backswingAnimationLength = GetAnimationLength("Sword|Backswing");
        yield return new WaitForSeconds(backswingAnimationLength  + cooldown);
        currentState = PlayerState.Idle;
        isBackswing = false;
    }

    IEnumerator AttackSequence(float attackAnimationLength)
    {
        yield return new WaitForSeconds(attackAnimationLength);
        if (!isBackswing)
        {
            currentState = PlayerState.Idle;
        }
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
        AnimatorStateInfo currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float currentTime = currentStateInfo.normalizedTime * currentStateInfo.length;
        float remainingTime = GetAnimationLength("Sword|Attack") - currentTime;
        return remainingTime;
    }
}
