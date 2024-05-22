using System;
using UnityEngine;

[System.Serializable]
public class RoomTypeTemplatesData {
    public String name;
    public RoomType roomType;
    public bool isMultipleRoomType = false;
    public GameObject[] singleRoomTemplates;
    public MultipleRoomExitsTemplate multipleRoomTemplate;
}