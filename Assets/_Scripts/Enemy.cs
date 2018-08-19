using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private float passiveProgress = 0f;
    private float passiveGain = 0.01f;

    private bool loaded = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!loaded) {
            passiveProgress += passiveGain;
        }
	}
}
