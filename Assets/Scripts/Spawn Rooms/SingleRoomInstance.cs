using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SingleRoomInstance : MonoBehaviour{
    public GameObject leftExit, rightExit, topExit, bottomExit;

    
    
    private void Awake() {
        leftExit.SetActive(true);
        rightExit.SetActive(true);
        bottomExit.SetActive(true);
        topExit.SetActive(true);
    }

    public void SetExits(RoomExits roomExits){
        Debug.Log($"Room Exits being set: {roomExits}");
        if ((roomExits & RoomExits.LeftExit) != 0) leftExit.SetActive(false);
        if ((roomExits & RoomExits.RightExit) != 0) rightExit.SetActive(false);
        if ((roomExits & RoomExits.TopExit) != 0) topExit.SetActive(false);
        if ((roomExits & RoomExits.BottomExit) != 0) bottomExit.SetActive(false);
    }
}