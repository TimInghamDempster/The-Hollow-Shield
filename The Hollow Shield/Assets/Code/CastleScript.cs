using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CastleScript : MonoBehaviour {
	
	GameObject archeryCounter;
	GameObject infantryCounter;
	GameObject cavalryCounter;


	public List<WorldTileScript> infantryRecruitmentTiles;
	public List<WorldTileScript> archeryRecruitmentTiles;
	public List<WorldTileScript> cavalryRecruitmentTiles;
	public List<WorldTileScript> uselessTerritoryTiles;

	// Use this for initialization
	void Start () {
	}

	public void Initialise(WorldGridScript world)
	{
		archeryCounter = new GameObject("Castle Archery Marker");
		infantryCounter = new GameObject("Infantry Archery Marker");
		cavalryCounter = new GameObject("Cavalry Archery Marker");


		Vector3 temp = gameObject.transform.position;
		temp.y = 5.0f;
		archeryCounter.transform.position = temp;
		infantryCounter.transform.position = temp;
		cavalryCounter.transform.position = temp;

		MeshRenderer archeryMeshRenderer = archeryCounter.AddComponent<MeshRenderer>();
		MeshFilter archeryMeshFilter = archeryCounter.AddComponent<MeshFilter>();
		archeryMeshRenderer.sharedMaterial = world.CounterMaterial;
		archeryMeshFilter.mesh = world.ArcheryCounterMesh;

		MeshRenderer infantryMeshRenderer = infantryCounter.AddComponent<MeshRenderer>();
		MeshFilter infantryMeshFilter = infantryCounter.AddComponent<MeshFilter>();
		infantryMeshRenderer.sharedMaterial = world.CounterMaterial;
		infantryMeshFilter.mesh = world.InfantryCounterMesh;

		MeshRenderer cavalryMeshRenderer = cavalryCounter.AddComponent<MeshRenderer>();
		MeshFilter cavalryMeshFilter = cavalryCounter.AddComponent<MeshFilter>();
		cavalryMeshRenderer.sharedMaterial = world.CounterMaterial;
		cavalryMeshFilter.mesh = world.CavalryCounterMesh;

		infantryRecruitmentTiles = new List<WorldTileScript>();
		archeryRecruitmentTiles = new List<WorldTileScript>();
		cavalryRecruitmentTiles = new List<WorldTileScript>();
		uselessTerritoryTiles = new List<WorldTileScript>();
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
		infantryCounter.transform.position = pos;

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
		archeryCounter.transform.position = pos;

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
		cavalryCounter.transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
