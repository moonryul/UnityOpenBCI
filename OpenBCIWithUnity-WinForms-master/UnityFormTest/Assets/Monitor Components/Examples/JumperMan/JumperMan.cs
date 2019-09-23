using UnityEngine;
using System.Collections;

public class JumperMan : MonoBehaviour 
{
	private float horizontalInput;
	private bool isGrounded;
	private new Rigidbody rigidbody;

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
		gameObject.layer = 10;
	}

	void Update() 
	{
		horizontalInput = Input.GetAxis("Horizontal");
		isGrounded = Physics.CheckSphere(transform.position, 0.8f, 1);

		if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
		{
			Debug.Log("Jump");

			// apply jump velocity.
			Vector3 velocity = rigidbody.velocity;
			velocity.y = 10f;
			rigidbody.velocity = velocity;
		}

		rigidbody.AddForce(10f * horizontalInput * Vector3.right * Time.deltaTime, ForceMode.VelocityChange);
	}
}
