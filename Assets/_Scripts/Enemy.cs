using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private GameManager gm;

    public GameObject sprite;

    private PlayerController player;

    private Rigidbody2D rb;

    private float passiveProgress = 0f;
    private float passiveGain = 0.005f;

    private int fromSection;
    private int toSection;

    private float speed = 1f;
    private Vector3 movement;

    private bool dead = false;

    private bool collected = false;

    private int damage = 1;
    private int damageCooldown = 0;
    private int damageCooldownSet = 10;

    private int health = 10;
    private int prize = 1;

    public bool tutorial = false;

    // PUBLIC INTERFACE ------------------------------------------------------------------------------------------------------------- 

    public void Hide() {
        sprite.SetActive(false);
    }

    public void Show() {
        sprite.SetActive(true);
    }

    public bool IsCollected() {
        return collected;
    }

    public bool Progress() {
        return (passiveProgress += passiveGain) > 1f;
    }

    public void ResetProgress() {
        passiveProgress = 0f;
    }

    public bool IsAlive() {
        return !dead;
    }

    public int GetPrize() {
        return prize;
    }

    public void SetHealth(int health) {
        this.health = health;
        this.prize = 1 + health / 10;
    }

    public void Damage(int damage) {
        health -= damage;

        if (health <= 0) {
            dead = true;
        }
    }

    public float GetProgress() {
        return passiveProgress;
    }

    public int GetPathFrom() {
        return fromSection;
    }

    public int GetPathTo() {
        return toSection;
    }

    public void SetPath(int from, int to) {
        fromSection = from;
        toSection = to;
    }

    // PRIVATE PARTS -----------------------------------------------------------------------------------------------------------------

	// Use this for initialization
	void Start () {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        player = gm.GetPlayer();
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        damageCooldown--;
	}

    public void Move() {
        if (!dead) {
            Vector3 toPlayer = player.GetPosition() - transform.position;
            movement = (5 * movement + toPlayer).normalized;
            rb.velocity = speed * movement;
        } else {
            rb.velocity = Vector3.zero;
        }
    }

    public void MoveTo(Vector3 dest) {
        transform.position = dest;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Projectile") && !dead) {
            if (collision.gameObject.GetComponent<ProjectileHandler>().CollideWith(gameObject)) {
                Damage(10);
                if (dead) {
                    transform.rotation = Quaternion.Euler(0f, 0f, (collision.gameObject.transform.position.x > transform.position.x) ? 90f : -90f);
                }
            }
        }

        if (collision.gameObject.CompareTag("Player")) {
            if (dead) {
                collected = true;
                if (tutorial) {
                    player.ChangeAmmo(GetPrize());
                    Destroy(gameObject);
                }
            } else {
                if (damageCooldown <= 0) {
                    damageCooldown = damageCooldownSet;
                    player.Damage(damage);
                    player.Flinch(player.GetPosition() - transform.position);
                }
            }
        }
    }
}
