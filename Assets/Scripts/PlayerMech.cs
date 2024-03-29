﻿using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMech : Pawn
{
	public PlayerController player;
	[SerializeField] GameObject indicatorprefab;
	[SerializeField] GameObject targettingPrefab;
	[SerializeField] TextMeshPro mechName;

	GameObject[] indicatorCache;
	GameObject[] targettingCache;

	protected override void Start()
	{
		base.Start();

		indicatorCache = new GameObject[41];
		for (int i = 0; i < indicatorCache.Length; i++)
		{
			indicatorCache[i] = Instantiate(indicatorprefab, Vector3.zero, Quaternion.Euler(90f, 45f, 0f));
			indicatorCache[i].SetActive(false);
			indicatorCache[i].hideFlags = HideFlags.HideInHierarchy;
		}

		targettingCache = new GameObject[24]; // should be enough?
		for (int i = 0; i < targettingCache.Length; i++)
		{
			targettingCache[i] = Instantiate(targettingPrefab, Vector3.zero, Quaternion.Euler(90f, 0f, 0f));
			targettingCache[i].SetActive(false);
			targettingCache[i].hideFlags = HideFlags.HideInHierarchy;
		}
	}

	private void Update()
	{
		//if (Input.GetKeyDown(KeyCode.Space))
		//{
		//GetPersonalMap(transform.position, moveDist);
		//FindNavigableNodes(transform.position, moveDist);
		//}

		//FindTargettableNodes(transform.position, attackDist, wind.x, wind.y);
	}

	private void OnDrawGizmos()
	{
		// draw personal map size
		//Gizmos.DrawWireCube(transform.position, new Vector3(personalMap.sizeX, 1, personalMap.sizeY));

		Gizmos.color = Color.cyan;
		MapNode startNode = GameManager.self.mainMap.grid.NodeFromWorldPosition(transform.position, Vector3.zero);
		//print(startNode.gridX + ", " + startNode.gridY);

		Gizmos.DrawCube(startNode.worldPosition, Vector3.one);

		//print("startnotede " + startNode.gridX + " " + startNode.gridY);

		//Handles.Label(transform.position + Vector3.back * (mapWorldSize.y + 1), grid.nodes.Length.ToString());

		if (personalMap != null)
		{
			//foreach (MapNode n in personalMap.nodes)
			//{
			//	Color nodecolor = Color.red; // not walkable not navigable
			//	if (n.walkable)
			//	{
			//		// if navigable
			//		// green
			//		// else
			//		// white
			//		nodecolor = Color.white;
			//	}

			//	Gizmos.color = nodecolor;
			//	Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.8f);
			//}

			foreach (MapNode n in navigableNodes)
			{
				//Gizmos.DrawCube(n.worldPosition, Vector3.one * 0.8f);
				// Handles.Label(n.worldPosition, n.cost.ToString());
			}

		}

		// targetable point test
		// foreach (Vector2Int p in targetablePoints)
		// {
		// 	Gizmos.DrawCube(p.toV3(), Vector3.one * 0.8f);
		// }
	}

	public override void PhaseInit(int windX, int windY)
	{
		base.PhaseInit(windX, windY);

		mechName.text = "hubert";
	}

	public override void SetActivePawn()
	{
		base.SetActivePawn();

		// THERE CAN ONLY BE ONE
		player.SetActiveMech(this);

		ShowMovementInidicators();
		ShowAttackIndicators();

		// if (moveIfTrue)
		// {
		// 	ShowMovementInidicators();
		// }
		// else
		// {
		// 	ShowAttackIndicators();
		// }
	}

	public void ShowMovementInidicators()
	{
		Color bc = indicatorprefab.GetComponent<MeshRenderer>().sharedMaterial.GetColor("_BaseColor");
		MeshRenderer mr;
		float ratio;

		int exitindex = navigableNodes.Count;
		for (int i = 0; i < indicatorCache.Length; i++)
		{
			if (i < exitindex)
			{ // do stuff
				indicatorCache[i].transform.position = navigableNodes[i].worldPosition + Vector3.up * 1.4f;
				indicatorCache[i].SetActive(true);

				mr = indicatorCache[i].GetComponent<MeshRenderer>();
				ratio = (float) navigableNodes[i].cost / (float) moveDist;

				mr.material.SetColor("_BaseColor", new Color(bc.r, bc.g, bc.b, Mathf.Lerp(.025f, 0.07f, Mathf.Pow(ratio, 3f))));
				indicatorCache[i].transform.localScale = Vector3.one * Mathf.Lerp(0.3f, 0.6f, Mathf.Pow(ratio, 2f));
			}
			else
			{ // do not do thing
				indicatorCache[i].SetActive(false);
			}
		}
	}

	public void ShowAttackIndicators()
	{
		FindTargettableNodes(transform.position, attackDist, 0, 0);

		// highlight nodes that can be attacked
		int exitindex = targetableDamageTakerPoints.Count;
		for (int i = 0; i < targettingCache.Length; i++)
		{
			if (i < exitindex)
			{
				targettingCache[i].transform.position = targetableDamageTakerPoints[i] + Vector3.up * 1.4f;
				targettingCache[i].SetActive(true);
			}
			else
			{
				targettingCache[i].SetActive(false);
			}
		}
	}

	public void DeselectMech()
	{
		foreach (GameObject t in indicatorCache)
		{
			t.SetActive(false);
		}

		foreach (GameObject t in targettingCache)
		{
			t.SetActive(false);
		}
	}

	IEnumerator SetActiveIndicatorAnim()
	{
		return null;
	}
}