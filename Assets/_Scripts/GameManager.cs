using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject camera;

    public Text uiText;
    public GameObject gameOverScreen;

    private BoardManager boardManager;
    private PlayerController player;

    private int baseHealth = 10;

    public void DamageBase(int damage) {
        baseHealth -= damage;
    }

	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        boardManager = gameObject.GetComponent<BoardManager>();
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
