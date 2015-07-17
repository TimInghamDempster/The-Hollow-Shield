using UnityEngine;
using System.Collections;

public class FactionAIScript : MonoBehaviour {

	WorldGridScript m_worldGrid;
	// Use this for initialization
	void Start () 
	{
		m_worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void DoAI(FactionScript faction)
	{
		foreach(ArmyCounter army in faction.m_armies)
		{
			if(!army.HasPath)
			{
				bool success = false;

				while(!success)
				{
					int x = (int)(Random.value * m_worldGrid.TileCountX);
					int y = (int)(Random.value * m_worldGrid.TileCountY);

					WorldTileScript tile = m_worldGrid.GetTile(x, y);

					if(tile.IsPassable)
					{
						success = true;
						army.SetObjective(tile);
					}
				}
			}
		}
	}
}
