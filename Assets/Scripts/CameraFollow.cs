using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public Transform following;

    void Update()
    {
        if (following == null) return;
        Vector3 newPos = new Vector3(following.position.x, following.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);
    }
}
