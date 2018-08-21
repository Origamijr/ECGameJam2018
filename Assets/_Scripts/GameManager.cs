using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject mainCamera;
    public GameObject minimapCamera;

    public Text uiText;
    public GameObject gameOverScreen;

    private BoardManager boardManager;
    private PlayerController player;
    private Minimap minimap;

    private int baseHealth = 10;

    private bool paused = false;

    public AudioClip baseDamageSound;
    
    // PUBLIC INTERFACE ----------------------------------------------------------------------------------------------------------------------

    public void DamageBase(int damage) {
        GetComponent<AudioSource>().PlayOneShot(baseDamageSound, 1f);
        baseHealth -= damage;
    }

    public PlayerController GetPlayer() {
        return player;
    }

    public BoardManager GetBoardManager() {
        return boardManager;
    }

    public GameObject GetMainCamera() {
        return mainCamera;
    }

    public Minimap GetMinimap() {
        return minimap;
    }

    public GameObject GetMinimapCamera() {
        return minimapCamera;
    }

    public bool IsPaused() {
        return paused;
    }

    public void Pause(bool paused) {
        this.paused = paused;
    }

    // PRIVATE PARTS -------------------------------------------------------------------------------------------------------------------------

	// Use this for initialization
	void Awake () {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        boardManager = gameObject.GetComponent<BoardManager>();
        minimap = GameObject.FindWithTag("Minimap").GetComponent<Minimap>();
	}
	
	// Update is called once per frame
	void Update () {

        int playerHealth = player.GetHealth();
        int ammo = player.GetAmmo();
        int currLayer = boardManager.GetCurrLayer();
        int lowest = boardManager.GetLowestLayer();

        uiText.text = "Base Health: " + baseHealth + "\nPlayer Health: " + playerHealth + "\nAmmo: " + ammo + "\nSentry Price: " + SentryController.GetPrice() + "\nCurrent Layer: " + currLayer + "\nLowest Layer: " + lowest;

        if (baseHealth <= 0 || playerHealth <= 0) {
            GameOver();
        }
	}

    void GameOver() {
        gameOverScreen.SetActive(true);
        StartCoroutine("Restart");
    }

    IEnumerator Restart() {
        Time.timeScale = 0f;
        while (!Input.GetKeyDown(KeyCode.R)) { yield return null; }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }
}
