using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	public static ScoreManager instance;

	public UILabel scoreLabel;
	public int currentScore;

	// Use this for initialization
	void Awake () {
		instance = this;
		currentScore = 0;
	}

	public void OnStartGame(){
		currentScore = 0;
		UpdateScoreLabel ();
	}

	public void UpdateScoreLabel(){
		scoreLabel.text = currentScore.ToString ();

	}

	public void OnUserFinished(){
		currentScore++;
		UpdateScoreLabel ();
	}
}
