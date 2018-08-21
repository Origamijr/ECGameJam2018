using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryController : MonoBehaviour {

    public GameObject sprite;

    public GameObject projectile;
    public float projectileSpeed = 10f;

    private int cooldown = 0;
    private int cooldownSet = 50;

    private static int price = 10;

    public AudioClip shootSound;

    public static int GetPrice() {
        return price;
    }

    public void Hide() {
        sprite.SetActive(false);
    }

    public void Show() {
        sprite.SetActive(true);
    }

    // Use this for initialization
    void Start () {
        price += 1;
	}
	
	// Update is called once per frame
	void Update () {
        cooldown--;
	}

    public bool ShootAt(Vector3 target) {
        if (target.x != 0 && target.y != 0 && cooldown <= 0) {
            cooldown = cooldownSet;

            Vector3 direction = (target - transform.position).normalized;

            GameObject instance = Instantiate(projectile, transform.position + direction, Quaternion.identity);

            instance.GetComponent<ProjectileHandler>().SetIsPlayer(false);

            float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            instance.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
            instance.GetComponent<Rigidbody2D>().velocity = projectileSpeed * instance.transform.right;
            GetComponent<AudioSource>().PlayOneShot(shootSound, 0.3f);
            return true;
        } else if (cooldown <= 0) {
            cooldown = cooldownSet;
            return true;
        }

        return false;
    }
}
