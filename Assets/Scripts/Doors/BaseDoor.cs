using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseDoor : MonoBehaviour
{
    protected bool closed;

    public virtual void DoorClosed(bool closed){
        this.closed = closed;
        CloseDoorAnimation();
    }

    protected abstract void CloseDoorAnimation();
}