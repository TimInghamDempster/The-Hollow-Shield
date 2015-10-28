using UnityEngine;
using System.Collections;

public class ArmyCounter : MonoBehaviour {

	public float HoverPlane;
	public FactionScript Faction;

	Camera m_camera;
	bool m_selected;
	bool m_latched;
	Vector3 m_positionWhenPickedUp;
	WorldGridScript m_worldGrid;
	WorldTileScript m_tile;

	WorldTileScript m_targetTile;

	WorldTileScript m_hoverTile;
	WorldPathPlanner m_pathPlanner;

	float m_xBound;
	float m_yBound;

	Vector3 m_targetPos;
	public float Speed; 

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
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		if(m_selected)
		{
			Transform cameraTransform = m_camera.gameObject.transform;

			float deltaY = cameraTransform.position.y - HoverPlane;

			float stepY = Mathf.Max(cameraTransform.forward.y, cameraTransform.forward.y * -1.0f);

			float dist = deltaY / stepY;

			Vector3 newPos = cameraTransform.position + (cameraTransform.forward * dist);

			newPos.x = Mathf.Clamp(newPos.x, 0.0f, m_xBound);
			newPos.z = Mathf.Clamp(newPos.z, 0.0f, m_yBound);
			newPos.y = HoverPlane;

			RaycastHit tileHit;
			bool hit = Physics.Raycast(newPos, new Vector3(0.0f, -1.0f, 0.0f), out tileHit, 2.0f * HoverPlane);

			if(hit)
			{
				WorldTileScript tile = tileHit.collider.gameObject.GetComponent<WorldTileScript>();
				if(tile && tile != m_hoverTile)
				{
					if(m_hoverTile)
					{
						if(m_hoverTile.IsPassable)
						{
							m_hoverTile.UnHighlight();
						}
					}

					if(tile.IsPassable)
					{
						tile.Highlight();
					}

					m_hoverTile = tile;
				}
			}

			transform.position = newPos;

			if(Input.GetMouseButtonUp(0))
			{
				m_latched = false;
			}

			if(Input.GetMouseButtonDown(0) && !m_latched)
			{
				m_selected = false;
				transform.position = m_positionWhenPickedUp;
				
				if(m_hoverTile)
				{
					if(m_hoverTile.IsPassable)
					{
						m_hoverTile.UnHighlight();
					}
				}
				
				if(m_tile &&
				   m_hoverTile &&
				   m_hoverTile.IsPassable)
				{
					m_pathPlanner.PlanPath(m_tile, m_hoverTile);
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
		m_pathPlanner.PlanPath(m_tile, objectiveTile);
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

				if(m_hoverTile)
				{
					if(m_hoverTile.IsPassable)
					{
						m_hoverTile.UnHighlight();
					}
				}

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
			m_camera = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
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
