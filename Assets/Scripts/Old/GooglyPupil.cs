using UnityEngine;
using System.Collections;

public class GooglyPupil : MonoBehaviour {
    public float eyeRadius = 1.0f;
    public float eyeMass = 1.0f;
    public float damping = 1.0f;
    public float restitution = 0.66f;
    public Vector2 gravity = new Vector2(0, -9.8f);
    Vector3 restPosition;
    Vector3 lastPosition;
    Vector3 lastVelocity;

    public Vector3 velocity;
    public Vector3 acceleration;

    void Start()
    {
        restPosition = transform.localPosition;
        lastPosition = transform.parent.position;
        lastVelocity = Vector3.zero;
    }
	
    void FixedUpdate()
    {
        // Calculate derivatives
        Vector3 newVelocity = (transform.parent.position - lastPosition) / Time.fixedDeltaTime;
        acceleration = (newVelocity - lastVelocity) / Time.fixedDeltaTime;

        // Update stored values
        lastVelocity = newVelocity;
        lastPosition = transform.parent.position;

        // Apply acceleration
        velocity += (-acceleration / eyeMass - velocity * damping + (Vector3)gravity) * Time.fixedDeltaTime;
        velocity = new Vector3(velocity.x, velocity.y, 0);
        transform.position += velocity * Time.fixedDeltaTime;
        float dist = ((Vector2)(transform.localPosition - restPosition)).magnitude;
        if (dist > eyeRadius)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x * eyeRadius / dist,
                transform.localPosition.y * eyeRadius / dist,
                transform.localPosition.z
                );
            velocity = -velocity * restitution;
        }
    }
}
