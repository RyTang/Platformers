/// <summary>
/// Stores the Type of Exits that the room has and can contain
/// </summary>
public enum RoomExits {
    // Using Bitwise operations to manage the types of rooms
    LeftExit = 1 << 0,
    RightExit = 1 << 1,
    TopExit = 1 << 2,
    BottomExit = 1 << 3,

    // Define additional room types with combinations of exits
    LeftRightExit = LeftExit | RightExit,                  // Room has exits on the left and right
    TopBottomExit = TopExit | BottomExit,                  // Room has exits on the top and bottom
    TopLeftExit = TopExit | LeftExit,                      // Room has exits on the top and left
    TopRightExit = TopExit | RightExit,                    // Room has exits on the top and right
    BottomLeftExit = BottomExit | LeftExit,                // Room has exits on the bottom and left
    BottomRightExit = BottomExit | RightExit,              // Room has exits on the bottom and right
    BottomLeftRightExit = BottomExit | LeftExit | RightExit,
    TopLeftRightExit = TopExit | LeftExit | RightExit,     // Room has exits on the top, left, and right
    TopBottomLeftExit = TopExit | BottomExit | LeftExit,   // Room has exits on the top, bottom, and left
    TopBottomRightExit = TopExit | BottomExit | RightExit, // Room has exits on the top, bottom, and right
    TopBottomLeftRightExit = TopExit | LeftExit | RightExit | BottomExit,
}