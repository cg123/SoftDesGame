using UnityEngine;
using System.Collections;

public class PlayerHealthController : MonoBehaviour
{
    public int health = 50;

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
