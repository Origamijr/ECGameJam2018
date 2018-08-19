using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    
    private float tileSize = 0.64f;
    private int rows = 14;
    private int cols = 18;
    public GameObject floorTile;
    public GameObject wallTile;
    public GameObject[] upStairs;
    public GameObject[] downStairs;

    private Vector3 midpoint;

    private int layer = 0;
    private Room[] upRooms = new Room[3];
    private Room[] downRooms = new Room[3];

    private GameObject[][] grid;

    public GameObject player;
    private int roomSection = 0;

    public void RoomSetup() {

        grid = new GameObject[rows][];

        for (int row = 0; row < rows; row++) {
            grid[row] = new GameObject[cols];
            for (int col = 0; col < cols; col++) {

                GameObject tile = (row == 0 || row == rows - 1 || col == 0 || col == cols - 1) ? wallTile : floorTile;


                if (row == 0) {
                    if (downRooms[0]) {
                        if (col == 2) tile = downStairs[0];
                        if (col == 3) tile = downStairs[1];
                    }
                    if (downRooms[1]) {
                        if (col == cols / 2 - 1) tile = downStairs[0];
                        if (col == cols / 2) tile = downStairs[1];
                    }
                    if (downRooms[2]) {
                        if (col == cols - 4) tile = downStairs[0];
                        if (col == cols - 3) tile = downStairs[1];
                    }
                }
                if (row == rows - 1) {
                    if (upRooms[0]) {
                        if (col == 2) tile = upStairs[0];
                        if (col == 3) tile = upStairs[1];
                    }
                    if (upRooms[1]) {
                        if (col == cols / 2 - 1) tile = upStairs[0];
                        if (col == cols / 2) tile = upStairs[1];
                    }
                    if (upRooms[2]) {
                        if (col == cols - 4) tile = upStairs[0];
                        if (col == cols - 3) tile = upStairs[1];
                    }
                }

                GameObject instance = Instantiate(tile, tileSize * (new Vector3(col, -(rows * 5f * tileSize) * layer + row, 0f)), Quaternion.identity);
                grid[row][col] = instance;

                instance.transform.SetParent(transform);
            }
        }

        midpoint = (grid[0][0].transform.position + grid[rows - 1][cols - 1].transform.position) / 2;
    }

    public Vector3 GetMidpoint() {
        return midpoint;
    }

    public Room GetNextRoom() {
        switch (roomSection) {
            case 0:
                return upRooms[0];
            case 1:
                return upRooms[1];
            case 2:
                return upRooms[2];
            case 3:
                return downRooms[0];
            case 4:
                return downRooms[1];
            case 5:
                return downRooms[2];
            default:
                return null;
        }
    }

    public int GetRoomSection() {
        return roomSection;
    }
    
    public void SetLayer(int layer) {
        this.layer = layer;
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

    public void Delete() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    // Use this for initialization
    void Start() {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update() {

    }

    public void setPlayerToSection(int section) {
        Debug.Log(section);

        float x, y;

        if (section < 3) {
            y = midpoint.y + (tileSize * 5);
        } else {
            y = midpoint.y - (tileSize * 5);
        }

        switch (section % 3) {
            case 0:
                x = midpoint.x - (tileSize * 5);
                break;
            case 2:
                x = midpoint.x + (tileSize * 5);
                break;
            default:
                x = midpoint.x;
                break;
        }

        player.transform.position = new Vector3(x, y, -0.1f);
    }

    public void UpdatePlayerPosition() {
        Vector3 playerPos = player.GetComponent<PlayerController>().GetPosition();

        roomSection = ((playerPos.y > midpoint.y) ? 0 : 3) + ((Mathf.Abs((float)(playerPos.x - midpoint.x)) < tileSize * 3) ? 1 : (playerPos.x < midpoint.x) ? 0 : 2);
    }
}
