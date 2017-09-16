using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quasi : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Input.mousePosition;
	}
}
