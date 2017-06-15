using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

  public int currentScore;
  public int level;
  public Text scoreFeedback;
  public Text levelFeedback;

  // Counter for when to increase difficulty
  private int interval;
  private int intervalMax;

	// Use this for initialization
	void Start () {
    scoreFeedback = GameObject.Find("Score").GetComponent<Text>();
    levelFeedback = GameObject.Find("Level").GetComponent<Text>();

    ResetScoreMan();
  }
	
	public void AddScore(int amt)
  {
    currentScore += amt;
    interval += amt;
    UpdateProcGenBehaviour();

    UpdateScoreDisplay();
  }

  public void ResetScoreMan()
  {
    level = 1;
    currentScore = 0;
    interval = 0;
    intervalMax = 10;

    UpdateScoreDisplay();
  }

  void UpdateScoreDisplay()
  {
    scoreFeedback.text = "Score: " + currentScore.ToString();
    levelFeedback.text = "Level: " + level.ToString();
  }

  // Changes the game state as levels get harder
  void UpdateProcGenBehaviour()
  {
    // Increase range every 20 score
    if (interval >= intervalMax)
    {
      ++level;
      interval -= intervalMax;

      // Every 1 interval
      ProcGenManager.IncreaseStartRange(1, 2);
      ProcGenManager.IncreaseEndRange(1, 2);

      // Every 2 intervals (even intervals)
      if (level % 2 == 0)
      {
        ProcGenManager.IncreaseGridSize(1, 1);
        ProcGenManager.IncreaseStartSpawnNum(1, 0);
        //ProcGenManager.IncreaseEndSpawnNum(1, 0);
      }
      
      // Every 2 intervals (odd intervals)
      if (level % 2 == 1)
      {
        ProcGenManager.IncreaseStartSpawnNum(0, 1);
        ProcGenManager.IncreaseEndSpawnNum(1, 1);
      }

    }
  }
}
