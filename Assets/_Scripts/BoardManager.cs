using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    public GameObject mainCamera;
    public GameObject roomPrefab;
    
    private Room currRoom = null;

    private int maxWidth = 7;

    private List<Room[]> board;

    private bool changing = false;

    void InititializeBoard() {
        board = new List<Room[]>();

        // start layer (control tutorial)
        Room[] layer1 = new Room[maxWidth];

        currRoom = CreateRoom(0);
        layer1[maxWidth / 2] = currRoom;

        board.Add(layer1);

        // second layer (combat tutorial)
        Room[] layer2 = new Room[maxWidth];

        layer2[maxWidth / 2] = CreateRoom(1);
        layer1[maxWidth / 2].SetDownRoom(null, layer2[maxWidth / 2], null);
        layer2[maxWidth / 2].SetUpRoom(null, layer1[maxWidth / 2], null);

        board.Add(layer2);
        
        // third layer (sentry tutorial)
        Room[] layer3 = new Room[maxWidth];

        layer3[maxWidth / 2] = CreateRoom(2);
        layer2[maxWidth / 2].SetDownRoom(null, layer3[maxWidth / 2], null);
        layer3[maxWidth / 2].SetUpRoom(null, layer2[maxWidth / 2], null);

        board.Add(layer3);

        // fourth layer (split)
        Room[] layer4 = new Room[maxWidth];

        layer4[maxWidth / 2 - 1] = CreateRoom(3);
        layer4[maxWidth / 2 + 1] = CreateRoom(3);
        layer3[maxWidth / 2].SetDownRoom(layer4[maxWidth / 2 - 1], null, layer4[maxWidth / 2 + 1]);
        layer4[maxWidth / 2 - 1].SetUpRoom(null, layer3[maxWidth / 2], null);
        layer4[maxWidth / 2 + 1].SetUpRoom(null, layer3[maxWidth / 2], null);

        board.Add(layer4);


        // generate room
        currRoom.RoomSetup();
        mainCamera.transform.position = new Vector3(currRoom.GetMidpoint().x, currRoom.GetMidpoint().y, -10f);
    }

    Room CreateRoom(int layer) {
        Room room = Instantiate(roomPrefab).GetComponent<Room>();
        room.SetLayer(layer);
        return room;
    }

    void GenerateLayer() {
        Room[] lastRow = board[board.Count - 1];

        // TODO
    }

    public void ChangeRoom() {
        if (!changing) {
            changing = true;
            StartCoroutine("CameraPan");
        }
    }

    IEnumerator CameraPan() {
        float t = 0f;
        Time.timeScale = 0f;

        int newSection = 5 - currRoom.GetRoomSection();
        Room nextRoom = currRoom.GetNextRoom();
        nextRoom.RoomSetup();

        Vector3 start = currRoom.GetMidpoint();
        Vector3 end = nextRoom.GetMidpoint();

        while (t <= 1f) {
            if (t > 0.5f) currRoom.Delete();
            t += Time.unscaledDeltaTime / 2;
            mainCamera.transform.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0f, 1f, t)) + (new Vector3(0, 0, -10));
            yield return null;
        }

        currRoom = nextRoom;
        nextRoom.setPlayerToSection(newSection);

        Time.timeScale = 1f;
        changing = false;
    }

    // Use this for initialization
    void Start () {
        InititializeBoard();
	}
	
	// Update is called once per frame
	void Update () {
        currRoom.UpdatePlayerPosition();
	}
}
