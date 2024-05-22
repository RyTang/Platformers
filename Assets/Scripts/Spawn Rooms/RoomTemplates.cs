using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Room Manager/Room Templates")]
public class RoomTemplates : ScriptableObject {
    public List<RoomTypeTemplatesData> roomTypeTemplatesDatas;
}

