using UnityEngine;
using System.Collections;

public class PlayerHealthController : MonoBehaviour
{
    public int health = 50;
    public GameObject explosionTemplate;


    void Update()
    {
        if (health < 1)
        {
            Destroy(GetComponent("PlayerController"));
            GetComponent<Rigidbody2D>().isKinematic = true;
            foreach (SpriteRenderer sr in gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }
            foreach (PolygonCollider2D pc in gameObject.GetComponentsInChildren<PolygonCollider2D>())
            {
                pc.enabled = false;
            }
            Instantiate(explosionTemplate, transform.position - Vector3.forward * 10, Quaternion.identity);
            enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Lightning")
        {
            rigidbody2D.AddForce(Random.onUnitSphere * 250);
            health -= 1;
            Destroy(col.gameObject);
        }
    }
}
