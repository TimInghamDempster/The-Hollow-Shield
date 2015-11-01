using UnityEngine;
using System.Collections;

public class FactionAIScript : MonoBehaviour {

	public Transform OrdersPrefab;

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
						if(army == faction.HomeArmy)
						{
							army.SetObjective(tile);
						}
						else
						{
							bool hasOrders = false;
							foreach(OrdersCourierScript courier in faction.m_orders)
							{
								if(courier.TargetArmy == army)
								{
									hasOrders = true;
									break;
								}
							}
							if(hasOrders)
							{
								continue;
							}

							Transform ordersOrigin = faction.HomeArmy.transform;
							Transform orders = (Transform)Instantiate(OrdersPrefab, ordersOrigin.position, ordersOrigin.rotation);
							OrdersCourierScript os = orders.gameObject.GetComponent<OrdersCourierScript>();
							os.TargetArmy = army;
							os.TargetArmyNewObjective = tile;
							os.Faction = faction;
							os.Tile = faction.HomeArmy.Tile;
							faction.m_orders.Add(os);
						}
					}
				}
			}
		}
	}
}
