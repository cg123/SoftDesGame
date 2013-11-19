using UnityEngine;
using System.Collections;

public class Head : MonoBehaviour
{
    public Vector3[] armAttachPoints;
    public Arm[] arms;

    void Update()
    {
        int i;
        for (i = 0; i < arms.Length; i++)
        {
            arms[i].transform.parent = transform;
            arms[i].transform.localPosition = armAttachPoints[i];
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        int i;
        for (i = 0; i < armAttachPoints.Length; i++)
        {
            Gizmos.DrawSphere(transform.TransformPoint(armAttachPoints[i]), 0.05f);
        }
    }
}
