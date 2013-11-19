using UnityEngine;
using System.Collections;

public class Claw : MonoBehaviour {
    public float closedFraction = 0.0f;
    public Transform leftJaw, rightJaw;
    void Update()
    {
        float dt = Mathf.Max(0, Mathf.Min(1, closedFraction));
        leftJaw.transform.localRotation = Quaternion.Euler(0, 0, -dt * 45.0f);
        rightJaw.transform.localRotation = Quaternion.Euler(0, 0, dt * 45.0f);
    }
}
