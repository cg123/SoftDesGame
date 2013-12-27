using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public int health = 10;
    public GameObject explosionTemplate;

    public float acceleration = 15.0f;
    public float damping = 1.0f;
    public float angularSpringConstant = 0.01f;
    public float angularDamping = 0.01f;

    float controlX, controlY;

    void Update()
    {
        // Are we dead?
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
            return;
        }

        // No. Read player controls.
        controlX = Input.GetAxis("Horizontal");
        controlY = Input.GetAxis("Vertical");
    }


    void FixedUpdate()
    {
        if (controlX < -0.01)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else if (controlY > 0.05)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        rigidbody2D.AddForce(new Vector2(controlX, controlY) * acceleration - rigidbody2D.velocity * damping);

        float angleOffset = -transform.localEulerAngles.z;
        angleOffset = ((angleOffset + 180) % 360) - 180;
        rigidbody2D.AddTorque(angleOffset * angularSpringConstant - rigidbody2D.angularVelocity * angularDamping);
    }
}
