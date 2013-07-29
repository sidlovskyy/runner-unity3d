using UnityEngine;

public class Toolbar : MonoBehaviour
{
	public Player Player;

	private GameManager _gameManager;

	void Awake()
	{
		_gameManager = GameManager.Instance;
		Player = _gameManager.Player;
	}

	void OnGUI()
	{
		RenderScores();

		if (_gameManager.IsGameOver)
		{
			GameOver();
		}
		else
		{
			RenderPauseResume();
		}
	}

	private void RenderScores()
	{
		var playerScores = Player.Scores;
		GUI.Label(new Rect(Screen.width - 125, 20, 100, 40), string.Format("COINS: {0}", playerScores));
	}

	private void RenderPauseResume()
	{
		if (GUI.Button(new Rect(25, 25, 100, 30), GetButtonText()))
		{
			PauseOrResumeGame();
			GUI.Button(new Rect(25, 25, 100, 30), GetButtonText());
		}
	}

	private void GameOver()
	{
		var buttonLeft = (Screen.width / 2) - 70;
		GUI.Label(new Rect(buttonLeft + 10, 25, 100, 30), "GAME OVER");

		if (GUI.Button(new Rect(buttonLeft, 75, 100, 30), "MENU"))
		{
			Application.LoadLevel(Scene.Menu);
		}
	}

	void PauseOrResumeGame()
	{
		if (_gameManager.IsPaused)
		{
			_gameManager.Resume();
		}
		else
		{
			_gameManager.Pause();
		}
	}

	string GetButtonText()
	{
		return _gameManager.IsPaused ? "RESUME" : "PAUSE";
	}
}
