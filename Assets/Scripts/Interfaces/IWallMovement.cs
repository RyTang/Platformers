using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWallMovement {
    void TouchingWall(bool touching);

    void WallJump();
}