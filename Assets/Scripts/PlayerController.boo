import UnityEngine

[RequireComponent(typeof(Rigidbody2D))]
class PlayerController(MonoBehaviour):
	public acceleration as single = 15.0f;
	public damping as single = 3.0f;
	public angularDamping as single = 3.0f;
	public turnConstant as single = 0.1f;
	public lastHeading as single = 0;
	public angleOffset as single = 0;

	def Update() as void:
		x as single = Input.GetAxis("Horizontal")
		y as single = Input.GetAxis("Vertical")
		if x < -0.05:
			transform.localScale = Vector3(-1.0f, 1.0f, 1.0f)
		elif x > 0.05:
			transform.localScale = Vector3(1.0f, 1.0f, 1.0f)
		if x*x + y*y >= 0.1:
			lastHeading = Mathf.Atan2(y, x) * 180 / Mathf.PI;
		if lastHeading < 0:
			lastHeading += 360;
		rigidbody2D.AddForce(Vector2(x, y) * acceleration - rigidbody2D.velocity * damping);
		angleOffset = -transform.localEulerAngles.z
		angleOffset = ((angleOffset + 180) % 360) - 180;
		rigidbody2D.AddTorque(angleOffset * turnConstant - rigidbody2D.angularVelocity * angularDamping);
