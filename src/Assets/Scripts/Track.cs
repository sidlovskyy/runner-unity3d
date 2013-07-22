using UnityEngine;
using System.Collections;

public class Track : MonoBehaviour
{
	
	public GameObject obstacle;
	public GameObject coin;
	public float delayMin = 0.5f;
	public float delayMax = 1.0f;
	public float coinsRate = 1.0f;

	private float timePassed = 0.0f;
	private float currentDelay;

	void Start() 
	{
		currentDelay = GetNewRandonDelay();
	}

	void Update()
	{
		timePassed += Time.deltaTime;

		if (timePassed >= currentDelay) {
			Instantiate(obstacle);
			timePassed = 0.0f;
			currentDelay = GetNewRandonDelay();
		}
		else {
			if (Random.Range(0, 100) < coinsRate) {
				Instantiate(coin);
			}
		}
	}

	float GetNewRandonDelay()
	{
		return Random.Range(delayMin, delayMax);
	}
}
