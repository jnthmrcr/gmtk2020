using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMech : Pawn
{
	private void Update()
	{
		//GetPMap(transform.position, moveDist);
		GetPersonalMap(transform.position, moveDist);
	}

	private void OnDrawGizmos()
	{

		//if (pMap.map != null)
		//{
		//	foreach (GridNode n in pMap.map)
		//	{
		//		Gizmos.color = (n.walkable) ? Color.white : Color.red;
		//		Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.8f);
		//	}
		//}

		Gizmos.DrawWireCube(transform.position, new Vector3(personalMap.sizeX, 1, personalMap.sizeY));

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
