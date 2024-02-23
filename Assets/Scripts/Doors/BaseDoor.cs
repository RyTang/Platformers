using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseDoor : MonoBehaviour
{
    protected bool opened;

    public virtual void DoorOpened(bool opened){
        this.opened = opened;
        OpenDoorAnimation();
    }

    protected abstract void OpenDoorAnimation();
}