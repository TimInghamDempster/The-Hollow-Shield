using UnityEngine;
using System.Collections;

public class WorldGridScript : MonoBehaviour {

	public int TileCountX;
	public int TileCountY;

	WorldTileScript[,] tiles;

	public void Create()
	{
		tiles = new WorldTileScript[TileCountX, TileCountY];
	}

	public void AddTile(int x, int y, WorldTileScript tile)
	{
		tiles[x, y] = tile;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
