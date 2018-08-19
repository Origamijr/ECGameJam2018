using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public GameObject roomPrefab;

    private Room upRoom = null;
    private Room currRoom = null;
    private Room downRoom = null;

    // Use this for initialization
    void Start () {
        currRoom = Instantiate(roomPrefab).GetComponent<Room>();
        currRoom.RoomSetup();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
