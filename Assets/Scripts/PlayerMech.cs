using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMech : Pawn
{
	private void Update()
	{
		GetPMap(transform.position, moveDist);
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


		foreach (GridNode n in navigableNodes)
		{
			Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.8f);
		}
	}
}
