using UnityEngine;
using System.Collections;

public class PostInitialisationCallback : MonoBehaviour {

	// Use this for initialization
	void Start () {
		WorldGridScript grid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		grid.PostInitialise();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
