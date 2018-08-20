using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour {

    private Rigidbody2D rb;

    private bool collided = false;

    private bool isPlayer;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetIsPlayer(bool isPlayer) {
        this.isPlayer = isPlayer;
    }

    public bool CollideWith(GameObject other) {
        if (!isPlayer) {
            Destroy(gameObject);
            return true;
        }
        if (!collided) {
            transform.SetParent(other.transform);
            rb.isKinematic = true;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;

            GetComponent<Collider2D>().isTrigger = true;

            collided = true;

            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if ((isPlayer != (collision.collider.tag == "Player"))) {
            CollideWith(collision.collider.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            collision.gameObject.GetComponent<PlayerController>().ChangeAmmo(1);
            Destroy(gameObject);
        }
    }
}
