using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TouchingLayers : MonoBehaviour
{
    [SerializeField] UnityEvent touchingLayerEvent;
    [SerializeField] UnityEvent stopTouchingLayerEvent;

    [SerializeField] float coyoteTime;

    [SerializeField] GameLayers[] gameLayers;



    void OnTriggerStay2D(Collider2D other)
    {
        if (gameLayers.Contains((GameLayers) other.gameObject.layer)){
            touchingLayerEvent.Invoke();

            CancelInvoke(nameof(StoppedTouching));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (gameLayers.Contains((GameLayers) other.gameObject.layer)){
            Invoke(nameof(StoppedTouching), coyoteTime * Time.deltaTime);
        }
    }


    void StoppedTouching(){
        stopTouchingLayerEvent.Invoke();
    }

}
