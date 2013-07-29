using UnityEngine;

public class InfiniteRotation : MonoBehaviour
{
	public float rotationSpeed = 1.0f;
	public Vector3 rotationAxis;

	void Update()
	{
		transform.Rotate(rotationAxis, Time.deltaTime * rotationSpeed);
	}
}
