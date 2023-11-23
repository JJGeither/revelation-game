using System.Collections;
using UnityEngine;

public class SwordAnimation : MonoBehaviour
{
    public Animator anim;
    public float backswingTime;
    public float cooldown;
    public string current_animation;
    bool isBackswing;
    AnimatorClipInfo[] animatorinfo;
    int targetTransitionNameHash = Animator.StringToHash("EndAttackTransition");

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
        isBackswing = false;
    }

    void Update()
    {
        animatorinfo = this.anim.GetCurrentAnimatorClipInfo(0);
        current_animation = animatorinfo[0].clip.name;
        Debug.Log(current_animation);

        if (Input.GetMouseButtonDown(1))
        {
            if (!anim.IsInTransition(0) && current_animation != "Sword|Attack.001")
            {
                anim.SetTrigger("attack");
                //Debug.Log("ATTACK");
                //float attackAnimationLength = GetAnimationLength("Sword|Attack.001");
                //StartCoroutine(AttackSequence(attackAnimationLength));
            }
            else if ( current_animation == "Sword|Attack.001" && !anim.IsInTransition(0))
            {

                anim.SetTrigger("backswing");
                //Debug.Log("Back");
                //StartCoroutine(InitiateBackswing());
            } else if (current_animation == "Sword|BackSwing")
            {
                anim.SetTrigger("attack");
            }
        }
    }

    IEnumerator AttackSequence(float attackAnimationLength)
    {
        anim.SetTrigger("attack");
        // anim.SetBool("isAttacking", true);
        currentState = PlayerState.Attacking;

        float attackLength = GetAnimationLength("Sword|Attack.001");
        //anim.SetBool("isAttacking", false);

        while (anim.GetBool("isBackswing"))
        {
            yield return null;
        }
        Debug.Log("DONE");

        //EndAttack();
    }

    void EndAttack()
    {

        anim.SetBool("isBackswing", false);
        anim.SetBool("isAttacking", false);
        anim.SetTrigger("finishAttack");

        currentState = PlayerState.Idle;

    }

    IEnumerator InitiateBackswing()
    {
        anim.SetBool("isBackswing", true);
        //float remainingTime = GetRemainingAttackTime();
        //Debug.Log(remainingTime);
        //yield return new WaitForSeconds(remainingTime);
        while (anim.GetBool("isAttacking"))
        {
            yield return null;
        }


        float backswingAnimationLength = GetAnimationLength("Sword|BackSwing.001");
        yield return new WaitForSeconds(backswingAnimationLength);

        anim.SetBool("isBackswing", false);
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
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime;
        float animationLength = stateInfo.length;
        float currentTime = normalizedTime * animationLength;
        float remainingTime = GetAnimationLength("Sword|Attack.001") - currentTime;


        return Mathf.Max(0f, remainingTime); // Ensure remaining time is non-negative
    }
}
