using UnityEngine;
using System.Collections;

public class WorldTileScript : MonoBehaviour {

	public WorldGridScript worldGrid;
	public int x;
	public int y;

	WorldTileScript[] m_neighbours;

	// Use this for initialization
	void Start () {
		worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		worldGrid.AddTile(x,y,this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetNeighbours(WorldTileScript[] neighbours)
	{
		m_neighbours = new WorldTileScript[neighbours.Length];

		for(int i = 0; i < neighbours.Length; i++)
		{
			m_neighbours[i] = neighbours[i];
		}
	}

	public void Unclick()
	{
		this.gameObject.GetComponent<Renderer>().material.color = Color.white;
	}

	void OnMouseDown(){
		this.gameObject.GetComponent<Renderer>().material.color = Color.blue;
		worldGrid.TileClicked(this);

		foreach(WorldTileScript neighbour in m_neighbours)
		{
			neighbour.gameObject.GetComponent<Renderer>().material.color = Color.green;
		}
	}
}
