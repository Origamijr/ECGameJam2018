using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public float tileSize = 0.64f;
    public int rows = 16;   // does not count walls
    public int cols = 16;
    public GameObject floorTile;
    public GameObject wallTile;

    private int layer = 0;
    private Room[] upRooms = new Room[3];
    private Room[] downRooms = new Room[3];

    private GameObject[][] grid;

    public void RoomSetup() {

        grid = new GameObject[rows + 2][];

        for (int row = 0; row < rows + 2; row++) {
            grid[row] = new GameObject[cols + 2];
            for (int col = 0; col < cols + 2; col++) {

                GameObject tile = (row == 0 || row == rows + 1 || col == 0 || col == cols + 1) ? wallTile : floorTile;

                GameObject instance = Instantiate(tile, tileSize * (new Vector3(col, -5 * layer + row, 0f)), Quaternion.identity);
                grid[row][col] = instance;

                instance.transform.SetParent(transform);
            }
        }
    }

    public void SetUpRoom(Room r1, Room r2, Room r3) {
        upRooms = new Room[3];

        upRooms[0] = r1;
        upRooms[1] = r2;
        upRooms[2] = r3;
    }

    public void SetDownRoom(Room r1, Room r2, Room r3) {
        downRooms = new Room[3];

        downRooms[0] = r1;
        downRooms[1] = r2;
        downRooms[2] = r3;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
