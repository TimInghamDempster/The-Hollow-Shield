using UnityEngine;
using System.Collections;

public class WorldTileScript : MonoBehaviour {

	public WorldGridScript worldGrid;
	public int x;
	public int y;
	public bool IsPassable = true;

	WorldTileScript[] m_neighbours;
	public FactionScript Faction;

	bool m_isHighlighted;

	// Use this for initialization
	void Start () {

		worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		worldGrid.AddTile(x,y,this);

		UpdateColour();

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

	public void Highlight()
	{
		m_isHighlighted = true;
		UpdateColour();
	}

	public void UnHighlight()
	{
		m_isHighlighted = false;
		UpdateColour();
	}

	public void ArmyEnter(ArmyCounter army)
	{
		Faction = army.Faction;
		UpdateColour();
	}

	public void ArmyExit(ArmyCounter army)
	{
	}

	void UpdateColour()
	{
		if(m_isHighlighted)
		{
			this.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
			return;
		}

		if(!IsPassable)
		{
			this.gameObject.GetComponent<Renderer>().material.color = Color.red;
			return;
		}

		if(Faction)
		{
			this.gameObject.GetComponent<Renderer>().material.color = Faction.FactionColor;
		}
		else
		{
			this.gameObject.GetComponent<Renderer>().material.color = Color.white;
		}
	}

	public void SetNeighbours(WorldTileScript[] neighbours)
	{
		m_neighbours = new WorldTileScript[neighbours.Length];

		for(int i = 0; i < neighbours.Length; i++)
		{
			m_neighbours[i] = neighbours[i];
		}
	}

	void OnMouseDown()
	{
	}
}
