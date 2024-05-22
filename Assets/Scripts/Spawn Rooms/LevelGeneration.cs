using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class LevelGeneration : MonoBehaviour{
    public RoomTemplates roomTemplates;
    public Vector2 worldSize = new Vector2(4, 4);
    public Vector2 roomSize = new Vector2(20, 20);
    public int numberOfRooms = 20;

    private Room[,] rooms;
    private List<Vector2> takenPositions = new List<Vector2>();
    private int gridSizeX, gridSizeY;
    

    private void Start() {
        // Don't spawn more rooms than what is allowed
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y *2)) {
            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y *2));
        }

        gridSizeX = Mathf.RoundToInt(worldSize.x);
        gridSizeY = Mathf.RoundToInt(worldSize.y);
        CreateRooms(); // Layouts the actual Map;
        SetRoomDoors(); // Set where the Door should exists
        DrawMap();
    }

    private void DrawMap()
    {
        foreach (Room room in rooms) {
            if (room == null) continue;

            // Generate the Room

            // Spawn Room Instance
            Vector2 roomPosition = room.gridPosition;
            roomPosition.x *= roomSize.x;
            roomPosition.y *= roomSize.y;

            // Instanstiate Room
            // Should just spawn the template here ady
            GameObject chosenRoom;
            GameObject spawnedInstance;

            // Get Room Object
            RoomTypeTemplatesData roomTypeTemplates = roomTemplates.roomTypeTemplatesDatas.First(templateRoom => templateRoom.roomType == room.roomType);
            int index;
            if (roomTypeTemplates == null){
                Debug.LogError($"Unable to find corresponding room for room of type {room.roomType}");
                continue; // If null move on next execution;
            }

            if (roomTypeTemplates.isMultipleRoomType){
                GameObject[] roomChoices = roomTypeTemplates.multipleRoomTemplate.GetCorrespondingRooms(room.roomExits);
                index = Random.Range(0, roomChoices.Length);
                chosenRoom = roomChoices[index];

                spawnedInstance = Instantiate(chosenRoom, roomPosition, Quaternion.identity);
            }
            else {
                GameObject[] singleRoomTemplates = roomTypeTemplates.singleRoomTemplates;
                index = Random.Range(0, singleRoomTemplates.Length);
                chosenRoom = singleRoomTemplates[index];
                
                spawnedInstance = Instantiate(chosenRoom, roomPosition, Quaternion.identity);
                // Configure Doors

                spawnedInstance.GetComponent<SingleRoomInstance>().SetExits(room.roomExits);
            }

            // Set Parent of Transform
            spawnedInstance.transform.SetParent(transform);
        }
    }


    private void CreateRooms()
    {
        rooms = new Room[gridSizeX * 2, gridSizeY * 2];
        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, RoomType.NormalRoom);

        takenPositions.Insert(0, Vector2.zero);
        Vector2 checkPos = Vector2.zero;

        // Magic numbers
        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;

        // Add Rooms
        for (int i = 0; i < numberOfRooms - 1; i++){
            float randomPerc = i / ((float) numberOfRooms - 1);
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);

            // Grab new position
            checkPos = NewPosition();

            // Test new position
            if (NumberOfNeighbours(checkPos, takenPositions) > 1 && Random.value > randomCompare) {
                int iterations = 0;

                do {
                    checkPos = SelectiveNewPosition();
                    iterations++;
                } while (NumberOfNeighbours(checkPos, takenPositions) > 1 && iterations < 100);

                if (iterations >= 100) {
					Debug.LogError("error: could not create with fewer neighbors than : " + NumberOfNeighbours(checkPos, takenPositions));
			    }
            }

            // Finalise Positions
            // TODO: Add the different room Types here
            rooms[(int) checkPos.x + gridSizeX, (int) checkPos.y + gridSizeY] = new Room(checkPos, RoomType.NormalRoom);
            takenPositions.Insert(0, checkPos);
        }
    }

    private Vector2 NewPosition()
    {
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;
        do {
            int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1)); // Pick random room
            x = (int) takenPositions[index].x; // Capture Xy position of room
            y = (int) takenPositions[index].y;

            bool upDown = Random.value < 0.5f; // Check if should be horizontal or vertical axis
            bool positive = Random.value < 0.5f; // Check go positive or negative on the selected axis

            if (upDown){ //find the position bnased on the above bools
				y += positive ? 1 : -1;
			}
            else{
				x += positive ? 1 : -1;
			}

            checkingPos = new Vector2(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY); // Make sure position is valid

        return checkingPos;
    }

    private Vector2 SelectiveNewPosition()
    {
        int index = 0, inc = 0;
        int x = 0, y = 0;
        Vector2 checkingPos = Vector2.zero;

        do {
            inc = 0;

            // Find a Room that has a neighbour at least so that we can ensure that branching happens
            do {
                index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
                inc++;
            } while (NumberOfNeighbours(takenPositions[index], takenPositions) > 1 && inc < 100);

            x = (int) takenPositions[index].x;
            y = (int) takenPositions[index].y;

            bool upDown = Random.value < 0.5f; // Check if should be horizontal or vertical axis
            bool positive = Random.value < 0.5f; // Check go positive or negative on the selected axis
            
            if (upDown){ //find the position bnased on the above bools
				y += positive ? 1 : -1;
			}
            else{
				x += positive ? 1 : -1;
			}
            checkingPos = new Vector2(x, y);
        } while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY); // Make sure position is valid

        if (inc >= 100) {
            Debug.LogError("Error: could not find position with only one neighbor");
        }

        return checkingPos;
    }

    private int NumberOfNeighbours(Vector2 checkPos, List<Vector2> takenPositions)
    {
        int numNeighbours = 0;
        if (takenPositions.Contains(checkPos + Vector2.right)) numNeighbours++;
        if (takenPositions.Contains(checkPos + Vector2.left)) numNeighbours++;
        if (takenPositions.Contains(checkPos + Vector2.up)) numNeighbours++;
        if (takenPositions.Contains(checkPos + Vector2.down)) numNeighbours++;

        return numNeighbours;
    }

    private void SetRoomDoors()
    {
        for (int x = 0; x < gridSizeX * 2; x++){
            for (int y = 0; y < gridSizeY * 2; y++){
                if (rooms[x,y] == null) continue;

                bool doorBot, doorTop, doorLeft, doorRight;

                doorBot = y - 1 >= 0 && rooms[x, y - 1] != null;
                doorTop = y + 1 < gridSizeY * 2 && rooms[x, y + 1] != null;
                doorLeft = x - 1 >= 0 && rooms[x - 1, y] != null;
                doorRight = x + 1 < gridSizeX * 2 && rooms[x + 1, y] != null;

                
                RoomExits requiredExits = (doorLeft ? RoomExits.LeftExit : 0) |
                    (doorRight ? RoomExits.RightExit : 0) |
                    (doorTop ? RoomExits.TopExit : 0) |
                    (doorBot ? RoomExits.BottomExit : 0);


                rooms[x, y].roomExits = requiredExits;
            }
        }
    }
}
