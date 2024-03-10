using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField] private float grappleLenth;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer rope;
    

    private bool grappled;
    private Vector2 grapplePoint;
    private DistanceJoint2D grappleJoint;

    void Start() {
        if (grappleJoint == null) {
            grappleJoint = GetComponent<DistanceJoint2D>();
        }
        Debug.Assert(grappleJoint != null, this + " has no attached grapple joint");

        grappleJoint.enabled = false;

        rope.enabled = false;
    }

    void Update() {
        if (grappled){
            rope.SetPosition(1, transform.position);
        }
    }

    public bool ShootGrapple(Vector2 direction){
        RaycastHit2D hit = Physics2D.Raycast(
            origin: transform.position,
            // origin: Camera.main.ScreenToWorldPoint(Input.mousePosition),
            direction: direction.normalized,
            distance: Mathf.Infinity, // TODO: Need to change this to length
            layerMask: grappleLayer
        );

        if (hit.collider != null){
            grappled = true;
            
            grapplePoint = hit.point;
            grappleJoint.connectedAnchor = grapplePoint;
            grappleJoint.enabled = true;
            grappleJoint.distance = grappleLenth;
            rope.SetPosition(0, grapplePoint);
            rope.SetPosition(1, transform.position);
            rope.enabled = true;
        }

        return grappled;
    }

    public void ReleaseGrapple(){
        if (grappled){
            grappleJoint.enabled = false;
            rope.enabled = false;
            grappled = false;
        }
    }

    public bool GetGrappled(){
        return grappled;
    }

}