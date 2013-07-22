using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
	void OnGUI()
	{
		var buttonLeft = (Screen.width / 2) - 50;
		
		if(GUI.Button(new Rect(buttonLeft, 25, 100, 30), "START")) {			
			Application.LoadLevel(Scene.MainScene);
			GameManager.Instance.StartGame();
		}
		
		if(GUI.Button(new Rect(buttonLeft, 75, 100, 30), "EXIT")) {			
			Application.Quit();
		}
	}
}
