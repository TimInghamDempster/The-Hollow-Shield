using UnityEngine;
using UnityEditor;

public class MenuItems
{
	static Material redMaterial;
	static Material defualtMaterial;

	[MenuItem("Hollow Shield Tools/Create Hex Grid")]
	private static void CreateGridMenuOption()
	{
		WorldGridScript grid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();

		float twoRootThree = 1.0f * Mathf.Sqrt(3.0f);
		for (int x = 0; x < grid.TileCountX; x++)
		{
			for (int y = 0; y < grid.TileCountY; y++)
			{
				float yPos = y * twoRootThree;
				
				if(x % 2 == 0)
				{
					yPos += twoRootThree / 2.0f;
				}

				GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(Resources.Load("WorldTile"));

				go.transform.position = new Vector3(x * 1.5f, 0, yPos);

				WorldTileScript tileScript = go.GetComponent<WorldTileScript>();

				tileScript.x = x;
				tileScript.y = y;
			}
		}
	}
	
	[MenuItem("Hollow Shield Tools/Reset Grid")]
	private static void ResetGridMenuOption()
	{
		WorldGridScript grid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		grid.DiscoverAndAddTiles();
		grid.ClearTiles();
	}

	[MenuItem("Hollow Shield Tools/Generate Coastline")]
	private static void GenerateCoastMenuOption()
	{
		WorldGridScript grid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		grid.DiscoverAndAddTiles();
		grid.PostInitialise();
		grid.GenerateCoastline();
	}

	[MenuItem("Hollow Shield Tools/Simulate Tectonics")]
	private static void SimulateTectonicsMenuOption()
	{
		WorldGridScript grid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		grid.DiscoverAndAddTiles();
		grid.PostInitialise();
		grid.DoTectonics();
	}

	[MenuItem("Hollow Shield Tools/Simulate Geology")]
	private static void SimulateGeologyMenuOption()
	{
		WorldGridScript grid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		grid.DiscoverAndAddTiles();
		grid.PostInitialise();
		grid.DoGeology();
	}

	[MenuItem("Hollow Shield Tools/Simulate Growth")]
	private static void SimulateRiversMenuOption()
	{
		WorldGridScript grid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		grid.DiscoverAndAddTiles();
		grid.PostInitialise();
		grid.AssignTileTypes();
	}

	[MenuItem("Hollow Shield Tools/UpdateTiles")]
	private static void UpdateTilesMenuOption()
	{
		Object[] tiles = GameObject.FindObjectsOfType<WorldTileScript>();
		WorldGridScript worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();

		foreach(object tile in tiles)
		{
			WorldTileScript castTile = (WorldTileScript)tile;

			if(!castTile.IsPassable)
			{
				if(defualtMaterial == null)
				{
					defualtMaterial = castTile.gameObject.GetComponent<Renderer>().sharedMaterial;
				}

				if(redMaterial == null)
				{
					redMaterial = new Material(defualtMaterial);
					redMaterial.color = Color.red;
				}

				castTile.gameObject.GetComponent<Renderer>().material = redMaterial;
			}
			else
			{
				if(defualtMaterial == null)
				{
					defualtMaterial = castTile.gameObject.GetComponent<Renderer>().sharedMaterial;
				}
				
				if(redMaterial == null)
				{
					redMaterial = new Material(defualtMaterial);
					redMaterial.color = Color.red;
				}

				castTile.gameObject.GetComponent<Renderer>().material = defualtMaterial;
			}

			switch (castTile.Type)
			{
			case TileTypes.Grass:
				{
					MeshFilter meshFilter = castTile.gameObject.GetComponent<MeshFilter>();
					meshFilter.mesh = worldGrid.GrassTileMesh;
					MeshRenderer meshRenderer = castTile.gameObject.GetComponent<MeshRenderer>();
					meshRenderer.material = worldGrid.GrassTileMaterial;
				}break;
			case TileTypes.Castle:
				{
					MeshFilter meshFilter = castTile.gameObject.GetComponent<MeshFilter>();
					meshFilter.mesh = castTile.Faction.FactionCastleMesh;
					MeshRenderer meshRenderer = castTile.gameObject.GetComponent<MeshRenderer>();
					meshRenderer.material = castTile.Faction.FactionCastleMaterial;
				}break;
			case TileTypes.Water:
				{
					MeshFilter meshFilter = castTile.gameObject.GetComponent<MeshFilter>();
					meshFilter.mesh = worldGrid.WaterTileMesh;
					MeshRenderer meshRenderer = castTile.gameObject.GetComponent<MeshRenderer>();
					meshRenderer.material = worldGrid.WaterTileMaterial;
				}break;
			case TileTypes.Sand:
				{
					castTile.SetMesh(worldGrid.SandTileMesh, worldGrid.SandTileMaterial);
				}break;
			}
		}
	}
}
