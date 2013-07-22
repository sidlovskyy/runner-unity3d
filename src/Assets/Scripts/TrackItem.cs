using UnityEngine;
using System.Collections;

public class TrackItem : MonoBehaviour
{

	public float appearDistance = 100.0f;

	void Start()
	{
		SetupInitPosition();
	}
	
	void Update()
	{		
		Move();
		
		if(IsOutOfTrack()) {
			Destroy();
		}
	}

	void Move()
	{
		transform.Translate(Vector3.back * Time.deltaTime * GlobalConstants.TrackSpeed);
	}

	bool IsOutOfTrack()
	{
		return transform.position.z < GlobalConstants.StartOfTrack;
	}

	void Destroy()
	{
		Destroy(gameObject);
	}
	
	void SetupInitPosition()
	{
		var currentItemWith = collider.bounds.size.x;		
		var currentItemHalfWith = currentItemWith / 2.0f;
		var halfTrackWith = GlobalConstants.TrackWidth / 2.0f;
		var itemRandomRange = halfTrackWith - currentItemHalfWith;
		var xAxisRandonPossition = Random.Range(-itemRandomRange, itemRandomRange);
		transform.Translate(xAxisRandonPossition, 0, 0);

		transform.Translate(0, 0, appearDistance);		
	}
}
