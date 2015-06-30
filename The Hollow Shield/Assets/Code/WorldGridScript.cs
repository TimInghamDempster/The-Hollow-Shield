using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGridScript : MonoBehaviour {

	public int TileCountX;
	public int TileCountY;

	WorldTileScript[,] tiles;

	public void Create()
	{
		tiles = new WorldTileScript[TileCountX, TileCountY];
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
					neighbours.Add(tiles[x,1]);
				}
				else
				{
					if(y < TileCountY  - 1)
					{
						neighbours.Add(tiles[x, y - 1]);
						neighbours.Add(tiles[x, y + 1]);
					}
					else
					{
						neighbours.Add(tiles[x,TileCountY - 2]);
					}
				}
				if(x == 0)
				{
					neighbours.Add(tiles[1,y]);
				}
				else
				{
					if(x < TileCountX  - 1)
					{
						neighbours.Add(tiles[x - 1, y]);
						neighbours.Add(tiles[x + 1, y]);
					}
					else
					{
						neighbours.Add(tiles[TileCountX - 2, y]);
					}
				}
				if(x % 2 == 0)
				{
					if(y < TileCountY  - 1)
					{
						if(x == 0)
						{
							neighbours.Add(tiles[1,y + 1]);
						}
						else
						{
							if(x < TileCountX  - 1)
							{
								neighbours.Add(tiles[x - 1, y + 1]);
								neighbours.Add(tiles[x + 1, y + 1]);
							}
							else
							{
								neighbours.Add(tiles[TileCountX - 2, y + 1]);
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
							neighbours.Add(tiles[1,y - 1]);
						}
						else
						{
							if(x < TileCountX  - 1)
							{
								neighbours.Add(tiles[x - 1, y - 1]);
								neighbours.Add(tiles[x + 1, y - 1]);
							}
							else
							{
								neighbours.Add(tiles[TileCountX - 2, y - 1]);
							}
						}
					}
				}

				tiles[x,y].SetNeighbours(neighbours.ToArray());
			}
		}
	}

	public void TileClicked(WorldTileScript tile)
	{
		foreach(WorldTileScript gridTile in tiles)
		{
			if(gridTile != tile)
			{
				gridTile.Unclick();
			}
		}
	}

	public void AddTile(int x, int y, WorldTileScript tile)
	{
		tiles[x, y] = tile;
	}

	// Use this for initialization
	void Start () {
		tiles = new WorldTileScript[TileCountX,TileCountY];
	}
	
	// Update is called once per frame
	void Update () {
		Cursor.lockState = CursorLockMode.Locked;
	}
}
