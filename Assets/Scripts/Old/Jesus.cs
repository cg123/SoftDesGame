using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider2D))]
public class Jesus : MonoBehaviour
{
    public float lastFire;
    public GameObject bulletTemplate;
    public GameObject explosionTemplate;
    public AudioClip[] laserSounds;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<AudioSource>().Stop();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Instantiate(explosionTemplate, transform.position - Vector3.forward * 10, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        Vector3 offset = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
        Vector2 offset2D = (Vector2)offset;
        RaycastHit2D res = Physics2D.Raycast(transform.position, offset2D);
        if (res.collider.tag == "Player")
        {
            if (Time.time > lastFire + 0.6)
            {
                Quaternion newRot = Quaternion.Euler(0, 0, Mathf.Atan2(offset2D.y, offset2D.x) * 180 / Mathf.PI);
                GameObject b = Instantiate(bulletTemplate, transform.position - Vector3.forward, newRot) as GameObject;
                b.rigidbody2D.velocity = offset2D.normalized * 3;
                lastFire = Time.time;
                GetComponent<AudioSource>().clip = laserSounds[Random.Range(0, laserSounds.Length - 1)];
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
