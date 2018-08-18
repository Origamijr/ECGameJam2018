using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public float speed = 1f;
    public float dash = 1.5f;

    public GameObject chargeBar;
    public Sprite[] chargeBarStates = new Sprite[6];
    private float charge = 0f;
    public float chargeRate = 1f;
    private bool charging = false;

    private Rigidbody2D rb;

    public GameObject projectile;
    public float projectileSpeed = 10;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rb.velocity = speed * (new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f)).normalized;

        if (Input.GetMouseButtonDown(0)) {
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

                float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                instance.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
                instance.GetComponent<Rigidbody2D>().velocity = projectileSpeed * (mousePos - transform.position).normalized;
            }

            charge = 0;
        }

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
}
