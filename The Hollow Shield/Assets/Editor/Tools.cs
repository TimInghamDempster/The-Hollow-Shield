using UnityEngine;
using UnityEditor;

public class MenuItems
{
	[MenuItem("Hollow Shield Tools/Create Hex Grid")]
	private static void NewMenuOption()
	{
		WorldGridScript grid = GameObject.Find("WorldGrid").GetComponent<WorldGridScript>();
		grid.Create();

		float twoRootThree = 1.0f * Mathf.Sqrt(3.0f);
		for (int x = 0; x < grid.TileCountX; x++)
		{
			for (int y = 0; y < grid.TileCountY; y++)
			{
				GameObject go = new GameObject("HexTile" + x + y);

				float yPos = y * twoRootThree;

				if(x % 2 == 0)
				{
					yPos += twoRootThree / 2.0f;
				}

				go.transform.position = new Vector3(x * 1.5f, 0, yPos);

				WorldTileScript tile = go.AddComponent<WorldTileScript>();
				
				grid.AddTile(x, y, tile);
			}
		}
	}
}
