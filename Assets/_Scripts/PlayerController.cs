using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject gameManager;

    public GameObject sprite;
    
    public float speed = 1f;
    public float dash = 1.5f;
    private Vector3 movement;

    private bool rolling;
    private float rollAdd = 6f;
    private float rot = 0;
    private float rotSpeed = 20f;

    public GameObject chargeBar;
    public Sprite[] chargeBarStates = new Sprite[6];
    private float charge = 0f;
    public float chargeRate = 5f;
    private bool charging = false;

    private Rigidbody2D rb;

    public GameObject projectile;
    public float projectileSpeed = 10;

    private int health = 10;
    private int ammo = 20;

    public void Damage(int damage) {
        health -= damage;
    }

    public void ChangeAmmo(int diff) {
        ammo += diff;
    }

    public int GetHealth() {
        return health;
    }

    public int GetAmmo() {
        return ammo;
    }

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.identity;

        if (Input.GetKeyDown(KeyCode.LeftShift) && !rolling) {
            rolling = true;
            speed += rollAdd;
        }

        if (rolling) {
            rot += rotSpeed;
            if (rot >= 360f) {
                rot = 0;
                rolling = false;
                speed -= rollAdd;
                sprite.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            } else {
                sprite.transform.rotation = Quaternion.Euler(0f, 0f, -Mathf.Sign(rb.velocity.x) * rot);
            }
        }

        Move(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetMouseButtonDown(0) && ammo > 0) {
            charging = true;
            chargeBar.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0)) {
            charging = false;
            chargeBar.SetActive(false);

            if (charge >= 100f) {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector3 direction = (mousePos - transform.position).normalized;

                GameObject instance = Instantiate(projectile, transform.position + direction, Quaternion.identity);

                instance.GetComponent<ProjectileHandler>().SetIsPlayer(true);

                float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                instance.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
                instance.GetComponent<Rigidbody2D>().velocity = projectileSpeed * instance.transform.right;
                ammo--;
            }

            charge = 0;
        }

        ChargeBar();
	}

    void Move(float h, float v) {
        if (!rolling) {
            if (h != 0f || v != 0f) {
                movement = (2 * movement + h * transform.right + v * transform.up).normalized;
                rb.velocity = speed * movement;
            } else {
                rb.velocity = rb.velocity / 2;
                movement = movement / 2;
            }
        } else {
            rb.velocity = speed * movement;
        }
    }

    void ChargeBar() {
        if (charging) {
            charge += chargeRate;

            if (charge >= 110) {
                chargeBar.GetComponent<SpriteRenderer>().sprite = chargeBarStates[5];
            } else if (charge >= 100) {
                chargeBar.GetComponent<SpriteRenderer>().sprite = chargeBarStates[4];
            } else if (charge >= 75) {
                chargeBar.GetComponent<SpriteRenderer>().sprite = chargeBarStates[3];
            } else if (charge >= 50) {
                chargeBar.GetComponent<SpriteRenderer>().sprite = chargeBarStates[2];
            } else if (charge >= 25) {
                chargeBar.GetComponent<SpriteRenderer>().sprite = chargeBarStates[1];
            } else {
                chargeBar.GetComponent<SpriteRenderer>().sprite = chargeBarStates[0];
            }
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.tag == "UpStairs" || collision.collider.tag == "DownStairs") {
            gameManager.GetComponent<BoardManager>().ChangeRoom();
        }
    }
}
