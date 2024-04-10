using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

public class SpawnRoomManager : MonoBehaviour
{
    // TODO: maybe add in the settings class here to manage the level generation
    public GameObject roomTemplate;
    public int gridSizeX;
    public int gridSizeY;

    public Vector2 roomSize = new Vector2(10, 10);

    private void Start() {
        LevelGenerator levelGeneration = new LevelGenerator(gridSizeX, gridSizeY, roomSize);
        RoomConfiguration[,] levelLayout = levelGeneration.StartLevelGeneration();

        GenerateRoom(levelLayout);
        // TODO: Print out the grid as blocks in the game

        Debug.Log("Finished pathfinding");
        // PrintGrid(levelLayout);
        // TODO: ADD IN REMAINING BLOCKS
        // TODO: ADD VARIABLITY IN SIZE SPAWNED
    }

    private void GenerateRoom(RoomConfiguration[,] levelLayout) {
        for (int x = 0; x < levelLayout.GetLength(0); x++){
            for (int y = 0; y < levelLayout.GetLength(1); y++){
                if (levelLayout[x, y] == null) continue;

                // Generate the room
                Vector2 spawnPosition = new Vector2(x * roomSize.x, y * roomSize.y);
                GameObject spawnedRoom = Instantiate(roomTemplate, spawnPosition, Quaternion.identity, transform);
                // TODO: Modify based on configuration
                RoomConfiguration roomConfiguration = levelLayout[x, y];
                
                BaseRoom room = spawnedRoom.GetComponent<BaseRoom>();

                if ((roomConfiguration.RoomExits & RoomExits.LeftExit) != 0) room.leftExit.SetActive(false);
                if ((roomConfiguration.RoomExits & RoomExits.RightExit) != 0) room.rightExit.SetActive(false);
                if ((roomConfiguration.RoomExits & RoomExits.TopExit) != 0) room.topExit.SetActive(false);
                if ((roomConfiguration.RoomExits & RoomExits.BottomExit) != 0) room.bottomExit.SetActive(false);;

            }
        }
    }

    private static void PrintGrid(RoomConfiguration[,] grid) {
        int rows = grid.GetLength(0);
        int columns = grid.GetLength(1);


        for (int i = 0; i < rows; i++)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int j = 0; j < columns; j++)
            {
                if (grid[i, j] != null){
                    Debug.Log("printing");
                    sb.Append(grid[i, j].ToString());}
                else {
                    sb.Append("null");
                }
                sb.Append(" ");
            }
            Debug.Log(sb);
        }
    }

}