using UnityEngine;

[CreateAssetMenu(menuName = "Room Manager/Multiple Room Template")]
public class MultipleRoomExitsTemplate : ScriptableObject {
    public GameObject[] LeftExits;
    public GameObject[] RightExits;
    public GameObject[] TopExits;
    public GameObject[] BottomExits;
    public GameObject[] LeftRightExits;
    public GameObject[] TopBottomExits;
    public GameObject[] TopLeftExits;
    public GameObject[] TopRightExits;
    public GameObject[] BottomLeftExits;
    public GameObject[] BottomRightExits;
    public GameObject[] BottomLeftRightExits;
    public GameObject[] TopLeftRightExits;
    public GameObject[] TopBottomLeftExits;
    public GameObject[] TopBottomRightExits;
    public GameObject[] TopBottomLeftRightExits;


    public GameObject[] GetCorrespondingRooms(RoomExits roomExits){
        switch (roomExits)
        {
            case RoomExits.LeftExit:
                return LeftExits;
            case RoomExits.RightExit:
                return RightExits;
            case RoomExits.TopExit:
                return TopExits;
            case RoomExits.BottomExit:
                return BottomExits;
            case RoomExits.LeftRightExit:
                return LeftRightExits;
            case RoomExits.TopBottomExit:
                return TopBottomExits;
            case RoomExits.TopLeftExit:
                return TopLeftExits;
            case RoomExits.TopRightExit:
                return TopRightExits;
            case RoomExits.BottomLeftExit:
                return BottomLeftExits;
            case RoomExits.BottomRightExit:
                return BottomRightExits;
            case RoomExits.BottomLeftRightExit:
                return BottomLeftRightExits;
            case RoomExits.TopLeftRightExit:
                return TopLeftRightExits;
            case RoomExits.TopBottomLeftExit:
                return TopBottomLeftExits;
            case RoomExits.TopBottomRightExit:
                return TopBottomRightExits;
            case RoomExits.TopBottomLeftRightExit:
                return TopBottomLeftRightExits;
            default:
                // Return an empty array if roomExits does not match any case
                Debug.LogError($"Unable to recognise room of type {roomExits}");
                return new GameObject[0];
        }
    }
}