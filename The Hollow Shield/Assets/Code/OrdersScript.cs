using UnityEngine;
using System.Collections;

public class OrdersScript : MonoBehaviour {

	WorldGridScript m_grid;
	// Use this for initialization
	void Start () 
	{
		m_grid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnMouseDown()
	{
		m_grid.EndTurn();
	}
}
