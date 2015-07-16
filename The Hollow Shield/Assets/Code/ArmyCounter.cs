using UnityEngine;
using System.Collections;

public class ArmyCounter : MonoBehaviour {

	public float HoverPlane;

	Camera m_camera;
	bool m_selected;
	Transform m_transform;
	Vector3 m_positionWhenPickedUp;
	WorldGridScript m_worldGrid;
	WorldTileScript m_tile;
	WorldTileScript m_hoverTile;
	WorldPathPlanner m_pathPlanner;

	float m_xBound;
	float m_yBound;

	// Use this for initialization
	void Start () {
		m_transform = this.GetComponent<Transform>();
		m_worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		m_pathPlanner = this.GetComponent<WorldPathPlanner>();

		m_xBound = m_worldGrid.XBounds;
		m_yBound = m_worldGrid.YBounds;
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
							m_hoverTile.gameObject.GetComponent<Renderer>().material.color = Color.white;
						}
						else
						{
							m_hoverTile.gameObject.GetComponent<Renderer>().material.color = Color.red;
						}
					}
					if(tile.IsPassable)
					{
						tile.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
					}
					else
					{
						tile.gameObject.GetComponent<Renderer>().material.color = Color.red;
					}
					m_hoverTile = tile;
				}
			}

			m_transform.position = newPos;
		}
	}

	void OnMouseOver () 
	{
		if(Input.GetMouseButtonDown(1))
		{
			if(m_selected)
			{
				m_selected = false;
				m_transform.position = m_positionWhenPickedUp;

				if(m_hoverTile)
				{
					if(m_hoverTile.IsPassable)
					{
						m_hoverTile.gameObject.GetComponent<Renderer>().material.color = Color.white;
					}
					else
					{
						m_hoverTile.gameObject.GetComponent<Renderer>().material.color = Color.red;
					}
				}

			}
		}
		else
		{
			m_pathPlanner.HighlightPath();
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
		else
		{
			m_selected = false;
			m_transform.position = m_positionWhenPickedUp;

			if(m_hoverTile)
			{
				if(m_hoverTile.IsPassable)
				{
					m_hoverTile.gameObject.GetComponent<Renderer>().material.color = Color.white;
				}
				else
				{
					m_hoverTile.gameObject.GetComponent<Renderer>().material.color = Color.red;
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
}
