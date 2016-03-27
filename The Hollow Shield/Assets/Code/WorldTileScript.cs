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

public class CastleComponent
{
	public int numTroops;
	public int numArchers;
	public int numCavalry;
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
	public bool FactionHighlight;

	ArmyCounter m_army;

	public ArmyCounter Army { get {return m_army; } }

	public bool IsUnitInUse;

	int respawnTimer;

	bool mouseClicked;
	float lastClickTime;
	bool mouseUp;

	// Use this for initialization
	void Start () {

		worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		worldGrid.AddTile(x,y,this);

		UpdateColour();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mouseClicked)
		{
			if(Time.time - lastClickTime > worldGrid.DoubleClickTime)
			{
				mouseClicked = false;
				if(mouseUp)
				{
					Clicked();
				}
			}
		}
		UpdateColour();
	}

	public void UnitDied()
	{
		respawnTimer = 10;
	}

	public void EndTurn()
	{
		if(respawnTimer > 0)
		{
			respawnTimer--;
			if(respawnTimer == 0)
			{
				IsUnitInUse = false;
			}
		}
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
		//UpdateColour();
	}

	public void UnHighlight()
	{
		m_isHighlighted = false;
		//UpdateColour();
	}

	public void ArmyEnter(ArmyCounter army)
	{
		m_army = army;
	}

	public void ArmyExit(ArmyCounter army)
	{
		m_army = null;
	}

	void UpdateColour()
	{
		if(m_isHighlighted || worldGrid.hoverTile == this)
		{
			this.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
			return;

			if(!IsPassable)
			{
				this.gameObject.GetComponent<Renderer>().material.color = (Color.red * 0.1f) + (Color.white * 0.9f);
				return;
			}
		}
		else if(FactionHighlight)
		{
			gameObject.GetComponent<Renderer>().material.color = Faction.FactionColor;
		}
		else
		{
			if(Type == TileTypes.Castle)
			{
				GetComponent<Renderer>().material.color = Faction.FactionColor;
			}
			else
			{
				GetComponent<Renderer>().material.color = Color.white;
			}
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

	void OnMouseEnter () 
	{
		if(Type == TileTypes.Castle)
		{
			CastleScript castleScript = GetComponents<CastleScript>()[0];//Gaurunteed to be 1 and only 1

			foreach(var tile in castleScript.infantryRecruitmentTiles)
			{
				tile.FactionHighlight = true;
			}

			foreach(var tile in castleScript.archeryRecruitmentTiles)
			{
				tile.FactionHighlight = true;
			}

			foreach(var tile in castleScript.cavalryRecruitmentTiles)
			{
				tile.FactionHighlight = true;
			}

			foreach(var tile in castleScript.uselessTerritoryTiles)
			{
				tile.FactionHighlight = true;
			}
		}
		worldGrid.hoverTile = this;
	}

	void OnMouseExit()
	{
		
		if(Type == TileTypes.Castle)
		{
			CastleScript castleScript = GetComponents<CastleScript>()[0];//Gaurunteed to be 1 and only 1
			foreach(var tile in castleScript.infantryRecruitmentTiles)
			{
				tile.FactionHighlight = false;
			}
			foreach(var tile in castleScript.archeryRecruitmentTiles)
			{
				tile.FactionHighlight = false;
			}
			foreach(var tile in castleScript.cavalryRecruitmentTiles)
			{
				tile.FactionHighlight = false;
			}
			foreach(var tile in castleScript.uselessTerritoryTiles)
			{
				tile.FactionHighlight = false;
			}
		}
	}

	void Clicked()
	{
		int a = 0;
	}

	void DoubleClicked()
	{
		if(Type == TileTypes.Castle || Army != null)
		{
			Camera camera = FindObjectOfType<Camera>();
			UltimateOrbitCamera orbitCam = camera.GetComponent<UltimateOrbitCamera>();
			orbitCam.target = this.transform;
		}
	}

	void OnMouseDown()
	{
		if(mouseClicked)
		{
			DoubleClicked();
			mouseClicked = false;
		}
		else
		{
			mouseClicked = true;
		}
		lastClickTime = Time.time;
		mouseUp = false;
	}

	void OnMouseUp()
	{
		mouseUp = true;
	}
}
