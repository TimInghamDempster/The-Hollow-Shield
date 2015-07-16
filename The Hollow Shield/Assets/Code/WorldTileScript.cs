using UnityEngine;
using System.Collections;

public class WorldTileScript : MonoBehaviour {

	public WorldGridScript worldGrid;
	public int x;
	public int y;
	public bool IsPassable = true;

	WorldTileScript[] m_neighbours;

	// Use this for initialization
	void Start () {
		worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		worldGrid.AddTile(x,y,this);
		if(!IsPassable)
		{
			this.gameObject.GetComponent<Renderer>().material.color = Color.red;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public WorldTileScript[] GetNeighbours()
	{
		return m_neighbours;
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

	public void Highlight(Color colour)
	{
		this.gameObject.GetComponent<Renderer>().material.color = colour;

	}

	public void HighlightNeighbour()
	{
		foreach(WorldTileScript neighbour in m_neighbours)
		{
			if(neighbour.IsPassable)
			{
				neighbour.gameObject.GetComponent<Renderer>().material.color = Color.green;
			}
			else
			{
				neighbour.gameObject.GetComponent<Renderer>().material.color = Color.red;
			}
		}
	}

	public void UnHighlight()
	{
		this.gameObject.GetComponent<Renderer>().material.color = Color.white;

		foreach(WorldTileScript neighbour in m_neighbours)
		{
			neighbour.gameObject.GetComponent<Renderer>().material.color = Color.white;
		}
	}

	void OnMouseDown()
	{

		worldGrid.ClearSelection();
		Highlight(Color.blue);
		HighlightNeighbour();
	}
}
