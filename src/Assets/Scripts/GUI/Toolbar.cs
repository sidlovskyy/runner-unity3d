using UnityEngine;
using System.Collections;

public class Toolbar : MonoBehaviour
{
	public Player player;

	private GameManager gameManager;
	
	void Awake()
	{
		gameManager = GameManager.Instance;
		player = gameManager.Player;
	}
	
	void OnGUI()
	{
		RenderScores();

		if (gameManager.IsGameOver) {
			GameOver();
		}
		else {
			RenderPauseResume();
		}
	}

	private void RenderScores()
	{
		var playerScores = player.Scores;
		GUI.Label(new Rect(Screen.width - 125, 20, 100, 40), string.Format("COINS: {0}", playerScores));
	}

	private void RenderPauseResume()
	{
		if (GUI.Button(new Rect(25, 25, 100, 30), GetButtonText())) {
			PauseOrResumeGame();			
			GUI.Button(new Rect(25, 25, 100, 30), GetButtonText());
		}
	}

	private void GameOver()
	{
		var buttonLeft = (Screen.width/2) - 70;		
		GUI.Label(new Rect(buttonLeft + 10, 25, 100, 30), "GAME OVER");

		if (GUI.Button(new Rect(buttonLeft, 75, 100, 30), "MENU")) {
			Application.LoadLevel(Scene.Menu);
		}
	}

	void PauseOrResumeGame()
	{
		if(gameManager.IsPaused) {
			gameManager.Resume();
		} else {
			gameManager.Pause();
		}
	}
	
	string GetButtonText()
	{
		return gameManager.IsPaused ? "RESUME" : "PAUSE";
	}
}
