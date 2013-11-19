using UnityEngine;
using System.Collections;

public class Arm : MonoBehaviour
{
    public Transform child;
    Transform lastChild;
    public Vector3 attachPoint;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.TransformPoint(attachPoint), 0.05f);
    }

    void Awake()
    {
        lastChild = null;
    }

    void Update()
    {
        if (child != lastChild && child != null)
        {
            child.parent = transform;
            child.localPosition = attachPoint;

            lastChild = child;
        }
    }
}
