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

    private void OnCollisionEnter2D(Collision2D collision) {

        if (!collided && (isPlayer != (collision.collider.tag == "Player"))) {
            transform.SetParent(collision.collider.gameObject.transform);
            rb.isKinematic = true;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0f;

            GetComponent<Collider2D>().isTrigger = true;

            collided = true;
        }
    }
}
