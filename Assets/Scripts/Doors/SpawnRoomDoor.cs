using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnRoomDoor : BaseDoor
{
    public Animator animator;

    protected override void OpenDoorAnimation()
    {
        animator.SetBool("Door_Opened", opened);
    }
}