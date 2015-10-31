using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	public static ScoreManager instance;

	public UILabel scoreLabel;
	public UILabel scoreFinalLabel;

	public int currentScore;

	// Use this for initialization
	void Awake () {
		instance = this;
		currentScore = 0;
		UpdateScoreLabel ();
	}

	public void OnStartGame(){
		currentScore = 0;
		UpdateScoreLabel ();
	}

	public void UpdateScoreLabel(){
		currentScore = Mathf.RoundToInt(Mathf.Clamp (currentScore, 0, Mathf.Infinity));
		scoreLabel.text = currentScore.ToString ();

	}

	public void OnUserFinished(){
		currentScore++;
		UpdateScoreLabel ();
	}
	public void OnUserFinishedWrong(){
		currentScore--;
		UpdateScoreLabel ();
	}
	public void UpdateFinalScore(){
		scoreFinalLabel.text = currentScore.ToString ();
	}
}
