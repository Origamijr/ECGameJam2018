using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    public GameObject boardManager;

    public GameObject minimapCamera;

    private int maxWidth;

    private List<GameObject[]> map;

    public GameObject blank;
    public GameObject room;
    public GameObject[] paths;
    public GameObject enemyPrefab;
    public GameObject sentry;
    public GameObject curr;

    private GameObject currObj;

    private List<GameObject[]> enemies;

    private GameObject GetPath(int top, int bot) {
        return paths[top * 8 + bot];
    }

    public void GenerateLayer(List<int> rooms, List<int> from, List<int> to) {
        float y = map[map.Count - 1][0].transform.position.y - 1;

        rooms.Sort();
        from.Sort();
        to.Sort();

        // set the tiles for the path layer
        GameObject[] pathLayer = new GameObject[maxWidth];
        for (int i = 0; i < maxWidth; i++) pathLayer[i] = blank;
        if (from.Count > 0) {

            // initialize memory booleans
            bool hasLeft = false;
            bool leftToTop = false;
            bool hasCol = false;
            bool continues = false;
            bool rightFromTop = false;
            int currInd = 1;

            // iterate through path nodes
            for (int i = 0; i < from.Count; i++) {

                // catch up to lower node of current path
                if (1 + 2 * from[i] > currInd && 1 + 2 * to[i] > currInd) {

                    // get binary ids for tile states at endpoints
                    int topBin = 0;
                    int botBin = 0;

                    if (hasLeft) {
                        if (leftToTop) {
                            topBin += 4;
                        } else {
                            botBin += 4;
                        }
                    }

                    if (hasCol) {
                        topBin += 2;
                        botBin += 2;
                    }

                    if (continues) {
                        if (rightFromTop) {
                            topBin += 1;
                        } else {
                            botBin += 1;
                        }
                    }
                    
                    // set the last visited tile via the binary infered from teh memory states
                    if (topBin != 0 || botBin != 0) pathLayer[currInd] = GetPath(topBin, botBin);

                    currInd++;

                    // catch up to current nodes
                    while (1 + 2 * from[i] > currInd && 1 + 2 * to[i] > currInd) {
                        
                        // handle loose ends of previous path
                        if (i > 0 && (1 + 2 * from[i - 1] == currInd || 1 + 2 * to[i - 1] == currInd)) {
                            pathLayer[currInd] = leftToTop ? GetPath(4, 0) : GetPath(0, 4);
                            continues = false;
                        } else if (continues) {

                            // continue if a path went to the left
                            pathLayer[currInd] = GetPath(0, 0);
                        }
                        currInd++;
                    }

                    // wrap up the catch up
                    hasLeft = continues;
                    leftToTop = !rightFromTop;
                    continues = false;
                    hasCol = false;
                }

                // set the new memory variables (if there is no catch up, this poart will execute right away)
                if (from[i] == to[i]) {
                    hasCol = true;
                } else {
                    continues = true;
                    rightFromTop = from[i] < to[i];
                }
            }

            // handle the last strech of tiles like the start of the catch up
            if (currInd <= 1 + 2 * from[from.Count - 1] || currInd <= 1 + 2 * to[to.Count - 1]) {
                int topBin = 0;
                int botBin = 0;

                if (hasLeft) {
                    if (leftToTop) {
                        topBin += 4;
                    } else {
                        botBin += 4;
                    }
                }

                if (hasCol) {
                    topBin += 2;
                    botBin += 2;
                }

                if (continues) {
                    if (rightFromTop) {
                        topBin += 1;
                    } else {
                        botBin += 1;
                    }
                }

                pathLayer[currInd++] = GetPath(topBin, botBin);

                if (continues) {
                    while (currInd < 1 + 2 * from[from.Count - 1] || currInd < 1 + 2 * to[to.Count - 1]) {
                        pathLayer[currInd++] = GetPath(0, 0);
                    }
                    pathLayer[currInd] = rightFromTop ? GetPath(0, 4) : GetPath(4, 0);
                }
            }
        }
        
        // set the tiles for the room layer
        GameObject[] roomLayer = new GameObject[maxWidth];
        for (int i = 0; i < maxWidth; i++) roomLayer[i] = blank;
        for (int i = 0; i < rooms.Count; i++) {
            roomLayer[1 + 2 * rooms[i]] = room;
        }

        // Instantiate the tiles
        for (int i = 0; i < pathLayer.Length; i++) {
            pathLayer[i] = Instantiate(pathLayer[i], new Vector3(transform.position.x + i, y, 0f), Quaternion.identity);
            pathLayer[i].transform.SetParent(transform);
        }
        for (int i = 0; i < pathLayer.Length; i++) {
            roomLayer[i] = Instantiate(roomLayer[i], new Vector3(transform.position.x + i, y - 1, 0f), Quaternion.identity);
            roomLayer[i].transform.SetParent(transform);
        }

        // push new variables to map
        map.Add(pathLayer);
        map.Add(roomLayer);

        // initialize enemy list for row
        enemies.Add(new GameObject[boardManager.GetComponent<BoardManager>().maxWidth]);
    } // end GenerateLayer()



    // convert coordinates to vector3 position
    public Vector3 GetRoomCoord(int layer, int col) {
        return map[2 + 2 * layer][1 + 2 * col].transform.position;
    }


    // sets the enemy icon
    public void SetEnemy(int layer, int col, bool exists) {
        if (exists && !enemies[layer][col]) {
            enemies[layer][col] = Instantiate(enemyPrefab, GetRoomCoord(layer, col) + (new Vector3(0f, 0f, -5f)), Quaternion.identity);
            enemies[layer][col].transform.SetParent(transform);
        } else if (!exists && enemies[layer][col]) {
            Destroy(enemies[layer][col]); 
        }
    }


    // sets the sentry icon
    public void SetSentry(int layer, int col) {
        Instantiate(sentry, GetRoomCoord(layer, col) + (new Vector3(0f, 0f, -3f)), Quaternion.identity);
    }


    // sets the current tile border (will overrite the previous current tile border)
    public void SetCurr(int layer, int col) {
        Destroy(currObj);
        currObj = Instantiate(curr, GetRoomCoord(layer, col) + (new Vector3(0f, 0f, -3f)), Quaternion.identity);
    }

    // Use this for initialization
    void Start () {
        maxWidth = 1 + 2 * boardManager.GetComponent<BoardManager>().maxWidth;

        enemies = new List<GameObject[]>();

        map = new List<GameObject[]>();

        // dummy layer since generateLayer looks at the last added layer
        GameObject[] dummyLayer = new GameObject[maxWidth];
        for (int i = 0; i < maxWidth; i++) {
            dummyLayer[i] = Instantiate(blank, new Vector3(transform.position.x + i, 0f, 0f), Quaternion.identity);
            dummyLayer[i].transform.SetParent(transform);
        }
        map.Add(dummyLayer);

        // instantiate the current tile border where I know the first room will be
        currObj = Instantiate(curr, (new Vector3(transform.position.x + maxWidth / 2, -2f, -3f)), Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
		
	}


}
