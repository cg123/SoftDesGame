using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Pickup : MonoBehaviour {
    public GameObject explosionTemplate;
    public UserInterface ui;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Player")
        {
            Instantiate(explosionTemplate, transform.position, Random.rotation);
            col.rigidbody.AddForceAtPosition((col.transform.position - transform.position).normalized * 1000, col.contacts[0].point);
            col.gameObject.GetComponent<PlayerHealthController>().health += 4;
            ui.baconCount--;
            Destroy(gameObject);
        }
    }
}
