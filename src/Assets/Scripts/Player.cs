using UnityEngine;

public class Player : MonoBehaviour
{
	private const string AnimationPossitionParam = "Possition";
	private const string AnimationIsJumpingParam = "IsJumping";
	private const string AnimationIsDeadParam = "IsDead";

	public GameManager gameManager;

	public float moveLeftRightSpeed = 5.0f;
	public float jumpForce = 1.0f;
	public float rotationTime = 0.3f;	
	
	private float _rotationStartTime = 0.0f;
	private float _rotationAngle = 0.0f;
	private float _currentRotationProgress = 0.0f;
	private bool _isRotating = false;

	private int _scores = 0;

	#region Events

	void Update()
	{
		UpdateJumpState();

		if (!IsJumping())
		{
			MoveLeftRight();
		}

		if (ShouldJump())
		{
			Jump();
		}

		if (IsTurnTile(transform.position) && !IsRotating() && !IsJumping())
		{
			HandleRotationInput();
		}

		if (IsRotating())
		{
			Rotate();
		}

		MoveForward();

		if (IsOutOfTrack())
		{
			Die();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == Tags.Obstacle)
		{
			Die();
		}

		if (collision.collider.tag == Tags.Coin)
		{
			AddScore();
			Destroy(collision.gameObject);
		}
	}

	#endregion

	#region Public Members

	public bool IsDead { get; private set; }

	public void Die()
	{
		IsDead = true;
		GetComponent<Animator>().SetBool(AnimationIsDeadParam, true);
		GameManager.Instance.GameOver();
	}

	public void AddScore()
	{
		_scores++;
	}

	public int Scores
	{
		get { return _scores; }
	}

	#endregion

	#region Private Members

	#region Rotation

	private void Rotate()
	{
		var currentRotationAngle = (Time.deltaTime / rotationTime) * _rotationAngle;
		var timeElapsedFromRotationStart = Time.time - _rotationStartTime;
		if (timeElapsedFromRotationStart > rotationTime)
		{
			currentRotationAngle = _rotationAngle - _currentRotationProgress;
			_isRotating = false;
		}

		_currentRotationProgress += currentRotationAngle;
		transform.Rotate(Vector3.up, currentRotationAngle);
	}

	private void HandleRotationInput()
	{
		var xAxis = Input.GetAxis(Axis.Horizontal);
		if (xAxis > 0.0f)
		{
			TurnRight();
		}
		else if (xAxis < -0.0f)
		{
			TurnLeft();
		}
	}

	private bool IsRotating()
	{
		return _isRotating;
	}

	private bool IsTurnTile(Vector3 position)
	{
		return gameManager.TerrainGenerator.IsTurnTile(position);
	}

	private void TurnRight()
	{
		Turn(90.0f);
	}

	private void TurnLeft()
	{
		Turn(-90.0f);
	}

	private void Turn(float angle)
	{
		_currentRotationProgress = 0.0f;
		_rotationAngle = angle;
		_rotationStartTime = Time.time;
		_isRotating = true;
	}

	#endregion

	#region Jumping

	private void UpdateJumpState()
	{
		GetComponent<Animator>().SetBool(AnimationIsJumpingParam, IsJumping());
	}

	private void Jump()
	{
		GetComponent<Animator>().SetBool(AnimationIsJumpingParam, true);
		rigidbody.AddForce(Vector3.up * jumpForce);
	}

	private bool ShouldJump()
	{
		return !IsJumping() && Input.GetButtonDown(Button.Jump);
	}

	private bool IsJumping()
	{
		return transform.position.y > 0.1f;
	}

	#endregion

	void MoveForward()
	{
		transform.Translate(Vector3.forward * Time.deltaTime * GlobalConstants.TrackSpeed);
	}

	private void MoveLeftRight()
	{
		var xAxis = Input.GetAxis(Axis.Horizontal);
		GetComponent<Animator>().SetFloat(AnimationPossitionParam, xAxis);
		transform.Translate(Vector3.right * xAxis * Time.deltaTime * moveLeftRightSpeed);
	}

	private bool IsOutOfTrack()
	{
		var terrainGenerator = gameManager.TerrainGenerator;
		return !terrainGenerator.ContainsPoint(transform.position) && !terrainGenerator.IsZeroTile(transform.position);
	}

	#endregion
}
