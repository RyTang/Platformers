using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Room {
    public Vector2 gridPosition;
    public RoomType roomType;
    public RoomExits roomExits;
    

    public Room(Vector2 gridPosition, RoomType roomType = RoomType.NormalRoom){
        this.gridPosition = gridPosition;
        this.roomType = roomType;
    }
}