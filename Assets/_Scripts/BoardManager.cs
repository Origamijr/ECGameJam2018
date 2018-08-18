using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public float tileSize = 0.64f;
    public int rows = 16;
    public int cols = 16;
    public GameObject floorTile;
    public GameObject wallTile;

    public Transform board;

    void BoardSetup() {
        board = new GameObject("Board").transform;

        for (int row = -1; row <= rows; row++) {
            for (int col = -1; col <= cols; col++) {
                GameObject tile = (row == -1 || row == rows || col == -1 || col == cols) ? wallTile : floorTile;

                GameObject instance = Instantiate(tile, tileSize * (new Vector3(col, row, 0f)), Quaternion.identity);

                instance.transform.SetParent(board);
            }
        }
    }

	// Use this for initialization
	void Start () {
        BoardSetup();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
