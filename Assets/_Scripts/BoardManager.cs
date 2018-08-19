using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    public GameObject roomPrefab;
    
    private Room currRoom = null;

    private int maxWidth = 7;

    private List<Room[]> board;

    void InititializeBoard() {
        board = new List<Room[]>();

        // start layer (control tutorial)
        Room[] layer1 = new Room[maxWidth];

        currRoom = CreateRoom();
        layer1[maxWidth / 2] = currRoom;

        board.Add(layer1);

        // second layer (combat tutorial)
        Room[] layer2 = new Room[maxWidth];

        layer2[maxWidth / 2] = CreateRoom();
        layer1[maxWidth / 2].SetDownRoom(null, layer2[maxWidth / 2], null);
        layer2[maxWidth / 2].SetUpRoom(null, layer1[maxWidth / 2], null);

        board.Add(layer2);
        
        // third layer (sentry tutorial)
        Room[] layer3 = new Room[maxWidth];

        layer3[maxWidth / 2] = CreateRoom();
        layer2[maxWidth / 2].SetDownRoom(null, layer3[maxWidth / 2], null);
        layer3[maxWidth / 2].SetUpRoom(null, layer2[maxWidth / 2], null);

        board.Add(layer3);

        // fourth layer (split)
        Room[] layer4 = new Room[maxWidth];

        layer4[maxWidth / 2 - 1] = CreateRoom();
        layer4[maxWidth / 2 + 1] = CreateRoom();
        layer3[maxWidth / 2].SetDownRoom(layer4[maxWidth / 2 - 1], null, layer4[maxWidth / 2 + 1]);
        layer4[maxWidth / 2 - 1].SetUpRoom(null, layer3[maxWidth / 2], null);
        layer4[maxWidth / 2 + 1].SetUpRoom(null, layer3[maxWidth / 2], null);

        board.Add(layer4);


        // generate room
        currRoom.RoomSetup();
    }

    Room CreateRoom() {
        Room room = Instantiate(roomPrefab).GetComponent<Room>();

        return room;
    }

    void GenerateLayer() {
        Room[] lastRow = board[board.Count - 1];
    }

    // Use this for initialization
    void Start () {
        currRoom = Instantiate(roomPrefab).GetComponent<Room>();
        currRoom.RoomSetup();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
