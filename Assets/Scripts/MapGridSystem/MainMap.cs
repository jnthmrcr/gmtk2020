using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainMap : MonoBehaviour
{
    LayerMask unwalkableMask;
    // player/enemy masks
    public MapGrid grid;
    public Vector2Int mapWorldSize;

	private void Start()
	{
		CreateMainMap();
	}

	/// <summary> creates map used by all pawns </summary>
	void CreateMainMap()
	{
        // create mapgrid based off mapworldsize
        grid = new MapGrid(mapWorldSize.x * 2 + 1, mapWorldSize.y * 2 + 1);
        transform.position = Vector3.zero; // just to be safe

		Vector3 worldBottomLeft = transform.position - Vector3.right * mapWorldSize.x - Vector3.forward * mapWorldSize.y;

		for (int x = 0; x < grid.sizeX; x++)
		{
			for (int y = 0; y < grid.sizeY; y++)
			{
				Vector3 worldpoint = worldBottomLeft + Vector3.right * x + Vector3.forward * y;
				bool walkable = !(Physics.CheckSphere(worldpoint, 0.4f, unwalkableMask));
				grid.nodes[x, y] = new MapNode(walkable, worldpoint, x, y); // fill nodes in grid with data
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(grid.sizeX, 1, grid.sizeY));
		
		Handles.Label(transform.position + Vector3.back * (mapWorldSize.y + 1), grid.nodes.Length.ToString());

		if (grid != null)
		{
			foreach (MapNode n in grid.nodes)
			{
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.8f);
			}
		}
	}
}
