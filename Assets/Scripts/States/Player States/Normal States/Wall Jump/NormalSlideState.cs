using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Normal State/Slide")]
public class NormalSlideState : BaseState<PlayerController>
{
    private Rigidbody2D rb2d;

    private float timer;

    private bool isSliding;

    public override void EnterState(PlayerController parent)
    {
        base.EnterState(parent);

        timer = Runner.GetPlayerData().slideDuration;
        isSliding = true;

        rb2d = parent.GetRigidbody2D();

        Runner.GetAnimator().SetTrigger(PlayerAnimation.triggerSlide);
        Runner.GetAnimator().SetBool(PlayerAnimation.isSlidingBool, true);
    }

    public override void CheckStateTransition()
    {
        if (!isSliding){
            CurrentSuperState.SetSubState(CurrentSuperState.GetState(typeof(NormalIdleState)));
        }
    }

    public override IEnumerator ExitState()
    {
        Runner.GetAnimator().SetBool(PlayerAnimation.isSlidingBool, false);
        return base.ExitState();
    }

    public override void UpdateState()
    {
        // Set Timer to a certain duration
        if (isSliding && (timer >= 0)) {
            timer -= Time.deltaTime;

            if (timer <= 0){
                isSliding = false;
            }
        }

        // TODO: CONSIDER IF WANT TO ADD INVULNERABILITY


        // Set Direction to forward Facing
        rb2d.velocity = new Vector2(Runner.GetPlayerData().slideVelocity * Mathf.Sign(Runner.transform.localScale.x), rb2d.velocity.y);
    }

}
