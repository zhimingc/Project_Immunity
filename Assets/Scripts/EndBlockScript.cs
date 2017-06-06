using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBlockScript : MonoBehaviour {

  public int endValue;

  // 0 for no input, -1 for wrong input, 1 for correct
  public int endStatus;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void UpdateEndStatus(int val)
  {
    if (val == endValue) endStatus = 1;
    else if (val != endValue) endStatus = -1;
    
    UpdateStatusFeedback();

    // Check if the level is completed
    //GameManager.Instance.CheckLevelComplete();
  }

  public void SetNoInput()
  {
    endStatus = 0;
    UpdateStatusFeedback();
  }

  public void UpdateStatusFeedback()
  {
    switch(endStatus)
    {
      case 0: // no input
        GetComponent<SpriteRenderer>().color = Color.white;

        break;
      case -1: // wrong input
        GetComponent<SpriteRenderer>().color = Color.red;

        break;
      case 1: // correct input
        GetComponent<SpriteRenderer>().color = Color.green;

        break;
    }
  }
}
