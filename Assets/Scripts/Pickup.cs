using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Pickup : MonoBehaviour {
    public GameObject explosionTemplate;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Player")
        {
            Instantiate(explosionTemplate, transform.position, Random.rotation);
            col.rigidbody.AddForceAtPosition((col.transform.position - transform.position).normalized * 1000, col.contacts[0].point);
            Destroy(gameObject);
        }
    }
}
