using UnityEngine;
using System.Collections;

public class OrdersCourierScript : MonoBehaviour {

	public ArmyCounter TargetArmy;
	public WorldTileScript TargetArmyNewObjective;

	public FactionScript Faction;

	WorldPathPlanner m_pathPlanner;
	public WorldTileScript Tile;

	Vector3 m_targetPos;
	public float Speed;
	WorldTileScript m_movementTargetTile;

	bool m_onSecondTileMove = false;

	public bool TurnEnded
	{
		get
		{
			return m_targetPos == transform.position;
		}
	}

	// Use this for initialization
	void Start ()
	{
		m_pathPlanner = gameObject.GetComponent<WorldPathPlanner>();
	
		m_targetPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_pathPlanner.Path.Count == 0)
		{
			PlanPath();
		}

		if(transform.position != m_targetPos)
		{
			Vector3 delta = m_targetPos - transform.position;
			float deltaDist = delta.magnitude;
			
			if(deltaDist > Speed)
			{
				delta.Normalize();
				delta *= Speed;
			}
			else // Change tile
			{
				Tile = m_movementTargetTile;
				if(Tile.Army == TargetArmy)
				{
					TargetArmy.SetObjective(TargetArmyNewObjective);
					Faction.m_orders.Remove(this);
					Destroy(this.gameObject);
				}
				if(m_onSecondTileMove == false)
				{
					m_onSecondTileMove = true;

					if(m_pathPlanner.DistanceAlongPath < m_pathPlanner.Path.Count - 1)
					{
						m_pathPlanner.DistanceAlongPath++;
						
						m_targetPos = m_pathPlanner.Path[m_pathPlanner.DistanceAlongPath].transform.position;
						m_movementTargetTile = m_pathPlanner.Path[m_pathPlanner.DistanceAlongPath];
					}
					else
					{
						if(Tile.Army != TargetArmy)
						{
							PlanPath();
						}
					}
				}
			}
			
			transform.position += delta;
			var tempPos = transform.position;
			transform.position = tempPos;//Need to do this properly sometime
		}
	}

	public void BeginEndTurn()
	{
		if(m_pathPlanner.Path.Count > 1)
		{
			m_onSecondTileMove = false;
			if(m_pathPlanner.DistanceAlongPath < m_pathPlanner.Path.Count - 1)
			{
				m_pathPlanner.DistanceAlongPath++;
				
				m_targetPos = m_pathPlanner.Path[m_pathPlanner.DistanceAlongPath].transform.position;
				m_movementTargetTile = m_pathPlanner.Path[m_pathPlanner.DistanceAlongPath];
			}
			else
			{
				if(Tile.Army != TargetArmy)
				{
					PlanPath();
				}
			}
		}
	}
	
	public void PlanPath()
	{
		m_pathPlanner.PlanPath(Tile, TargetArmy.Tile, true);
	}
}
