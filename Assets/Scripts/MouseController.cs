using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

 // public int objsIntersecting;

	// Use this for initialization
	void Start () {
  }
	
	// Update is called once per frame
	void Update () {
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0.0f;
    transform.position = mousePos;
  }
}
