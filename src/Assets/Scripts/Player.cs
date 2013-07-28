using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	private const string AnimationPossitionParam = "Possition";
	private const string AnimationIsJumpingParam = "IsJumping";
	private const string AnimationIsDeadParam = "IsDead";
	
	public GameManager gameManager;
	
	public float turnSpeed = 5.0f;
	public float jumpForce = 1.0f;

	private int scores = 0;

	public float rotationTime = 0.1f;
	
	private float rotationStartTime = 0.0f;
	private float rotationAngle = 0.0f;
	private float rotationProgress = 0.0f;
	private bool isRotating = false;

		
	#region Events

	void Update()
	{
		MoveForward();
		
		UpdateJumpState();

		if (!IsJumping()) {
			MoveLeftRight();
		}
		
		if(ShouldJump()) {
			Jump();
		}
		
		if(IsRotating())
		{
			Rotate();
		}
		
		if(IsTurnTile(transform.position) && !IsRotating() && !IsJumping())
		{
			HandleRotationInput ();
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
	
	public bool IsDead { get; private set; }
	
	bool IsRotating() 
	{
		return isRotating;
	}

	void MoveLeftRight()
	{
		var xAxis = Input.GetAxis(Axis.Horizontal);
		GetComponent<Animator>().SetFloat(AnimationPossitionParam, xAxis);	
		transform.Translate(Vector3.right * xAxis * Time.deltaTime * turnSpeed);
	}
	
	bool IsTurnTile(Vector3 possition)
	{
		return gameManager.TerrainGenerator.IsTurnTile(possition);
	}
	
	void TurnRight()
	{
		rotationProgress = 0.0f;
		rotationAngle = 90.0f;
		rotationStartTime = Time.time;
		isRotating = true;
	}
	
	void TurnLeft()
	{
		rotationProgress = 0.0f;
		rotationAngle = -90.0f;
		rotationStartTime = Time.time;
		isRotating = true;
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
		return gameManager.TerrainGenerator.ContainsPoint(transform.position);
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
		//TODO: this is not the best implementation
		return Vector3.Distance(transform.position, LocationToCoordinates(terrainTile.X, terrainTile.Y)) >= 60.0f;
	}

	public bool CanFullySee(TerrainTile terrainTile)
	{
		//TODO: this is not the best implementation
		return Vector3.Distance(transform.position, LocationToCoordinates(terrainTile.X, terrainTile.Y)) < 50.0f;
	}
	
	void MoveForward()
	{
		transform.Translate(Vector3.forward * Time.deltaTime * GlobalConstants.TrackSpeed);
	}

	void Rotate ()
	{
		var rotationDelta = (Time.deltaTime / rotationTime) * rotationAngle;
		if(Time.time - rotationStartTime > rotationTime)
		{
			rotationDelta = rotationAngle - rotationProgress;
			isRotating = false;
		}
		
		rotationProgress += rotationDelta;
		transform.Rotate(Vector3.up, rotationDelta);
	}

	void HandleRotationInput()
	{
		var xAxis = Input.GetAxis(Axis.Horizontal);		
		if(xAxis > 0.0f)
		{
			TurnRight();
		}
		else if(xAxis < -0.0f)			
		{
			TurnLeft();
		}
	}
	
	private Vector3 LocationToCoordinates(int x, int y)
	{
		return new Vector3(
			x * GlobalConstants.TerrainTileSize, 
			0, 
			y * GlobalConstants.TerrainTileSize);
	}
}
