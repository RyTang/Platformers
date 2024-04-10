using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction {
    Left,
    Right,
    Top,
    Bottom
}

public class LevelGenerator {
    // TODO: Add in a grid system
    private RoomConfiguration[,] levelLayout;
    private int gridSizeX;
    private int gridSizeY;

    private Vector2 roomSize; // Allow variability in the room Size
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Vector2 currentPosition;
    private bool doneGenerating = false;    
    private Direction nextDirection;

    private int counter = 0;

    public LevelGenerator(int gridSizeX, int gridSizeY, Vector2 roomSize){
        this.gridSizeX = gridSizeX;
        this.gridSizeY = gridSizeY;
        this.roomSize = roomSize;

        doneGenerating = false;
    }

    public RoomConfiguration[,] StartLevelGeneration(){
        levelLayout = new RoomConfiguration[gridSizeX, gridSizeY];

        startPosition = new Vector2(0, Random.Range(0, gridSizeY));
        currentPosition = startPosition;

        RoomConfiguration createdRoom = SelectRoom(RoomExits.RightExit);

        levelLayout[(int) currentPosition.x, (int) currentPosition.y] = createdRoom;

        nextDirection = ChooseDirection();
        Debug.Log($"Start Room: {startPosition}, of Room Type {createdRoom}, current Direction: {nextDirection}");

        while (!doneGenerating && counter <= 20){    
            Move(nextDirection);
            counter += 1;
        }


        return levelLayout;
    }


    private void Move(Direction direction){
        // NOTE: Dealing with grid means that we are in reversed
        RoomExits exitsNeeded = RoomExits.RightExit;
        switch (direction) {
            case Direction.Right:
                if (currentPosition.x + 1 < gridSizeX){ // if can spawn to the Right
                    
                    // Move to the Right
                    currentPosition = new Vector2(currentPosition.x + 1, currentPosition.y);
                    Debug.Log($"{currentPosition}");
                    // Select a Room

                    nextDirection = ChooseDirection();

                    if (nextDirection == Direction.Left){
                        int i = Random.Range(1, 2);
                        nextDirection = i == 1 ? Direction.Top : Direction.Bottom;  
                    }

                    exitsNeeded = RoomExits.LeftExit | DirectionToExitConversion(nextDirection);
                    
                }
                // If reached border then reached end
                else {
                    doneGenerating = true;
                    endPosition = currentPosition;
                    Debug.Log($"End Room: {endPosition}");

                    return;
                }
                break;

            case Direction.Top:
                if (currentPosition.y + 1 < gridSizeY){
                    // Adding the rest of the required data

                    // Move to the top
                    currentPosition = new Vector2(currentPosition.x, currentPosition.y + 1);

                    // Select A room

                    nextDirection = ChooseDirection();

                    if (nextDirection == Direction.Bottom){
                        int i = Random.Range(1, 2);
                        nextDirection = i == 1 ? Direction.Top : Direction.Right;  
                    }

                    exitsNeeded = RoomExits.BottomExit | DirectionToExitConversion(nextDirection);
                }
                // If reached border then reached top
                else {
                    // TODO: Check type of room that was created and see if it has a right exit
                    RoomConfiguration currentRoomConfiguration = levelLayout[(int) currentPosition.x, (int) currentPosition.y];
                    // If it doesn't contain a right exit, change it to have 1
                    if ((currentRoomConfiguration.RoomExits & RoomExits.RightExit) == 0){
                        exitsNeeded = RoomExits.BottomLeftRightExit;
                    }
                    nextDirection = Direction.Right;
                }
                break;
            case Direction.Bottom:
                if (currentPosition.y - 1 > 0){
                    // Move to bottom
                    currentPosition = new Vector2(currentPosition.x, currentPosition.y - 1);

                    // select a room

                    nextDirection = ChooseDirection();

                    if (nextDirection == Direction.Top){
                        int i = Random.Range(1, 2);
                        nextDirection = i == 1 ? Direction.Bottom : Direction.Right;  
                    }

                    exitsNeeded = RoomExits.TopExit | DirectionToExitConversion(nextDirection);
                }
                // If reached border then reached bottom
                else {
                    // TODO: Check type of room that was created and see if it has a right exit
                    RoomConfiguration currentRoomConfiguration = levelLayout[(int) currentPosition.x, (int) currentPosition.y];
                    // If it doesn't contain a right exit, change it to have 1
                    if ((currentRoomConfiguration.RoomExits & RoomExits.RightExit) == 0){
                        exitsNeeded = RoomExits.TopLeftRightExit;
                    }
                    nextDirection = Direction.Right;
                }
                break;

            // TODO: THINK OF HOW TO HANDLE MOVING TO THE LEFT TO ADD VARIABILITY
        }

        RoomConfiguration createdRoom = SelectRoom(exitsNeeded);
        levelLayout[(int) currentPosition.x, (int) currentPosition.y] = createdRoom;
        Debug.Log($"Current Position: {currentPosition}, room configuration added: {createdRoom}, next Direction: {nextDirection}");
    }

    private RoomExits DirectionToExitConversion(Direction direction){
        switch (direction){
            case Direction.Left:
                return RoomExits.LeftExit;
            case Direction.Right:
                return RoomExits.RightExit;
            case Direction.Top:
                return RoomExits.TopExit;
            case Direction.Bottom:
                return RoomExits.BottomExit;
            default:
                return RoomExits.TopBottomLeftRightExit;
        }
    }



    private Direction ChooseDirection(){
        Direction currentDirection;
        switch (Random.Range(1, 4)){
            case 1:
                currentDirection = Direction.Top;
                break;
            case 2:
                currentDirection = Direction.Right;
                break;
            case 3:
                currentDirection = Direction.Bottom;
                break;
            default:
                currentDirection = Direction.Right;
                break;
        }

        return currentDirection;
    }


    /// <summary>
    /// Creates a Room Configuration with at least the required room exits
    /// </summary>
    /// <param name="roomExitsNeeded">Room Exits that are needed to be included</param>
    /// <returns>A Room Type with the requested types</returns>
    private RoomConfiguration SelectRoom(RoomExits roomExitsNeeded){
        bool requireLeftExit = (roomExitsNeeded & RoomExits.LeftExit) != 0;
        bool requireRightExit = (roomExitsNeeded & RoomExits.RightExit) != 0;
        bool requireTopExit = (roomExitsNeeded & RoomExits.TopExit) != 0;
        bool requireBottomExit = (roomExitsNeeded & RoomExits.BottomExit) != 0;

        Debug.Log($"left exit: {requireLeftExit}, right exit: {requireRightExit}, top exit: {requireTopExit}, bottom exit: {requireBottomExit}");

        RoomExits requiredExits = (requireLeftExit ? RoomExits.LeftExit : 0) |
            (requireRightExit ? RoomExits.RightExit : 0) |
            (requireTopExit ? RoomExits.TopExit : 0) |
            (requireBottomExit ? RoomExits.BottomExit : 0);

        RoomExits[] roomExitsAvailable = System.Enum.GetValues(typeof(RoomExits))
            .Cast<RoomExits>()
            .Where(roomType => (roomType & requiredExits) == requiredExits && (roomType & ~requiredExits) != 0)
            .ToArray();

        RoomExits roomExitsChosen = roomExitsAvailable[Random.Range(0, roomExitsAvailable.Length)];

        RoomConfiguration createdRoom = new RoomConfiguration(roomExitsChosen, RoomType.NormalRoom);

        return createdRoom;
    }
}