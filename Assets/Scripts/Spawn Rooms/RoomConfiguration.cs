public class RoomConfiguration 
{
    private RoomExits roomExits;

    private RoomType roomType;


    public RoomExits RoomExits { get => roomExits; set => roomExits = value; }
    public RoomType RoomType { get => roomType; set => roomType = value; }

    
    public RoomConfiguration(RoomExits roomExits, RoomType roomType) {
        RoomExits = roomExits;
        RoomType = roomType;
    }

    public override string ToString()
    {
        return $"{roomType}: {roomExits}";
    }
}