using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    public float tileSize = 0.64f;
    public int rows = 16;
    public int cols = 16;
    public GameObject floorTile;
    public GameObject wallTile;

    private int layer = 0;
    private Room[] upRooms = new Room[3];
    private Room[] downRooms = new Room[3];

    public void RoomSetup() {

        for (int row = -1; row <= rows; row++) {
            for (int col = -1; col <= cols; col++) {
                GameObject tile = (row == -1 || row == rows || col == -1 || col == cols) ? wallTile : floorTile;

                GameObject instance = Instantiate(tile, tileSize * (new Vector3(col, -5 * layer + row, 0f)), Quaternion.identity);

                instance.transform.SetParent(transform);
            }
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
