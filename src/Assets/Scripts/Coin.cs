using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour 
{
	//TODO: Ugly solution. destroy coin if it's generated inside obstacle
	void OnCollisionStay(Collision collision)
	{
		if (collision.collider.tag == Tags.Obstacle) {
			Destroy(gameObject);
		}
	}	
}
