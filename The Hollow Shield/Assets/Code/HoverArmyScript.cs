using UnityEngine;
using System.Collections;

public class HoverArmyScript : MonoBehaviour {

	GameObject m_archeryCounter;
	GameObject m_infantryCounter;
	GameObject m_cavalryCounter;

	CastleScript m_castle;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialise(CastleScript castle, WorldGridScript world)
	{
		m_archeryCounter = new GameObject("Hover Archery Marker");
		m_infantryCounter = new GameObject("Hover Infantry Marker");
		m_cavalryCounter = new GameObject("Hover Cavalry Marker");

		m_archeryCounter.transform.SetParent(transform);
		m_infantryCounter.transform.SetParent(transform);
		m_cavalryCounter.transform.SetParent(transform);
		
		
		Vector3 temp = gameObject.transform.position;
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

		m_cavalryCounter.AddComponent<MeshCollider>();
		m_infantryCounter.AddComponent<MeshCollider>();
		m_archeryCounter.AddComponent<MeshCollider>();
				
		UnitSlider cavalry = m_cavalryCounter.AddComponent<UnitSlider>();
		UnitSlider infantry = m_infantryCounter.AddComponent<UnitSlider>();
		UnitSlider archery = m_archeryCounter.AddComponent<UnitSlider>();

		infantry.Init(castle.infantryRecruitmentTiles);
		cavalry.Init(castle.cavalryRecruitmentTiles);
		archery.Init(castle.archeryRecruitmentTiles);

		m_castle = castle;
	}
}
