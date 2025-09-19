using System.Collections.Generic;
using UnityEngine;

public class LayerCheck : MonoBehaviour
{
    [SerializeField] private Vector2 layerCheckBox = new Vector2(1, 1);
    [SerializeField] private LayerMask collisionMask;

    public bool Check(){
        return Physics2D.OverlapBox(transform.position, layerCheckBox, 0, collisionMask);
    }

    public List<GameObject> GetObjectsInCheck(){

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, layerCheckBox, 0, collisionMask);
        
        List<GameObject> gameObjects = new List<GameObject>();
        foreach (Collider2D collider in colliders)
        {
            gameObjects.Add(collider.gameObject);
        }

        return gameObjects;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, layerCheckBox);
    }
}