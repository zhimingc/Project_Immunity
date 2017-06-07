using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

  public int currentScore;
  public Text scoreFeedback;

	// Use this for initialization
	void Start () {
    currentScore = 0;
    scoreFeedback = GameObject.Find("Score").GetComponent<Text>();
	}
	
	public void AddScore(int amt)
  {
    currentScore += amt;
    scoreFeedback.text = "Score: " + currentScore.ToString();
  }
}
