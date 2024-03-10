using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State/Grapple State")]
public class GrappleHookState : BaseState<PlayerController>
{
    private float horizontalControl, verticalControl, grappleControl;

    [SerializeField] private GrappleHook grappleHook;

    private Rigidbody2D rb2d;

    public override void EnterState(PlayerController parent, object objToPass)
    {
        base.EnterState(parent, objToPass);

        if ((Vector2) objToPass != null){
            ExitState();
        }
        rb2d = parent.GetRigidbody2D();
        // TODO: Call Check for Grapple

        bool attached = grappleHook.ShootGrapple(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public override void CaptureInput()
    {
        horizontalControl = Runner.GetHorizontalControls();
        verticalControl = Runner.GetVerticalControls();

        // TODO: Set up Controls for Grapple
        grappleControl = Runner.GetGrappleControls();

        if (grappleControl < 0){
            grappleHook.ReleaseGrapple();
        }
    }

    public override void CheckStateTransition()
    {
        // TODO: If not Grapple then change
        if (!grappleHook.GetGrappled()) {
            Runner.SetMainState(typeof(NormalMainState));
        }
    }

    public override void FixedUpdateState()
    {
    }

    public override void InitialiseSubState()
    {
    }

    public override void OnStateCollisionEnter(Collision2D collision)
    {
    }

    public override void OnStateCollisionExit(Collision2D collision)
    {
    }

    public override void OnStateCollisionStay(Collision2D collision)
    {
    }

    public override void UpdateState()
    {
        if (horizontalControl != 0){
            rb2d.velocity = horizontalControl > 0 ? new Vector2(Runner.GetPlayerData().moveSpeed, rb2d.velocity.y) : new Vector2(-Runner.GetPlayerData().moveSpeed, rb2d.velocity.y);
        }

        // TODO: If Vertical Controls -> Will need to move up and down -> Shoten and Lengthen Rope
    }
}