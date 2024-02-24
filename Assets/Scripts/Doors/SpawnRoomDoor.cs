using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnRoomDoor : BaseDoor
{
    public SpawnRoom spawnRoom;
    public Animator animator;
    protected override void CloseDoorAnimation()
    {
        animator.SetBool("Door_Closed", closed);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == (int) GameLayers.PLAYER){
            spawnRoom.OnCharacterEnterRoom();
        }
    }
}