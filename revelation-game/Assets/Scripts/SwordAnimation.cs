using System.Collections;
using UnityEngine;

public class SwordAnimation : MonoBehaviour
{
    public Collider collider;
    private bool hasHit;
    private Animator anim;
    private string current_animation;
    AnimatorClipInfo[] animatorinfo;

    void Start()
    {
        collider.enabled = false; 
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        animatorinfo = this.anim.GetCurrentAnimatorClipInfo(0);
        current_animation = animatorinfo[0].clip.name;

        if (Input.GetMouseButtonDown(1))
        {
            if (!anim.IsInTransition(0) && !isForwardSwing())
            {
                anim.SetTrigger("attack");
            }
            else if (isForwardSwing() && !anim.IsInTransition(0))
            {
                anim.SetTrigger("backswing");
            } else if (isBackswing())
            {
                anim.SetTrigger("attack");
            }
        }
    }

    bool isAttacking()
    {
        return !anim.IsInTransition(0) && current_animation == "Sword|Attack.001" || current_animation == "Sword|BackSwing";
    }

    bool isForwardSwing()
    {
        return current_animation == "Sword|Attack.001";
    }

    bool isBackswing()
    {
        return current_animation == "Sword|BackSwing";
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasHit = true;
    }

    void colliderEnable()
    {
        collider.enabled = true;
    }

    void colliderDisable()
    {
        collider.enabled = false;
    }

}
