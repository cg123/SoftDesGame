using UnityEngine;
using System.Collections;

public class TelescopeExtension : MonoBehaviour {
    public float extensionFraction = 0;
    public float wobbleAmount = 0;
    public float wobbleDamping = 5;
    public Transform childOne, childTwo;

    void Drift(Transform child)
    {
        Quaternion drift = Quaternion.Euler(0, 0, wobbleAmount * (Random.value * 2 - 1) * Time.deltaTime);
        Quaternion damping = Quaternion.Inverse(child.localRotation);
        float dampingAngle;
        Vector3 dampingAxis;
        damping.ToAngleAxis(out dampingAngle, out dampingAxis);
        Quaternion scaledDamping = Quaternion.AngleAxis(dampingAngle * Time.deltaTime * wobbleDamping, dampingAxis);
        child.localRotation = scaledDamping * drift * child.localRotation;
    }

	// Update is called once per frame
    void Update()
    {
        float distance = Mathf.Min(1, Mathf.Max(0.25f, extensionFraction + 0.25f));
        childOne.localPosition = distance * 0.17f * Vector3.right - Vector3.forward * 0.1f;
        childTwo.localPosition = distance * 0.17f * Vector3.right - Vector3.forward * 0.2f;
        Drift(childOne);
        Drift(childTwo);
	}
}
