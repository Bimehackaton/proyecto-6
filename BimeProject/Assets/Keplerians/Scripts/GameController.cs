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
		ScoreManager.instance.OnStartGame ();
		HUDController.instance.OnStartGame ();
		gameState = GameState.PLAYING;
	}

	public void OnLostGame ()
	{
		gameState = GameState.LOST;
		ScoreManager.instance.UpdateFinalScore ();
		//TODO mostrar pantalla de perder
		HUDController.instance.ShowLostWindow ();
	}

	public void OnFinishGame(){
		ClientManager.instance.photonView.RPC("PlayerLost",PhotonTargets.Others);

		PhotonNetwork.LeaveRoom ();

	}

}
