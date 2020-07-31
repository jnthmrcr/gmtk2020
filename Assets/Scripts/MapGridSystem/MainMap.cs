using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class MainMap : MonoBehaviour
{
    [SerializeField] LayerMask unwalkableMask;
    // player/enemy masks
    public MapGrid grid;
    public Vector2Int mapWorldSize;

	private void Awake()
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
		//MapNode node = grid.NodeFromWorldPosition(Vector3.zero);
		//Gizmos.DrawSphere(node.worldPosition, 1f);
		//node = grid.NodeFromWorldPosition(Vector3.forward * 5);
		//Gizmos.DrawSphere(node.worldPosition, 1f);
		//node = grid.NodeFromWorldPosition(Vector3.right * 5);
		//Gizmos.DrawSphere(node.worldPosition, 1f);
		//node = grid.NodeFromWorldPosition(new Vector3(5f, 0f, 5f));
		//Gizmos.DrawSphere(node.worldPosition, 1f);

		//node = grid.NodeFromWorldPosition(Vector3.forward * -3);
		//Gizmos.DrawSphere(node.worldPosition, 1f);
		//node = grid.NodeFromWorldPosition(Vector3.right * -3);
		//Gizmos.DrawSphere(node.worldPosition, 1f);
		//node = grid.NodeFromWorldPosition(new Vector3(-3f, 0f, -3f));
		//Gizmos.DrawSphere(node.worldPosition, 1f);

		//node = grid.NodeFromWorldPosition(new Vector3(6f, 0f, 8f));
		//Gizmos.DrawSphere(node.worldPosition, 1f);
		//node = grid.NodeFromWorldPosition(new Vector3(-6f, 0f, -8f));
		//Gizmos.DrawSphere(node.worldPosition, 1f);

		Gizmos.DrawWireCube(transform.position, new Vector3(grid.sizeX, 1, grid.sizeY));
		
		Handles.Label(transform.position + Vector3.back * (mapWorldSize.y + 1), grid.nodes.Length.ToString());

		if (grid != null)
		{
			foreach (MapNode n in grid.nodes)
			{
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.2f);
				//Handles.Label(n.worldPosition, n.gridY.ToString());
			}
		}
	}
}
