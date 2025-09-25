using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGoal : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // TODO: Make this better
        {
            // TODO: Should create the scoreboard
            GameManager.Instance.ReloadGame();
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
