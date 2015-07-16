using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldPathPlanner : MonoBehaviour {

	WorldGridScript worldGrid;
	List<WorldTileScript> m_path;

	public int DistanceAlongPath {get;set;}

	public List<WorldTileScript> Path
	{
		get
		{
			return m_path;
		}
	}

	// Use this for initialization
	void Start () {
		worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		m_path = new List<WorldTileScript>();
	}

	public void PlanPath(WorldTileScript start, WorldTileScript destination)
	{
		bool[,] closed = new bool[worldGrid.TileCountX, worldGrid.TileCountY];
		WorldTileScript[,] parent = new WorldTileScript[worldGrid.TileCountX, worldGrid.TileCountY];

		List<WorldTileScript> openList = new List<WorldTileScript>();
		List<int> costList = new List<int>();
		List<int> distFromDest = new List<int>();


		openList.Add(destination);
		distFromDest.Add(0);
		costList.Add(CalcDistance(start, destination));

		while(openList.Count > 0)
		{
			int currentIndex = GetNearest(openList, costList);
			WorldTileScript current = openList[currentIndex];
			int currentCostToTile = costList[currentIndex];
			closed[current.x, current.y] = true;
			openList.RemoveAt(currentIndex);
			distFromDest.RemoveAt(currentIndex);
			costList.RemoveAt(currentIndex);

			if(current == start)
			{
				UnHighlightPath();

				// found so copy the path:
				m_path.Clear();
				DistanceAlongPath = 0;
				WorldTileScript pathHead = start;
				while(true)
				{
					m_path.Add(pathHead);
					pathHead = parent[pathHead.x, pathHead.y];
					if(pathHead == destination)
					{
						m_path.Add(pathHead);
						break;
					}
				}
				break;
			}

			foreach(WorldTileScript neighbour in current.GetNeighbours())
			{
				if(!neighbour.IsPassable)
				{
					closed[neighbour.x, neighbour.y] = true;
				}

				if(closed[neighbour.x, neighbour.y])
				{
					continue;
				}

				int costToHere = currentCostToTile + 1;
				int dist = CalcDistance(start, neighbour);

				int openListIndex = openList.FindIndex( a => a == neighbour);

				if(openListIndex == -1)
				{
					openList.Add(neighbour);
					costList.Add(costToHere);
					distFromDest.Add(dist);
					parent[neighbour.x, neighbour.y] = current;
				}
				else
				{
					if(costToHere < costList[openListIndex])
					{
						distFromDest[openListIndex] = dist;
						costList[openListIndex] = costToHere;
						parent[neighbour.x, neighbour.y] = current;
					}
				}
			}
		}
	}

	int CalcDistance(WorldTileScript start, WorldTileScript end)
	{
		Vector3 startVec = start.GetComponent<Transform>().position;
		Vector3 endVec = end.GetComponent<Transform>().position;

		return (int)Vector3.Distance(startVec, endVec);
	}

	int GetNearest(List<WorldTileScript> tiles, List<int> costs)
	{
		int cost = int.MaxValue;
		int best = -1;

		for(int i = 0; i < costs.Count; i++)
		{
			if(costs[i] < cost)
			{
				cost = costs[i];
				best = i;
			}
		}

		return best;
	}

	public void HighlightPath()
	{
		foreach(WorldTileScript tile in m_path)
		{
			tile.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
		}
	}

	public void UnHighlightPath()
	{
		foreach(WorldTileScript tile in m_path)
		{
			tile.gameObject.GetComponent<Renderer>().material.color = Color.white;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
