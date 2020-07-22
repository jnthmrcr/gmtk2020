using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMech : Pawn
{
	private void Update()
	{
		//if (Input.GetKeyDown(KeyCode.Space))
		//{
			GetPersonalMap(transform.position, moveDist);
		//}
	}

	private void OnDrawGizmos()
	{
		// draw personal map size
		//Gizmos.DrawWireCube(transform.position, new Vector3(personalMap.sizeX, 1, personalMap.sizeY));

		Gizmos.color = Color.cyan;
		MapNode startNode = mainMap.grid.NodeFromWorldPosition(transform.position);
		print(startNode.gridX + ", " + startNode.gridY);

		Gizmos.DrawCube(startNode.worldPosition, Vector3.one);

		print("startnotede " + startNode.gridX + " " + startNode.gridY);


		//Handles.Label(transform.position + Vector3.back * (mapWorldSize.y + 1), grid.nodes.Length.ToString());

		if (personalMap != null)
		{
			foreach (MapNode n in personalMap.nodes)
			{
				Color nodecolor = Color.red; // not walkable not navigable
				if (n.walkable)
				{
					// if navigable
					// green
					// else
					// white
					nodecolor = Color.white;
				}

				Gizmos.color = nodecolor;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.8f);
			}
		}
	}
}
