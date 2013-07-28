using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{	
	private const string AnimationPossitionParam = "Possition";
	private const string AnimationIsJumpingParam = "IsJumping";
	private const string AnimationIsDeadParam = "IsDead";
		
	public float turnSpeed = 5.0f;
	public float jumpForce = 1.0f;

	private int scores = 0;

	public bool IsDead { get; private set; }

	#region Events

	void Update()
	{
		UpdateJumpState();

		if (!IsJumping()) {
			MoveLeftRight();
		}		
		
		if(ShouldJump()) {
			Jump();
		}		
		
		if(IsOutOfTrack()) {
			Die();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == Tags.Obstacle) {
			Die();
		}

		if (collision.collider.tag == Tags.Coin) {
			AddScore();
			Destroy(collision.gameObject);
		}
	}	

	#endregion

	void MoveLeftRight()
	{
		var xAxis = Input.GetAxis(Axis.Horizontal);
		GetComponent<Animator>().SetFloat(AnimationPossitionParam, xAxis);	
		transform.Translate(Vector3.right * xAxis * Time.deltaTime * turnSpeed);
	}
	
	void UpdateJumpState()
	{
		GetComponent<Animator>().SetBool(AnimationIsJumpingParam, IsJumping());
	}
	
	void Jump()
	{
		GetComponent<Animator>().SetBool(AnimationIsJumpingParam, true);
		rigidbody.AddForce(Vector3.up * jumpForce);
	}

	bool ShouldJump()
	{
		return !IsJumping() && Input.GetButtonDown(Button.Jump);
	}
	
	bool IsJumping()
	{		
		return transform.position.y > 0.1f;
	}
	
	bool IsOutOfTrack()
	{
		var halfTrackWidth = GlobalConstants.TrackWidth / 2.0f;
		var isOutOfTrack = Mathf.Abs(transform.position.x) > halfTrackWidth;
		return isOutOfTrack;
	}
	
	public void Die()
	{
		IsDead = true;		
		GetComponent<Animator>().SetBool(AnimationIsDeadParam, true);
		GameManager.Instance.GameOver();
	}	

	public void AddScore()
	{
		scores++;
	}

	public int Scores
	{
		get { return scores; }
	}

	public bool CannotSee(TerrainTile terrainTile)
	{
		//TODO: implement
		return false;
	}

	public bool CanFullySee(TerrainTile terrainTile)
	{
		//TODO: implement
		return Vector3.Distance(transform.position, LocationToCoordinates(terrainTile.X, terrainTile.Y)) < 50.0f;
	}
			
	private Vector3 LocationToCoordinates(int x, int y)
	{
		return new Vector3(
			x * GlobalConstants.TerrainTileSize, 
			0, 
			y * GlobalConstants.TerrainTileSize);
	}
}
