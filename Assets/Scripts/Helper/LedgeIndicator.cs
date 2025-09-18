using UnityEngine;

public class LedgeIndicator : MonoBehaviour
{
    public bool isLeftLedge;
    [SerializeField] private Vector2 hangPosition;
    [SerializeField] private Vector2 standPosition;


    public Vector2 GetHangPosition(){
        return (Vector2) transform.position + hangPosition;
    }

    public Vector2 GetStandPosition(){
        return (Vector2) transform.position + standPosition;
    }

    public void SetIsLeftLedge(bool isLeftLedge){
        this.isLeftLedge = isLeftLedge;

        // If Not facing right direction, flip it
        if (this.isLeftLedge){
            hangPosition = new Vector2(hangPosition.x * -1, hangPosition.y);
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2) transform.position + hangPosition, Vector2.one * 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2) transform.position + standPosition, Vector2.one * 0.5f);

        gameObject.GetComponentsInChildren<LedgeIndicator>();
    }
}