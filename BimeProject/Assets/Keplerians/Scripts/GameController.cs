using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public static GameController instance;
	public enum GameState{MENU,PLAYING,LOST}
	public GameState gameState;

	// Use this for initialization
	void Awake() {
		instance = this;

		gameState = GameState.MENU;
	}
	
	public void OnStartGame(){
		HUDController.instance.OnStartGame ();
		gameState = GameState.PLAYING;
	}
}
