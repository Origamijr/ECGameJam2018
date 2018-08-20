using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    public GameObject mainCamera;
    public GameObject minimapCamera;
    public GameObject roomPrefab;
    public GameObject minimapObj;
    private Minimap minimap;
    
    private Room currRoom = null;

    public int maxWidth = 7;

    private List<Room[]> board;

    private bool changing = false;

    public int GetCurrLayer() {
        return currRoom.GetLayer() + 1;
    }

    public int GetLowestLayer() {
        return board.Count - 3; 
    }

    // A buch of magic numbers and stuff to create the tutorial layers
    void InititializeBoard() {
        board = new List<Room[]>();

        List<int> rooms = new List<int>();
        rooms.Add(maxWidth / 2);
        List<int> from = new List<int>();
        List<int> to = new List<int>();

        // start layer (control tutorial)
        Room[] layer1 = new Room[maxWidth];

        currRoom = CreateRoom(0, maxWidth / 2);
        layer1[maxWidth / 2] = currRoom;
        currRoom.SetBase();

        minimap.GenerateLayer(rooms, from, to);

        board.Add(layer1);

        // second layer (combat tutorial)
        Room[] layer2 = new Room[maxWidth];

        layer2[maxWidth / 2] = CreateRoom(1, maxWidth / 2);
        layer1[maxWidth / 2].SetDownRoom(null, layer2[maxWidth / 2], null);
        layer2[maxWidth / 2].SetUpRoom(null, layer1[maxWidth / 2], null);

        from.Add(maxWidth / 2);
        to.Add(maxWidth / 2);
        minimap.GenerateLayer(rooms, from, to);

        board.Add(layer2);
        
        // third layer (sentry tutorial)
        Room[] layer3 = new Room[maxWidth];

        layer3[maxWidth / 2] = CreateRoom(2, maxWidth / 2);
        layer2[maxWidth / 2].SetDownRoom(null, layer3[maxWidth / 2], null);
        layer3[maxWidth / 2].SetUpRoom(null, layer2[maxWidth / 2], null);

        minimap.GenerateLayer(rooms, from, to);

        board.Add(layer3);

        // fourth layer (split)
        Room[] layer4 = new Room[maxWidth];

        layer4[maxWidth / 2 - 1] = CreateRoom(3, maxWidth / 2 - 1);
        layer4[maxWidth / 2 + 1] = CreateRoom(3, maxWidth / 2 + 1);
        layer3[maxWidth / 2].SetDownRoom(layer4[maxWidth / 2 - 1], null, layer4[maxWidth / 2 + 1]);
        layer4[maxWidth / 2 - 1].SetUpRoom(null, null, layer3[maxWidth / 2]);
        layer4[maxWidth / 2 + 1].SetUpRoom(layer3[maxWidth / 2], null, null);

        rooms.Clear();
        rooms.Add(maxWidth / 2 - 1);
        rooms.Add(maxWidth / 2 + 1);
        from.Add(maxWidth / 2);
        to.Clear();
        to.Add(maxWidth / 2 - 1);
        to.Add(maxWidth / 2 + 1);
        minimap.GenerateLayer(rooms, from, to);

        board.Add(layer4);
        
        // generate room
        currRoom.RoomSetup();
        mainCamera.transform.position = new Vector3(currRoom.GetMidpoint().x, currRoom.GetMidpoint().y, -10f);
        minimapCamera.transform.position = minimap.GetRoomCoord(0, maxWidth / 2) + (new Vector3(0f, 0f, -10f));
    } // end of InitializeBoard


    // instantiates a room (does not create path links)
    Room CreateRoom(int layer, int col) {
        Room room = Instantiate(roomPrefab).GetComponent<Room>();
        room.SetLayer(layer, col);
        return room;
    }


    // procedurally generate a layer
    void GenerateLayer() {
        Room[] lastRow = board[board.Count - 1];
        int layer = board.Count;

        List<int> rooms = new List<int>();
        List<int> from = new List<int>();
        List<int> to = new List<int>();

        List<int> gaps = new List<int>();

        // establish straights
        for (int i = 0; i < maxWidth; i++) {
            if (lastRow[i]) {
                rooms.Add(i);
                from.Add(i);
                to.Add(i);
            } else {
                gaps.Add(i);
            }
        }

        // linear chance to remove/add (independant) a room
        float t = (((float)rooms.Count - 2) / (maxWidth - 2));
        float removeChance = 0.9f * t;
        float addChance = 0.6f * (1 - t);
        
        if (Random.value < removeChance) {
            // remove a room
            int toRemove = rooms[Random.Range(0, rooms.Count - 1)];
            rooms.Remove(toRemove);

            // move left and right from the removed room to find the new upper room
            int low = toRemove, high = toRemove, link = 0;
            while (true) {
                if (low < 0) {
                    while (!lastRow[high]) { high++; }
                    link = high;
                    break;
                } else if (high >= maxWidth) {
                    while (!lastRow[low]) { low--; }
                    link = low;
                    break;
                } else {
                    if (Random.value < 0f) {
                        low--;
                        if (low >= 0 && lastRow[low]) {
                            link = low;
                            break;
                        }
                    } else {
                        high++;
                        if (high < maxWidth && lastRow[high]) {
                            link = high;
                            break;
                        }
                    }
                }
            }

            // remove old path and add new path
            from.Remove(toRemove);
            to.Remove(toRemove);
            from.Add(toRemove);
            to.Add(link);
        }
        if (Random.value < addChance) {
            // add a room in a randomly selected empty space
            int toAdd = gaps[Random.Range(0, gaps.Count - 1)];
            rooms.Add(toAdd);

            // move left and right from the selected gap to obtain the new upper room
            int low = toAdd, high = toAdd, link = 0;
            while (true) {
                if (low < 0) {
                    while (!lastRow[high]) { high++; }
                    link = high;
                    break;
                } else if (high >= maxWidth) {
                    while (!lastRow[low]) { low--; }
                    link = low;
                    break;
                } else {
                    if (Random.value < 0f) {
                        low--;
                        if (low >= 0 && lastRow[low]) {
                            link = low;
                            break;
                        }
                    } else {
                        high++;
                        if (high < maxWidth && lastRow[high]) {
                            link = high;
                            break;
                        }
                    }
                }
            }

            // create new path
            from.Add(link);
            to.Add(toAdd);
        }

        // TODO: room shifting! (to get rid of the awkward random straights

        // if four rooms form a square, there is a chance to create a diagonal path between the two
        rooms.Sort();
        for (int i = 0; i < rooms.Count - 1; i++) {
            if (rooms[i + 1] - rooms[i] == 1 && lastRow[rooms[i]] && lastRow[rooms[i + 1]] && Random.value < 0.3f) {
                if (Random.value < 0.5f) {
                    from.Add(rooms[i]);
                    to.Add(rooms[i + 1]);
                } else {
                    to.Add(rooms[i]);
                    from.Add(rooms[i + 1]);
                }
            }
        }
        
        // actually generate the rooms
        rooms.Sort();
        from.Sort();
        to.Sort();
        Room[] newRow = new Room[maxWidth];
        for (int i = 0, j = 0; i < maxWidth && j < rooms.Count; i++) {
            if (rooms[j] == i) {
                j++;
                newRow[i] = CreateRoom(layer, i);
            }
        }
        for (int i = 0; i < from.Count;) {
            Room left = null, right = null, mid = null;

            int top = from[i];

            while (i < from.Count && from[i] == top) {
                if (top > to[i]) {
                    left = newRow[to[i]];
                } else if (top == to[i]) {
                    mid = newRow[to[i]];
                } else {
                    right = newRow[to[i]];
                }
                i++;
            }

            lastRow[top].SetDownRoom(left, mid, right);
        }
        for (int i = 0; i < to.Count;) {
            Room left = null, right = null, mid = null;

            int bot = to[i];

            while (i < to.Count && to[i] == bot) {
                if (bot > from[i]) {
                    left = lastRow[from[i]];
                } else if (bot == from[i]) {
                    mid = lastRow[from[i]];
                } else {
                    right = lastRow[from[i]];
                }
                i++;
            }

            newRow[bot].SetUpRoom(left, mid, right);
        }
        board.Add(newRow);
        minimap.GenerateLayer(rooms, from, to);
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

        Vector3 mmStart = minimap.GetRoomCoord(currRoom.GetLayer(), currRoom.GetCol());
        Vector3 mmEnd = minimap.GetRoomCoord(nextRoom.GetLayer(), nextRoom.GetCol());

        // generate new layer if required
        if (board.Count < nextRoom.GetLayer() + 4) {
            GenerateLayer();
        }

        while (t <= 1f) {
            if (t > 0.5f) {
                currRoom.Delete();
                minimap.SetCurr(nextRoom.GetLayer(), nextRoom.GetCol());
            }
            t += Time.unscaledDeltaTime / 2;
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            mainCamera.transform.position = Vector3.Lerp(start, end, smooth) + (new Vector3(0, 0, -10));
            minimapCamera.transform.position = Vector3.Lerp(mmStart, mmEnd, smooth) + (new Vector3(0, 0, -10));
            yield return null;
        }

        currRoom = nextRoom;
        nextRoom.setPlayerToSection(newSection);

        Time.timeScale = 1f;
        changing = false;
    }

    // Use this for initialization
    void Start () {
        minimap = minimapObj.GetComponent<Minimap>();
        InititializeBoard();
	}
	
	// Update is called once per frame
	void Update () {
        currRoom.UpdatePlayerPosition();

        if (GetCurrLayer() >= 3) {

            for (int row = board.Count - 2; row < board.Count; row++) {
                for (int col = 0; col < maxWidth; col++) {
                    if (board[row][col]) {
                        board[row][col].SpawnEnemy(0.005f);
                    }
                }
            }
        }
	}
}
