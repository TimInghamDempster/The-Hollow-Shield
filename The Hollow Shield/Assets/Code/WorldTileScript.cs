using UnityEngine;
using System.Collections;

public class WorldTileScript : MonoBehaviour {

	public WorldGridScript worldGrid;
	public int x;
	public int y;

	// Use this for initialization
	void Start () {
		
		worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		worldGrid.AddTile(x,y,this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Unclick()
	{
		this.gameObject.GetComponent<Renderer>().material.color = Color.white;
	}

	void OnMouseDown(){
		this.gameObject.GetComponent<Renderer>().material.color = Color.blue;
		worldGrid.TileClicked(this);
	}
}
