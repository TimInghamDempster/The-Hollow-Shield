using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Point2d
{
	public int x;
	public int y;
}

[System.Serializable]
public class Range
{
	public int max;
	public int min;
}

[System.Serializable]
public class CoastSetup
{
	public int Seed;
	public Point2d SWCornerOffset;
	public Point2d NWCornerOffset;
	public Point2d NECornerOffset;
	public Point2d SECornerOffset;
	public SubductionMountainProperties NorthMountains;
	public SubductionMountainProperties EastMountains;
	public SubductionMountainProperties SouthMountains;
	public SubductionMountainProperties WestMountains;
	public List<Range> MinAndMaxDisplacements;
}

[System.Serializable]
public class SubductionMountainProperties
{
	public bool Active;
	public Point2d StartOffset;
	public Point2d EndOffset;
	public int ThickeningIterations;
	public float ThickeningProbability;
}

[System.Serializable]
public class TectonicsProperties
{
	public int NumberOfPlates;
	public int MinimumSeparation;
	public int Seed;
	public int NumberOfSecondaryPoints;
	public int SecondaryMaximumSeparations;
}

public class WorldGridScript : MonoBehaviour {


	public int TileCountX;
	public int TileCountY;
	public FactionScript PlayerFaction;

	public List<FactionScript> m_factions = new List<FactionScript>();

	int m_currentFaction;
	bool m_factionChanging = false;

	public CoastSetup CoastProperties;
	public TectonicsProperties TectonicsSetup;

	public Mesh WaterTileMesh;
	public Material WaterTileMaterial;
	public Mesh GrassTileMesh;
	public Material GrassTileMaterial;
	public Mesh SandTileMesh;
	public Material SandTileMaterial;
	public Mesh SnowTileMesh;
	public Material SnowTileMaterial;
	public Mesh MountainMesh;
	public Material MountainMaterial;

	
	WorldTileScript[,] m_tiles;
	List<WorldTileScript> m_collisionFaults = new List<WorldTileScript>();
	List<WorldTileScript> m_slipFaults = new List<WorldTileScript>();
	List<WorldTileScript> m_separationFaults = new List<WorldTileScript>();

	public float XBounds
	{
		get
		{
			return TileCountX * 1.4f;
		}
	}

	public float YBounds
	{
		get
		{
			return TileCountY * 1.66f;
		}
	}


	public void ClearTiles()
	{
		for(int y = 0; y < TileCountY; y++)
		{
			for(int x = 0; x < TileCountX; x++)
			{
				WorldTileScript tile = m_tiles[x, y];
				tile.Type = TileTypes.Grass;
				tile.SetMesh(GrassTileMesh, GrassTileMaterial);
			}
		}
	}

	public void GenerateCoastline()
	{
		List<WorldTileScript> waterTiles = new List<WorldTileScript>();
		Random.seed = CoastProperties.Seed;

		for(int x = 0; x < TileCountX; x++)
		{
			waterTiles.Add(m_tiles[x, 0]);
			waterTiles.Add(m_tiles[x, TileCountY - 1]);
		}

		for(int y = 0; y < TileCountY; y++)
		{
			waterTiles.Add(m_tiles[0, y]);
			waterTiles.Add(m_tiles[TileCountY - 1, y]);
		}

		List<Point2d> landCorners = new List<Point2d>();
		List<Point2d> seaCorners = new List<Point2d>();

		int min = CoastProperties.MinAndMaxDisplacements[0].min;
		int max = CoastProperties.MinAndMaxDisplacements[0].max;

		landCorners.Add(new Point2d() {x = Random.Range(min, max) + CoastProperties.SWCornerOffset.x,
			y = Random.Range(min, max) + CoastProperties.SWCornerOffset.y});		
		landCorners.Add(new Point2d() {x = Random.Range(min, max) + CoastProperties.NWCornerOffset.x,
			y = TileCountY - Random.Range(min, max) - 1 + CoastProperties.NWCornerOffset.y});		
		landCorners.Add(new Point2d() {x = TileCountX - Random.Range(min, max) - 1 + CoastProperties.SECornerOffset.x,
			y = Random.Range(min, max) + CoastProperties.SECornerOffset.y});
		landCorners.Add(new Point2d() {x = TileCountX - Random.Range(min, max) - 1 + CoastProperties.NECornerOffset.x,
			y = TileCountY - Random.Range(min, max) - 1 + CoastProperties.NECornerOffset.y});

		List<List<Point2d>> landEdges = new List<List<Point2d>>();
		List<List<Point2d>> seaEdges = new List<List<Point2d>>();
		List<List<Point2d>> mountainEdges = new List<List<Point2d>>();

		landEdges.Add(new List<Point2d>() {
			landCorners[1], landCorners[3]});
		landEdges.Add(new List<Point2d>() {
			landCorners[3], landCorners[2]});
		landEdges.Add(new List<Point2d>() {
			landCorners[2], landCorners[0]});
		landEdges.Add(new List<Point2d>() {
			landCorners[0], landCorners[1]});

		seaCorners.Add(new Point2d(){x = landCorners[0].x - 2, y = landCorners[0].y - 2});
		seaCorners.Add(new Point2d(){x = landCorners[1].x - 2, y = landCorners[1].y + 2});
		seaCorners.Add(new Point2d(){x = landCorners[2].x + 2, y = landCorners[2].y - 2});
		seaCorners.Add(new Point2d(){x = landCorners[3].x + 2, y = landCorners[3].y + 2});

		for(int i = 0; i < seaCorners.Count; i++)
		{
			Point2d point = seaCorners[i];
			point.x = StaticHelpers.Clamp(point.x, 0, TileCountX - 1);
			point.y = StaticHelpers.Clamp(point.y, 0, TileCountY - 1);
			seaCorners[i] = point;
		}

		seaEdges.Add(new List<Point2d>() {
			seaCorners[1], seaCorners[3]});
		seaEdges.Add(new List<Point2d>() {
			seaCorners[3], seaCorners[2]});
		seaEdges.Add(new List<Point2d>() {
			seaCorners[2], seaCorners[0]});
		seaEdges.Add(new List<Point2d>() {
			seaCorners[0], seaCorners[1]});

		mountainEdges.Add(new List<Point2d>() {
			landCorners[1], landCorners[3]});
		mountainEdges.Add(new List<Point2d>() {
			landCorners[3], landCorners[2]});
		mountainEdges.Add(new List<Point2d>() {
			landCorners[2], landCorners[0]});
		mountainEdges.Add(new List<Point2d>() {
			landCorners[0], landCorners[1]});

		mountainEdges[0][0] = new Point2d()
			{x = mountainEdges[0][0].x + CoastProperties.NorthMountains.StartOffset.x,
			y = mountainEdges[0][0].y + CoastProperties.NorthMountains.StartOffset.y};
		mountainEdges[0][1] = new Point2d()
		{x = mountainEdges[0][1].x + CoastProperties.NorthMountains.EndOffset.x,
			y = mountainEdges[0][1].y + CoastProperties.NorthMountains.EndOffset.y};

		mountainEdges[1][0] = new Point2d()
		{x = mountainEdges[1][0].x + CoastProperties.EastMountains.StartOffset.x,
			y = mountainEdges[1][0].y + CoastProperties.EastMountains.StartOffset.y};
		mountainEdges[1][1] = new Point2d()
		{x = mountainEdges[1][1].x + CoastProperties.EastMountains.EndOffset.x,
			y = mountainEdges[1][1].y + CoastProperties.EastMountains.EndOffset.y};

		mountainEdges[2][0] = new Point2d()
		{x = (mountainEdges[2][0].x + CoastProperties.SouthMountains.StartOffset.x),
			y = (mountainEdges[2][0].y + CoastProperties.SouthMountains.StartOffset.y)};
		mountainEdges[2][1] = new Point2d()
		{x = mountainEdges[2][1].x + CoastProperties.SouthMountains.EndOffset.x,
			y = mountainEdges[2][1].y + CoastProperties.SouthMountains.EndOffset.y};

		mountainEdges[3][0] = new Point2d()
		{x = mountainEdges[3][0].x + CoastProperties.WestMountains.StartOffset.x,
			y = mountainEdges[3][0].y + CoastProperties.WestMountains.StartOffset.y};
		mountainEdges[3][1] = new Point2d()
		{x = mountainEdges[3][1].x + CoastProperties.WestMountains.EndOffset.x,
			y = mountainEdges[3][1].y + CoastProperties.WestMountains.EndOffset.y};

		for(int i = 0; i < mountainEdges.Count; i++)
		{
			List<Point2d> edge = mountainEdges[i];
			for(int j = 0; j < edge.Count; j++)
			{
				edge[j] = new Point2d()
				{ x = StaticHelpers.Clamp(edge[j].x, 1, TileCountX - 2),
					y = StaticHelpers.Clamp(edge[j].y, 1, TileCountY - 2)};
			}
		}

		for(int i = 1; i < CoastProperties.MinAndMaxDisplacements.Count; i++)
		{
			int rangeMin = CoastProperties.MinAndMaxDisplacements[i].min;
			int rangeMax = CoastProperties.MinAndMaxDisplacements[i].max;

			List<List<Point2d>> doubledLandEdges = new List<List<Point2d>>();
			List<List<Point2d>> doubledSeaEdges = new List<List<Point2d>>();
			List<List<Point2d>> doubledMountainEdges = new List<List<Point2d>>();

			for(int k = 0; k < landEdges.Count; k++)
			{
				List<Point2d> landEdge = landEdges[k];
				List<Point2d> seaEdge = seaEdges[k];
				List<Point2d> mountainEdge = mountainEdges[k];

				List<Point2d> newLandEdge = new List<Point2d>();
				doubledLandEdges.Add(newLandEdge);

				List<Point2d> newSeaEdge = new List<Point2d>();
				doubledSeaEdges.Add(newSeaEdge);

				List<Point2d> newMountainEdge = new List<Point2d>();
				doubledMountainEdges.Add(newMountainEdge);

				for(int j = 0; j < landEdge.Count - 1; j++)
				{
					int deltaX = Random.Range(rangeMin, rangeMax);
					int deltaY = Random.Range(rangeMin, rangeMax);

					Point2d firstLandPoint = landEdge[j];
					Point2d secondLandPoint = landEdge[j + 1];
					Point2d newLandPoint;
					newLandPoint.x = (firstLandPoint.x + secondLandPoint.x) / 2;
					newLandPoint.y = (firstLandPoint.y + secondLandPoint.y) / 2;

					newLandPoint.x += deltaX;
					newLandPoint.y += deltaY;

					newLandPoint.x = StaticHelpers.Clamp(newLandPoint.x, 1, TileCountX - 2);
					newLandPoint.y = StaticHelpers.Clamp(newLandPoint.y, 1, TileCountY - 2);

					newLandEdge.Add(firstLandPoint);
					newLandEdge.Add(newLandPoint);

					if(i < CoastProperties.MinAndMaxDisplacements.Count - 2)
					{
						Point2d firstSeaPoint = seaEdge[j];
						Point2d secondSeaPoint = seaEdge[j + 1];
						Point2d newSeaPoint;
						newSeaPoint.x = (firstSeaPoint.x + secondSeaPoint.x) / 2;
						newSeaPoint.y = (firstSeaPoint.y + secondSeaPoint.y) / 2;
						
						newSeaPoint.x += deltaX;
						newSeaPoint.y += deltaY;
						
						newSeaPoint.x = StaticHelpers.Clamp(newSeaPoint.x, 0, TileCountX - 1);
						newSeaPoint.y = StaticHelpers.Clamp(newSeaPoint.y, 0, TileCountY - 1);
						
						newSeaEdge.Add(firstSeaPoint);
						newSeaEdge.Add(newSeaPoint);
					}
					Point2d firstMountainPoint = mountainEdge[j];
					Point2d secondMountainPoint = mountainEdge[j + 1];
					Point2d newMountainPoint;
					newMountainPoint.x = (firstMountainPoint.x + secondMountainPoint.x) / 2;
					newMountainPoint.y = (firstMountainPoint.y + secondMountainPoint.y) / 2;
					
					newMountainPoint.x += deltaX;
					newMountainPoint.y += deltaY;
					
					newMountainPoint.x = StaticHelpers.Clamp(newMountainPoint.x, 0, TileCountX - 1);
					newMountainPoint.y = StaticHelpers.Clamp(newMountainPoint.y, 0, TileCountY - 1);
					
					newMountainEdge.Add(firstMountainPoint);
					newMountainEdge.Add(newMountainPoint);
				}
				newLandEdge.Add(landEdge[landEdge.Count - 1]);
				newMountainEdge.Add(mountainEdge[mountainEdge.Count - 1]);

				if(i < CoastProperties.MinAndMaxDisplacements.Count - 2)
				{
					newSeaEdge.Add(seaEdge[seaEdge.Count - 1]);
				}
			}
			landEdges = doubledLandEdges;
			mountainEdges = doubledMountainEdges;

			if(i < CoastProperties.MinAndMaxDisplacements.Count - 2)
			{
				seaEdges = doubledSeaEdges;
			}
		}

		List<WorldTileScript> sandTiles = new List<WorldTileScript>();

		foreach(List<Point2d> list in landEdges)
		{
			foreach(Point2d point in list)
			{
				sandTiles.Add(m_tiles[point.x, point.y]);
			}
		}
		foreach(List<Point2d> edge in seaEdges)
		{
			foreach(Point2d point in edge)
			{
				waterTiles.Add(m_tiles[point.x, point.y]);
			}
		}

		List<List<WorldTileScript>> fillInputLists = 
			new List<List<WorldTileScript>>(){	sandTiles, waterTiles };
		List<List<WorldTileScript>> fillOutputLists;

		StaticHelpers.FloodFill(TileCountX, TileCountY, fillInputLists, out fillOutputLists);

		foreach(WorldTileScript tile in fillOutputLists[0])
		{
			tile.Type = TileTypes.Grass;
			tile.SetMesh(GrassTileMesh, GrassTileMaterial);
		}

		foreach(WorldTileScript tile in fillOutputLists[1])
		{
			tile.Type = TileTypes.Water;
			tile.SetMesh(WaterTileMesh, WaterTileMaterial);
		}

		List<WorldTileScript> totalMountains = new List<WorldTileScript>();

		if(CoastProperties.NorthMountains.Active)
		{
			List<WorldTileScript> localMountains = new List<WorldTileScript>();
			List<WorldTileScript> newLocalMountains = new List<WorldTileScript>();
			
			foreach(Point2d point in mountainEdges[0])
			{
				localMountains.Add(m_tiles[point.x, point.y]);
				totalMountains.Add(m_tiles[point.x, point.y]);
			}
			
			bool[,] closed = new bool[TileCountX, TileCountY];
			
			for(int i = 0; i < CoastProperties.NorthMountains.ThickeningIterations; i++)
			{
				foreach(WorldTileScript tile in localMountains)
				{
					closed[tile.x, tile.y] = true;
					WorldTileScript[] neighbours = tile.GetNeighbours();
					for(int j = 0; j < neighbours.Length; j++)
					{
						WorldTileScript neighbour = neighbours[j];
						if(Random.value > CoastProperties.NorthMountains.ThickeningProbability)
						{
							if(!closed[neighbour.x, neighbour.y])
							{
								newLocalMountains.Add(neighbour);
								totalMountains.Add(neighbour);
							}
						}
						closed[neighbour.x, neighbour.y] = true;
					}
				}
				List<WorldTileScript> temp = localMountains;
				localMountains = newLocalMountains;
				newLocalMountains = temp;
				newLocalMountains.Clear();
			}
		}

		if(CoastProperties.EastMountains.Active)
		{
			List<WorldTileScript> localMountains = new List<WorldTileScript>();
			List<WorldTileScript> newLocalMountains = new List<WorldTileScript>();
			
			foreach(Point2d point in mountainEdges[1])
			{
				localMountains.Add(m_tiles[point.x, point.y]);
				totalMountains.Add(m_tiles[point.x, point.y]);
			}
			
			bool[,] closed = new bool[TileCountX, TileCountY];
			
			for(int i = 0; i < CoastProperties.EastMountains.ThickeningIterations; i++)
			{
				foreach(WorldTileScript tile in localMountains)
				{
					closed[tile.x, tile.y] = true;
					WorldTileScript[] neighbours = tile.GetNeighbours();
					for(int j = 0; j < neighbours.Length; j++)
					{
						WorldTileScript neighbour = neighbours[j];
						if(Random.value > CoastProperties.EastMountains.ThickeningProbability)
						{
							if(!closed[neighbour.x, neighbour.y])
							{
								newLocalMountains.Add(neighbour);
								totalMountains.Add(neighbour);
							}
						}
						closed[neighbour.x, neighbour.y] = true;
					}
				}
				List<WorldTileScript> temp = localMountains;
				localMountains = newLocalMountains;
				newLocalMountains = temp;
				newLocalMountains.Clear();
			}
		}

		if(CoastProperties.SouthMountains.Active)
		{
			List<WorldTileScript> localMountains = new List<WorldTileScript>();
			List<WorldTileScript> newLocalMountains = new List<WorldTileScript>();

			foreach(Point2d point in mountainEdges[2])
			{
				localMountains.Add(m_tiles[point.x, point.y]);
				totalMountains.Add(m_tiles[point.x, point.y]);
			}

			bool[,] closed = new bool[TileCountX, TileCountY];

			for(int i = 0; i < CoastProperties.SouthMountains.ThickeningIterations; i++)
			{
				foreach(WorldTileScript tile in localMountains)
				{
					closed[tile.x, tile.y] = true;
					WorldTileScript[] neighbours = tile.GetNeighbours();
					for(int j = 0; j < neighbours.Length; j++)
					{
						WorldTileScript neighbour = neighbours[j];
						if(Random.value > CoastProperties.SouthMountains.ThickeningProbability)
						{
							if(!closed[neighbour.x, neighbour.y])
							{
								newLocalMountains.Add(neighbour);
								totalMountains.Add(neighbour);
							}
						}
						closed[neighbour.x, neighbour.y] = true;
					}
				}
				List<WorldTileScript> temp = localMountains;
				localMountains = newLocalMountains;
				newLocalMountains = temp;
				newLocalMountains.Clear();
			}
		}

		if(CoastProperties.WestMountains.Active)
		{
			List<WorldTileScript> localMountains = new List<WorldTileScript>();
			List<WorldTileScript> newLocalMountains = new List<WorldTileScript>();
			
			foreach(Point2d point in mountainEdges[3])
			{
				localMountains.Add(m_tiles[point.x, point.y]);
				totalMountains.Add(m_tiles[point.x, point.y]);
			}
			
			bool[,] closed = new bool[TileCountX, TileCountY];
			
			for(int i = 0; i < CoastProperties.WestMountains.ThickeningIterations; i++)
			{
				foreach(WorldTileScript tile in localMountains)
				{
					closed[tile.x, tile.y] = true;
					WorldTileScript[] neighbours = tile.GetNeighbours();
					for(int j = 0; j < neighbours.Length; j++)
					{
						WorldTileScript neighbour = neighbours[j];
						if(Random.value > CoastProperties.WestMountains.ThickeningProbability)
						{
							if(!closed[neighbour.x, neighbour.y])
							{
								newLocalMountains.Add(neighbour);
								totalMountains.Add(neighbour);
							}
						}
						closed[neighbour.x, neighbour.y] = true;
					}
				}
				List<WorldTileScript> temp = localMountains;
				localMountains = newLocalMountains;
				newLocalMountains = temp;
				newLocalMountains.Clear();
			}
		}

		foreach(WorldTileScript tile in totalMountains)
		{
			if(tile.Type == TileTypes.Grass)
			{
				tile.SetMesh(MountainMesh, MountainMaterial);
				tile.Type = TileTypes.Mountain;
			}
		}
	}

	public void DoTectonics ()
	{
		Random.seed = TectonicsSetup.Seed;

		int[,] tectonicGrid = new int[TileCountX, TileCountY];

		List<List<WorldTileScript>> inputLists = new List<List<WorldTileScript>>();
		List<List<WorldTileScript>> outputLists = new List<List<WorldTileScript>>();

		for(int i = 0; i < TectonicsSetup.NumberOfPlates; i++)
		{
			for(int j = 0; j < 10000; j++)
			{
				int x = Random.Range(0, TileCountX);
				int y = Random.Range(0, TileCountY);
			

				if(tectonicGrid[x,y] != i || i == 0)
				{
					int dist = int.MaxValue;

					for(int k = 0; k <  inputLists.Count; k++)
					{
						int deltaX = inputLists[k][0].x - x;
						int deltaY = inputLists[k][0].y - y;
						int newDist = deltaX * deltaX + deltaY * deltaY;
						dist = newDist < dist ? newDist : dist;
					}

					if(dist > (TectonicsSetup.MinimumSeparation * TectonicsSetup.MinimumSeparation))
					{
						List<WorldTileScript> inputList = new List<WorldTileScript>();
						inputList.Add(m_tiles[x,y]);
						inputLists.Add(inputList);

						int secondaryDist = TectonicsSetup.SecondaryMaximumSeparations;
						for(int u = 0; u < TectonicsSetup.NumberOfSecondaryPoints; u++)
						{
							int x2 = x + Random.Range(secondaryDist * -1, secondaryDist);
							int y2 = y + Random.Range(secondaryDist * -1, secondaryDist);

							if(x2 >= 0 &&
							   x2 < TileCountX &&
							   y2 >= 0 &&
							   y2 < TileCountY)
							{
								inputList.Add(m_tiles[x2,y2]);
							}
						}

						break;
					}
				}
			}
		}

		StaticHelpers.FloodFill(TileCountX, TileCountY, inputLists, out outputLists);

		List<Vector2> tectonicMovementVectors = new List<Vector2>();
	
		for(int i = 0; i < outputLists.Count; i++)
		{
			foreach(WorldTileScript tile in outputLists[i])
			{
				tectonicGrid[tile.x, tile.y] = i;
			}

			Vector2 tectonicVector = new Vector2(Random.value - 0.5f, Random.value - 0.5f);
			tectonicVector.Normalize();
			tectonicMovementVectors.Add(tectonicVector);
		}

		for(int x = 0; x < TileCountX; x++)
		{
			for(int y = 0; y < TileCountY; y++)
			{
				var tile = m_tiles[x,y];
				int set = tectonicGrid[x,y];
				foreach(var neighbour in tile.GetNeighbours())
				{
					int secondSet = tectonicGrid[neighbour.x, neighbour.y];
					if(secondSet != set)
					{
						float closingSpeed = Vector2.Dot(tectonicMovementVectors[set], tectonicMovementVectors[secondSet]);
						if(closingSpeed > 0.9f)
						{
							m_collisionFaults.Add(tile);
							//tile.SetMesh(GrassTileMesh, null); // Uncomment for hacky visualisation
						}
						else if(closingSpeed < -0.8f)
						{
							m_separationFaults.Add(tile);
							//tile.SetMesh(SnowTileMesh, SnowTileMaterial); // Uncomment for hacky visualisation
						}
						else
						{
							m_slipFaults.Add(tile);
							//tile.SetMesh(SandTileMesh, SandTileMaterial); // Uncomment for hacky visualisation
						}

					}
				}
			}
		}
	}

	public void DiscoverAndAddTiles()
	{
		m_tiles = new WorldTileScript[TileCountX, TileCountY];
		WorldTileScript[] foundTiles = GameObject.FindObjectsOfType<WorldTileScript>();

		foreach(WorldTileScript tile in foundTiles)
		{
			AddTile(tile.x, tile.y, tile);
		}

		PostInitialise();
	}

	public void PostInitialise()
	{
		for(int y = 0; y < TileCountY; y++)
		{
			for(int x = 0; x < TileCountX; x++)
			{
				List<WorldTileScript> neighbours = new List<WorldTileScript>();
				if(y == 0)
				{
					neighbours.Add(m_tiles[x,1]);
				}
				else
				{
					if(y < TileCountY  - 1)
					{
						neighbours.Add(m_tiles[x, y - 1]);
						neighbours.Add(m_tiles[x, y + 1]);
					}
					else
					{
						neighbours.Add(m_tiles[x,TileCountY - 2]);
					}
				}
				if(x == 0)
				{
					neighbours.Add(m_tiles[1,y]);
				}
				else
				{
					if(x < TileCountX  - 1)
					{
						neighbours.Add(m_tiles[x - 1, y]);
						neighbours.Add(m_tiles[x + 1, y]);
					}
					else
					{
						neighbours.Add(m_tiles[TileCountX - 2, y]);
					}
				}
				if(x % 2 == 0)
				{
					if(y < TileCountY  - 1)
					{
						if(x == 0)
						{
							neighbours.Add(m_tiles[1,y + 1]);
						}
						else
						{
							if(x < TileCountX  - 1)
							{
								neighbours.Add(m_tiles[x - 1, y + 1]);
								neighbours.Add(m_tiles[x + 1, y + 1]);
							}
							else
							{
								neighbours.Add(m_tiles[TileCountX - 2, y + 1]);
							}
						}
					}
				}
				else
				{
					if(y > 0)
					{
						if(x == 0)
						{
							neighbours.Add(m_tiles[1,y - 1]);
						}
						else
						{
							if(x < TileCountX  - 1)
							{
								neighbours.Add(m_tiles[x - 1, y - 1]);
								neighbours.Add(m_tiles[x + 1, y - 1]);
							}
							else
							{
								neighbours.Add(m_tiles[TileCountX - 2, y - 1]);
							}
						}
					}
				}

				m_tiles[x,y].SetNeighbours(neighbours.ToArray());
			}
		}
	}

	public void EndTurn ()
	{
		m_factionChanging = true;
	}

	public void AddTile(int x, int y, WorldTileScript tile)
	{
		if(m_tiles == null)
		{
			m_tiles = new WorldTileScript[TileCountX,TileCountY];
		}
		m_tiles[x, y] = tile;
	}

	public WorldTileScript GetTile(int x, int y)
	{
		return m_tiles[x, y];
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Cursor.lockState = CursorLockMode.Locked;

		if(m_factionChanging)
		{
			int previousFaction = m_currentFaction == 0 ? m_factions.Count - 1 : m_currentFaction - 1;

			if(m_factions[previousFaction].TurnEnded)
			{
				m_factions[m_currentFaction].EndTurn();
				m_currentFaction++;

				if(m_currentFaction == m_factions.Count)
				{
					m_currentFaction = 0;
					m_factionChanging = false;
				}
			}
		}
	}
}
