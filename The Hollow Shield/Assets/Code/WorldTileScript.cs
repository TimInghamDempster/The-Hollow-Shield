using UnityEngine;
using System.Collections;

public enum TileTypes
{
	Grass,
	Snow,
	Sand,
	Mountain,
	Water,
	Forest,
	Sea,
	Castle,
	HuntingLodge,
	CombatSchool,
	ArcherySchool
}

public enum SeedTileTypes
{
	Grass,
	Snow,
	Sand,
	Mountain,
	Water
}

public class WorldTileScript : MonoBehaviour {

	public WorldGridScript worldGrid;
	public int x;
	public int y;
	public bool IsPassable = true;

	WorldTileScript[] m_neighbours;
	public FactionScript Faction;

	public TileTypes Type;

	bool m_isHighlighted;

	ArmyCounter m_army;

	public ArmyCounter Army { get {return m_army; } }

	// Use this for initialization
	void Start () {

		worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		worldGrid.AddTile(x,y,this);

		UpdateColour();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetMesh(Mesh mesh, Material material)
	{
		MeshFilter filter = gameObject.GetComponent<MeshFilter>();
		MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
		filter.mesh = mesh;
		renderer.material = material;
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
		m_army = army;
	}

	public void ArmyExit(ArmyCounter army)
	{
		m_army = null;
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
			this.gameObject.GetComponent<Renderer>().material.color = (Color.red * 0.1f) + (Color.white * 0.9f);
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
