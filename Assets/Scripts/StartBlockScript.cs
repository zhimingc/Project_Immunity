using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBlockScript : MonoBehaviour {

  // Flag for if the block is used in finishing the level
  public bool isInUse;

	// Use this for initialization
	void Start () {
    isInUse = false;
	}
	
	public void SetInUse(bool flag)
  {
    isInUse = flag;
  }


}
