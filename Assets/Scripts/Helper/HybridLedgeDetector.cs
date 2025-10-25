using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Combines raycast-based ledge detection with prefab-based LedgeIndicators.
/// Detects ledges in front of the player and returns hang/stand positions.
/// Includes Gizmos for visual debugging.
/// </summary>
public class HybridLedgeDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Layer mask used to detect ground and ledges.")]
    public LayerMask groundMask;

    [Tooltip("How far forward to check for walls.")]
    public float wallCheckDistance = 0.3f;

    [Tooltip("Vertical offset above wall hit to start ledge-top ray.")]
    public float ledgeTopCheckOffset = 0.2f;

    [Tooltip("Vertical depth to check for ledge top.")]
    public float ledgeTopCheckDepth = 1f;

    [Tooltip("Height to check above ledge to ensure it's clear (no wall above).")]
    public float clearanceCheckHeight = 1f;

    [Tooltip("Maximum distance player can be from hang position to grab ledge.")]
    public float maxGrabDistance = 0.5f;

    [Header("Offsets (Fallback if no LedgeIndicator)")]
    [Tooltip("Offset from ledge top to hang position.")]
    public Vector2 hangOffset = new Vector2(-0.2f, -0.3f);

    [Tooltip("Offset from ledge top to stand position.")]
    public Vector2 standOffset = new Vector2(0.2f, 1f);

    [Header("References")]
    [Tooltip("Reference to your LayerCheck (for overlap checks).")]
    public LayerCheck ledgeCheck;

    [Tooltip("Reference to the player controller.")]
    public PlayerController Runner;

    // Gizmo Debugging
    private Vector2 lastWallRayStart;
    private Vector2 lastClearanceOrigin;
    private Vector2 lastHangPos;
    private bool lastDetectionSuccess;
    private bool lastClearanceCheckFailed;
    private bool lastDistanceCheckFailed;

    /// <summary>
    /// Attempts to find a ledge either via raycast or prefab indicators.
    /// </summary>
    public bool TryFindLedge(out LedgeIndicator ledgeIndicator, out Vector2 hangPos, out Vector2 standPos)
    {
        ledgeIndicator = null;
        hangPos = Vector2.zero;
        standPos = Vector2.zero;

        if (!Runner) { Debug.LogWarning("HybridLedgeDetector missing Runner reference!"); return false; }
        if (!ledgeCheck) { Debug.LogWarning("HybridLedgeDetector missing LayerCheck reference!"); return false; }

        Vector2 handPos = ledgeCheck.transform.position;
        Vector2 facingDir = Runner.IsFacingRight() ? Vector2.right : Vector2.left;

        // Step 1: Forward wall ray
        lastWallRayStart = handPos;
        RaycastHit2D wallHit = Physics2D.Raycast(handPos, facingDir, wallCheckDistance, groundMask);
        if (!wallHit)
        {
            lastDetectionSuccess = false;
            lastDistanceCheckFailed = false;
            return false;
        }

        // Step 2: Downward ledge-top ray
        Vector2 topOrigin = wallHit.point + Vector2.up * ledgeTopCheckOffset;
        RaycastHit2D topHit = Physics2D.Raycast(topOrigin, Vector2.down, ledgeTopCheckDepth, groundMask);
        if (!topHit)
        {
            lastDetectionSuccess = false;
            lastDistanceCheckFailed = false;
            return false;
        }

        // Step 2.5: Clearance check - ensure there's no wall above the ledge
        // Add Wall More Depth to prevent false positives
        float dirX = facingDir.x;
        hangPos = topHit.point + new Vector2(dirX * hangOffset.x, hangOffset.y);
        standPos = topHit.point + new Vector2(dirX * standOffset.x, standOffset.y);

        Vector2 clearanceOrigin = standPos + Vector2.down * 0.2f; // Remove small offset below the stand position
        lastClearanceOrigin = clearanceOrigin;
        RaycastHit2D clearanceHit = Physics2D.Raycast(clearanceOrigin, Vector2.up, clearanceCheckHeight, groundMask);
        if (clearanceHit)
        {
            // There's a wall above this ledge, so it's not a valid grab point
            lastDetectionSuccess = false;
            lastDistanceCheckFailed = false;
            lastClearanceCheckFailed = true;
            return false;
        }
        lastClearanceCheckFailed = false;

        // Step 3: Check for prefab indicators nearby
        List<GameObject> ledges = ledgeCheck.GetObjectsInCheck();
        if (ledges.Count > 0)
        {
            ledgeIndicator = ledges
                .Select(go => go.GetComponent<LedgeIndicator>())
                .Where(ind => ind != null)
                .OrderBy(ind => Vector2.Distance(handPos, ind.transform.position))
                .FirstOrDefault();
        }

        // Step 4: Determine hang & stand positions
        if (ledgeIndicator != null)
        {
            hangPos = ledgeIndicator.GetHangPosition();
            standPos = ledgeIndicator.GetStandPosition();
        }

        // Step 5: Distance validation - check if player is close enough to grab the ledge
        float distanceToHangPos = Vector2.Distance(handPos, hangPos);
        if (distanceToHangPos > maxGrabDistance)
        {
            // Store debug info for failed distance check
            lastHangPos = hangPos;
            lastDistanceCheckFailed = true;
            lastDetectionSuccess = false;
            return false;
        }

        // Store results for gizmos
        lastHangPos = hangPos;
        lastDistanceCheckFailed = false;
        lastDetectionSuccess = true;

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Runner) return;

        Vector2 facingDir = Runner.IsFacingRight() ? Vector2.right : Vector2.left;

        // Always draw the forward check ray
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.6f); // Orange, semi-transparent
        Gizmos.DrawRay(lastWallRayStart, facingDir * wallCheckDistance);

        if (lastDetectionSuccess)
        {
            // Valid ledge - draw in green
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lastHangPos, 0.1f);
            
            // Draw clearance check (simplified)
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawLine(lastClearanceOrigin, lastClearanceOrigin + Vector2.up * clearanceCheckHeight);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(lastHangPos + Vector2.up * 0.15f, "GRAB");
#endif
        }
        else if (lastClearanceCheckFailed)
        {
            // Wall above - draw in red
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawLine(lastClearanceOrigin, lastClearanceOrigin + Vector2.up * clearanceCheckHeight);
            
#if UNITY_EDITOR
            UnityEditor.Handles.Label(lastClearanceOrigin + Vector2.up * 0.3f, "✗ Wall Above");
#endif
        }
        else if (lastDistanceCheckFailed)
        {
            // Too far - draw in yellow
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawWireSphere(lastHangPos, 0.08f);
            Gizmos.DrawLine(lastWallRayStart, lastHangPos);
            
#if UNITY_EDITOR
            float distance = Vector2.Distance(lastWallRayStart, lastHangPos);
            UnityEditor.Handles.Label(lastHangPos + Vector2.up * 0.15f, $"✗ Too Far ({distance:F2}m)");
#endif
        }
    }
}
