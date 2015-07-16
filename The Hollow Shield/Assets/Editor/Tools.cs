using UnityEngine;
using UnityEditor;

public class MenuItems
{
	static Material redMaterial;
	static Material defualtMaterial;

	[MenuItem("Hollow Shield Tools/Create Hex Grid")]
	private static void NewMenuOption()
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
	
	[MenuItem("Hollow Shield Tools/UpdateTiles")]
	private static void UpdateTilesMenuOption()
	{
		Object[] tiles = GameObject.FindObjectsOfType<WorldTileScript>();

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
		}
	}
}
