using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class StaticHelpers {

	public static void Expand (int tileCountX, int tileCountY, List<WorldTileScript> tiles)
	{
		List<WorldTileScript> newTiles = new List<WorldTileScript>();
		foreach(WorldTileScript inputTile in tiles)
		{
			foreach(WorldTileScript newTile in inputTile.GetNeighbours())
			{
				if(!newTiles.Contains(newTile))
				{
					newTiles.Add(newTile);
	             }
			}
		}
		foreach(WorldTileScript tile in newTiles)
		{
			tiles.Add(tile);
		}
	}

	public static int Clamp( int value, int min, int max )
	{
		return (value < min) ? min : (value > max) ? max : value;
	}

	public static void FloodFill(int GridSizeX, int GridSizeY, List<List<WorldTileScript>> StartingLists, out List<List<WorldTileScript>> OutputLists)
	{
		OutputLists = new List<List<WorldTileScript>>();
		for(int i = 0; i < StartingLists.Count; i++)
		{
			OutputLists.Add(new List<WorldTileScript>());
		}

		bool[,] closed = new bool[GridSizeX, GridSizeY];

		List<List<WorldTileScript>> openLists = new List<List<WorldTileScript>>();
		List<List<WorldTileScript>> openListsToWriteTo = new List<List<WorldTileScript>>();

		foreach(List<WorldTileScript> list in StartingLists)
		{
			List<WorldTileScript> newList = new List<WorldTileScript>();

			for(int i = 0; i < list.Count; i++)
			{
				WorldTileScript tile = list[i];
				newList.Add(tile);
				closed[tile.x, tile.y] = true;
			}
			openLists.Add(newList);
			openListsToWriteTo.Add(new List<WorldTileScript>());
		}

		bool finished = false;

		while(!finished)
		{
			finished = true;

			bool openListsEmpty = false;

			while(!openListsEmpty)
			{
				openListsEmpty = true;
				for(int i = 0; i < openLists.Count; i++)
				{
					List<WorldTileScript> list = openLists[i];

					if(list.Count > 0)
					{
						finished = false;
						openListsEmpty = false;

						WorldTileScript tile = list[list.Count - 1];
						WorldTileScript[] neighbours = tile.GetNeighbours();
						list.RemoveAt(list.Count - 1);
						OutputLists[i].Add(tile);

						for(int j = 0; j < neighbours.Length; j++)
						{
							WorldTileScript neighbour = neighbours[j];
							if(!closed[neighbour.x, neighbour.y])
							{
								closed[neighbour.x, neighbour.y] = true;
								openListsToWriteTo[i].Add(neighbour);
							}
						}
					}
				}
			}

			List<List<WorldTileScript>> temp   = openLists;
			openLists = openListsToWriteTo;
			openListsToWriteTo = temp;

			foreach(List<WorldTileScript> list in openListsToWriteTo)
			{
				list.Clear();
			}
		}

		return;
	}
}
