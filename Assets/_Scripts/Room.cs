using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    private GameManager gm;

    private bool isBase = false;

    private float tileSize = 0.64f;
    private int rows = 14;
    private int cols = 18;
    public GameObject floorTile;
    public GameObject wallTile;
    public GameObject[] upStairs;
    public GameObject[] downStairs;

    private Vector3 midpoint;

    private int col = 0;
    private int layer = 0;
    private Room[] upRooms = new Room[3];
    private Room[] downRooms = new Room[3];

    private GameObject[][] grid;

    private PlayerController player;
    private int roomSection = 0;

    private Minimap minimap;

    public GameObject[] enemyPrefabs;
    private List<Enemy> enemies = new List<Enemy>();
    private int enemyCooldown = 0;
    private int enemyCooldownSet = 500;

    public GameObject sentry;
    private List<SentryController> sentries = new List<SentryController>();

    private bool loaded = false;

    public AudioClip sentryBuildSound;

    // PUBLIC INTERFACE -----------------------------------------------------------------------------------------------------

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

        loaded = true;
        midpoint = (grid[0][0].transform.position + grid[rows - 1][cols - 1].transform.position) / 2;

        foreach (Enemy e in enemies) {
            Vector3 lerp = Vector3.Lerp(GetSectionCoord(e.GetPathFrom(), -3f), GetSectionCoord(e.GetPathTo(), -3f), Mathf.Clamp(e.GetProgress(), 0.2f, 0.75f));
            e.MoveTo(lerp);
            e.Show();
        }

        foreach (SentryController s in sentries) {
            s.Show();
        }

    }

    public Vector3 GetMidpoint() {
        return midpoint;
    }

    public Room GetUpRoom(int section) {
        return upRooms[section];
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

    public Vector3 GetSectionCoord(int section, float z) {
        
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

        return new Vector3(x, y, z);
    }

    public int GetLayer() {
        return layer;
    }

    public int GetCol() {
        return col;
    }

    public void SetBase() {
        isBase = true;
    }

    public void SetLayer(int layer, int col) {
        this.layer = layer;
        this.col = col;
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
            if (child.gameObject.CompareTag("Tile") || child.gameObject.CompareTag("DownStairs") || child.gameObject.CompareTag("UpStairs")) {
                Destroy(child.gameObject);
            }
        }

        foreach (Enemy e in enemies) {
            e.Hide();
        }

        foreach (SentryController s in sentries) {
            s.Hide();
        }

        loaded = false;
    }

    // PRIVATE PARTS --------------------------------------------------------------------------------------------------------------------------

    // Use this for initialization
    void Start() {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        player = gm.GetPlayer(); ;
        minimap = gm.GetMinimap(); ;
    }

    // Update is called once per frame
    void Update() {
        enemyCooldown = enemyCooldown == 0 ? 0 : enemyCooldown - 1;
        
        if (!loaded) {
            List<Enemy> temp = new List<Enemy>(enemies);
            for (int i = 0; i < temp.Count; i++) {
                if (temp[i].Progress()) {
                    if (isBase) {
                        enemies.Remove(temp[i]);
                        GameObject.FindWithTag("GameController").GetComponent<GameManager>().DamageBase(1);
                    } else {
                        Room destRoom = upRooms[temp[i].GetPathTo()];

                        if (destRoom.isBase) {
                            enemies.Remove(temp[i]);
                            temp[i].SetPath(5 - temp[i].GetPathTo(), 1);
                            destRoom.AddEnemy(temp[i]);
                            temp[i].ResetProgress();
                        } else {
                            int tries = 1000;
                            while (true && tries-- > 0) {
                                int r = Random.Range(0, 3);
                                if (destRoom.GetUpRoom(r)) {
                                    enemies.Remove(temp[i]);
                                    temp[i].SetPath(5 - temp[i].GetPathTo(), r);
                                    destRoom.AddEnemy(temp[i]);
                                    temp[i].ResetProgress();
                                    break;
                                }
                            }

                            if (tries < 0) {
                                Debug.Log("failure move");
                            }
                        }
                    }
                }
            }

            //shot
            foreach (SentryController s in sentries) {
                if (enemies.Count > 0) {
                    if (s.ShootAt(Vector3.zero)) {
                        enemies[0].Damage(5);
                        if (!enemies[0].IsAlive()) {
                            player.GetComponent<PlayerController>().ChangeAmmo(enemies[0].GetPrize());
                            enemies.RemoveAt(0);
                        }
                    }
                }
            }
        } else {
            if (Input.GetKeyDown(KeyCode.E) && player.GetComponent<PlayerController>().GetAmmo() >= SentryController.GetPrice()) {
                GetComponent<AudioSource>().PlayOneShot(sentryBuildSound, 1f);
                player.GetComponent<PlayerController>().ChangeAmmo(-SentryController.GetPrice());
                sentries.Add(Instantiate(sentry, player.transform.position, Quaternion.identity).GetComponent<SentryController>());
                minimap.SetSentry(layer, col);
            }

            Enemy target = null;
            for (int i = 0; i < enemies.Count; i++) {
                if (enemies[i].IsCollected()) {
                    Destroy(enemies[i].gameObject);
                    player.GetComponent<PlayerController>().ChangeAmmo(enemies[i].GetPrize());
                    enemies.RemoveAt(i--);
                } else {
                    if (enemies[i].IsAlive() && !target) {
                        target = enemies[i];
                    }
                    enemies[i].Move();
                }
            }
            foreach (SentryController s in sentries) {
                if (target) {
                    s.ShootAt(target.transform.position);
                }
            }
        }

        minimap.SetEnemy(layer, col, enemies.Count != 0);
    }

    public void AdvanceEnemy(Enemy e) {
        Room destRoom = upRooms[e.GetPathTo()];

        int tries = 1000;
        while (true && tries-- > 0) {
            int r = Random.Range(0, 3);
            if (destRoom.GetUpRoom(r)) {
                enemies.Remove(e);
                e.SetPath(5 - e.GetPathTo(), r);
                destRoom.AddEnemy(e);
                e.ResetProgress();

                break;
            }
        }

        if (tries < 0) {
            Debug.Log("failure move");
        }
    }

    public void AddEnemy(Enemy e) {
        e.gameObject.transform.SetParent(transform);
        enemies.Add(e);

        if (loaded) {
            e.MoveTo(GetSectionCoord(e.GetPathFrom(), -3f));
            e.Show();
        }
    }

    public void setPlayerToSection(int section) {
        player.transform.position = GetSectionCoord(section, -5f);
    }

    public void UpdatePlayerPosition() {
        Vector3 playerPos = player.GetPosition();

        roomSection = ((playerPos.y > midpoint.y) ? 0 : 3) + ((Mathf.Abs((float)(playerPos.x - midpoint.x)) < tileSize * 3) ? 1 : (playerPos.x < midpoint.x) ? 0 : 2);
    }

    public void SpawnEnemy(float chance) {
        if (enemyCooldown <= 0) {
            if (Random.value < chance) {
                enemyCooldown = enemyCooldownSet;
                GameObject enemy = Instantiate(enemyPrefabs[0]);
                enemy.transform.SetParent(transform);
                Enemy e = enemy.GetComponent<Enemy>();
                enemies.Add(e);

                int tries = 100;
                while (true && tries-- > 0) {
                    int r = Random.Range(0, 3);
                    if (GetUpRoom(r)) {
                        e.SetPath(4, r);
                        e.ResetProgress();
                        break;
                    }
                }

                e.SetHealth(layer);
            }
        }
    }
}
