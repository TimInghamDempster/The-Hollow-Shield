using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CastleScript : MonoBehaviour {
	
	GameObject m_archeryCounter;
	GameObject m_infantryCounter;
	GameObject m_cavalryCounter;
	GameObject m_hoverArmy;


	public List<WorldTileScript> infantryRecruitmentTiles;
	public List<WorldTileScript> archeryRecruitmentTiles;
	public List<WorldTileScript> cavalryRecruitmentTiles;
	public List<WorldTileScript> uselessTerritoryTiles;

	bool m_selected;

	// Use this for initialization
	void Start () {
	}

	public void Initialise(WorldGridScript world)
	{
		m_archeryCounter = new GameObject("Castle Archery Marker");
		m_infantryCounter = new GameObject("Castle Infantry Marker");
		m_cavalryCounter = new GameObject("Castle Cavalry Marker");


		Vector3 temp = gameObject.transform.position;
		temp.y = 5.0f;
		m_archeryCounter.transform.position = temp;
		m_infantryCounter.transform.position = temp;
		m_cavalryCounter.transform.position = temp;

		MeshRenderer archeryMeshRenderer = m_archeryCounter.AddComponent<MeshRenderer>();
		MeshFilter archeryMeshFilter = m_archeryCounter.AddComponent<MeshFilter>();
		archeryMeshRenderer.sharedMaterial = world.CounterMaterial;
		archeryMeshFilter.mesh = world.ArcheryCounterMesh;

		MeshRenderer infantryMeshRenderer = m_infantryCounter.AddComponent<MeshRenderer>();
		MeshFilter infantryMeshFilter = m_infantryCounter.AddComponent<MeshFilter>();
		infantryMeshRenderer.sharedMaterial = world.CounterMaterial;
		infantryMeshFilter.mesh = world.InfantryCounterMesh;

		MeshRenderer cavalryMeshRenderer = m_cavalryCounter.AddComponent<MeshRenderer>();
		MeshFilter cavalryMeshFilter = m_cavalryCounter.AddComponent<MeshFilter>();
		cavalryMeshRenderer.sharedMaterial = world.CounterMaterial;
		cavalryMeshFilter.mesh = world.CavalryCounterMesh;

		infantryRecruitmentTiles = new List<WorldTileScript>();
		archeryRecruitmentTiles = new List<WorldTileScript>();
		cavalryRecruitmentTiles = new List<WorldTileScript>();
		uselessTerritoryTiles = new List<WorldTileScript>();

		SpawnHoverArmy();
	}

	public void ClearLists()
	{
		infantryRecruitmentTiles.Clear();
		archeryRecruitmentTiles.Clear();
		cavalryRecruitmentTiles.Clear();
		uselessTerritoryTiles.Clear();
	}

	public void UpdateMarkerHeights()
	{
		Vector3 pos = gameObject.transform.position;
		float tileHeight = pos.y;

		float counterHeight = 0;

		foreach(WorldTileScript tile in infantryRecruitmentTiles)
		{
			if(!tile.IsUnitInUse)
			{
				counterHeight++;
			}
		}
		counterHeight /= 100;

		pos.y = tileHeight + counterHeight;
		m_infantryCounter.transform.position = pos;

		counterHeight = 0;
		
		foreach(WorldTileScript tile in archeryRecruitmentTiles)
		{
			if(!tile.IsUnitInUse)
			{
				counterHeight++;
			}
		}
		counterHeight /= 100;
		
		pos.y = tileHeight + counterHeight;
		m_archeryCounter.transform.position = pos;

		counterHeight = 0;
		
		foreach(WorldTileScript tile in cavalryRecruitmentTiles)
		{
			if(!tile.IsUnitInUse)
			{
				counterHeight++;
			}
		}
		counterHeight /= 100;
		
		pos.y = tileHeight + counterHeight;
		m_cavalryCounter.transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMarkerHeights();
	}

	void OnMouseDown()
	{
		if(!m_selected)
		{
			WorldTileScript tile = gameObject.GetComponent<WorldTileScript>();
			tile.worldGrid.ClearSelected();

			m_selected = true;
			gameObject.GetComponent<WorldTileScript>().Select();
			gameObject.GetComponent<WorldTileScript>().Highlight();
			m_hoverArmy.SetActive(true);
		}
		else
		{
			Deselect();
		}
	}

	public void Deselect()
	{
		m_selected = false;
		gameObject.GetComponent<WorldTileScript>().UnSelect();
		gameObject.GetComponent<WorldTileScript>().UnHighlight();
		m_hoverArmy.SetActive(false);
	}

	void SpawnHoverArmy()
	{
		m_hoverArmy = new GameObject();

		WorldTileScript tile = gameObject.GetComponent<WorldTileScript>();

		MeshFilter meshFilter = m_hoverArmy.AddComponent<MeshFilter>();
		MeshRenderer renderer = m_hoverArmy.AddComponent<MeshRenderer>();
		meshFilter.mesh = tile.Faction.FactionArmyMesh;
		renderer.material = tile.Faction.FactionArmyMaterial;
		renderer.material.color = tile.Faction.FactionColor;

		Vector3 pos = transform.position;
		pos.y += 3.0f;
		m_hoverArmy.transform.position = pos;

		HoverArmyScript hoverScript = m_hoverArmy.AddComponent<HoverArmyScript>();
		hoverScript.Initialise(this, tile.worldGrid);
		
		m_hoverArmy.SetActive(false);
	}
}
