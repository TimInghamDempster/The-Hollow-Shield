using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSlider : MonoBehaviour {

	bool m_dragging;
	float m_baseY;
	int m_availableUnits;

	List<WorldTileScript> m_sourceTiles;
	List<WorldTileScript> m_commitedTiles;

	// Use this for initialization
	void Start () {
		m_baseY = transform.position.y;
	}

	public void Init(List<WorldTileScript> sourceTiles)
	{
		m_sourceTiles = sourceTiles;
		m_commitedTiles = new List<WorldTileScript>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(m_dragging)
		{
			Vector3 pos = transform.position;
			Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);

			Vector3 mousePos = Input.mousePosition;

			Vector3 adjustedMousePos = mousePos;
			adjustedMousePos.z = screenPos.z;

			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(adjustedMousePos);

			pos.y = Mathf.Max(mouseWorldPos.y, m_baseY);
			float maxY = m_baseY + (m_availableUnits + m_commitedTiles.Count) / 100.0f;
			pos.y = Mathf.Min(pos.y, maxY);

			float relativeHeight = pos.y - m_baseY;
			int unitsCommited = (int)(relativeHeight * 100.0f);
			int unitsCommitedSinceLast = unitsCommited - m_commitedTiles.Count;

			if(unitsCommitedSinceLast > 0)
			{
				foreach(WorldTileScript tile in m_sourceTiles)
				{
					if(!tile.IsUnitInUse)
					{
						tile.IsUnitInUse = true;
						m_commitedTiles.Add(tile);
						unitsCommitedSinceLast--;
						if(unitsCommitedSinceLast == 0)
						{
							break;
						}
					}
				}
			}
			else if(unitsCommitedSinceLast < 0)
			{
				for(int i = m_commitedTiles.Count -1; i >= 0; i--)
				{
					WorldTileScript tile = m_commitedTiles[i];
					tile.IsUnitInUse = false;
					m_commitedTiles.RemoveAt(m_commitedTiles.Count - 1);
					unitsCommitedSinceLast++;
					if(unitsCommitedSinceLast == 0)
					{
						break;
					}
				}
			}
			
			transform.position = pos;
		}
		m_availableUnits = 0;
		foreach(WorldTileScript tile in m_sourceTiles)
		{
			if(!tile.IsUnitInUse)
			{
				m_availableUnits++;
			}
		}
	}
	
	void OnMouseUp()
	{
		m_dragging = false;
	}

	void OnMouseDown()
	{
		m_dragging = true;
	}
}
