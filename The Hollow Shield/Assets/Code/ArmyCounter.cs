using UnityEngine;
using System.Collections;

public class ArmyCounter : MonoBehaviour {

	public float HoverPlane;
	public FactionScript Faction;
	public Transform OrdersPrefab;

	bool m_selected;
	bool m_latched;
	Vector3 m_positionWhenPickedUp;
	WorldGridScript m_worldGrid;
	WorldTileScript m_tile;

	WorldTileScript m_targetTile;
	WorldPathPlanner m_pathPlanner;

	float m_xBound;
	float m_yBound;

	Vector3 m_targetPos;
	public float Speed; 

	public WorldTileScript Tile
	{
		get
		{
			return m_tile;
		}
	}

	public bool HasPath 
	{
		get
		{
			return m_pathPlanner.Path != null && m_pathPlanner.DistanceAlongPath < m_pathPlanner.Path.Count;
		}
	}

	public bool TurnEnded
	{
		get
		{
			return m_targetPos == transform.position;
		}
	}

	// Use this for initialization
	void Start () {
		m_worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		m_pathPlanner = this.GetComponent<WorldPathPlanner>();

		m_xBound = m_worldGrid.XBounds;
		m_yBound = m_worldGrid.YBounds;

		m_targetPos = transform.position;

		this.gameObject.GetComponent<Renderer>().material.color = Faction.FactionColor;

		RaycastHit tileHit;
		bool hit = Physics.Raycast(transform.position, new Vector3(0.0f, -1.0f, 0.0f), out tileHit, 2.0f * HoverPlane);
		
		if(hit)
		{
			WorldTileScript tile = tileHit.collider.gameObject.GetComponent<WorldTileScript>();
			if(tile)
			{				
				m_tile = tile;
				m_tile.ArmyEnter(this);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		if(m_selected)
		{
			Vector3 newPos = m_worldGrid.hoverTile.transform.position;
			newPos.y += HoverPlane;
			transform.position = newPos;

			if(Input.GetMouseButtonUp(0))
			{
				m_latched = false;
			}

			if(Input.GetMouseButtonDown(0) && !m_latched)
			{
				m_selected = false;
				transform.position = m_positionWhenPickedUp;

				if(Faction.HomeArmy == this)
				{
					if(m_tile &&
					   m_worldGrid.hoverTile.IsPassable)
					{
						SetObjective(m_worldGrid.hoverTile);
					}
				}
				else
				{
					Transform ordersOrigin = Faction.HomeArmy.transform;
					Transform orders = (Transform)Instantiate(OrdersPrefab, ordersOrigin.position, ordersOrigin.rotation);
					OrdersCourierScript os = orders.gameObject.GetComponent<OrdersCourierScript>();
					os.TargetArmy = this;
					os.TargetArmyNewObjective = m_worldGrid.hoverTile;
					os.Faction = Faction;
					os.Tile = Faction.HomeArmy.Tile;
					Faction.m_orders.Add(os);
				}
			}
		}
		else
		{
			if(transform.position != m_targetPos)
			{
				Vector3 delta = m_targetPos - transform.position;
				float deltaDist = delta.magnitude;

				if(deltaDist > Speed)
				{
					delta.Normalize();
					delta *= Speed;
				}
				else
				{
					if(m_tile)
					{
						m_tile.ArmyExit(this);
					}
					m_tile = m_targetTile;
					m_tile.ArmyEnter(this);
				}
				
				transform.position += delta;
			}
		}
	}

	public void SetObjective(WorldTileScript objectiveTile)
	{
		m_pathPlanner.PlanPath(m_tile, objectiveTile, false);
	}

	public void BeginEndTurn()
	{
		if(m_pathPlanner.Path.Count > 1)
		{
			if(m_pathPlanner.DistanceAlongPath < m_pathPlanner.Path.Count - 1)
			{
				m_pathPlanner.DistanceAlongPath++;

				m_targetPos = m_pathPlanner.Path[m_pathPlanner.DistanceAlongPath].transform.position;
				m_targetTile = m_pathPlanner.Path[m_pathPlanner.DistanceAlongPath];
			}
			else
			{
				m_pathPlanner.Path.Clear();
			}
		}
	}

	void OnMouseOver () 
	{
		if(Input.GetMouseButtonDown(1))
		{
			if(m_selected)
			{
				m_selected = false;
				transform.position = m_positionWhenPickedUp;
			}
		}
		else
		{
			if(!m_selected)
			{
				m_pathPlanner.HighlightPath();
			}
		}
	}

	void OnMouseExit()
	{
		m_pathPlanner.UnHighlightPath();
	}

	void OnMouseDown()
	{
		if(!m_selected)
		{
			m_positionWhenPickedUp = transform.position;
			m_selected = true;
			m_latched = true;

			if(!m_tile)
			{
				Vector3 newPos = m_positionWhenPickedUp;
				newPos.y = HoverPlane;
				RaycastHit tileHit;
				bool hit = Physics.Raycast(newPos, new Vector3(0.0f, -1.0f, 0.0f), out tileHit, 2.0f * HoverPlane);
				
				if(hit)
				{
					WorldTileScript tile = tileHit.collider.gameObject.GetComponent<WorldTileScript>();
					if(tile)
					{
						m_tile = tile;
					}
				}
			}
		}
	}
}
