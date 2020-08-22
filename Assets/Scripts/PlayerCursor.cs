﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerCursor : MonoBehaviour
{
	[SerializeField] GameObject enemyRangeIndicator;
	[SerializeField] GameObject playerTargettingPrefab;
	[SerializeField] LayerMask enemyMask;
	[SerializeField] LayerMask friendlyMask;
	[SerializeField] LayerMask indicatorMask;
	[SerializeField] float curorLerpSpeed = 20f;
	[SerializeField] GameObject largeDiamondCursor;
	[SerializeField] GameObject smallDiamondCursor;
	Pawn cursorPawn;

	Vector3 goalPoint;

	GameObject[] targetIndicatorCache;

	GameManager gm;
	PlayerController playerController;

	private void Awake()
	{
		gm = GetComponentInParent<GameManager>();
		playerController = GetComponentInParent<PlayerController>();
		cursorPawn = GetComponent<Pawn>();
	}

	private void Start()
	{
		Cursor.visible = false;
		enemyRangeIndicator.transform.parent = null;

		targetIndicatorCache = new GameObject[41];
		for (int i = 0; i < targetIndicatorCache.Length; i++)
		{
			targetIndicatorCache[i] = Instantiate(playerTargettingPrefab, Vector3.zero, Quaternion.Euler(90f, 0f, 0f), transform);
			targetIndicatorCache[i].SetActive(false);
		}

	}

	private void Update()
	{
		Vector2 mouse = Input.mousePosition;
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, Camera.main.transform.position.y));
		goalPoint = new Vector3(Mathf.Round(worldPoint.x), 0, Mathf.Round(worldPoint.z));
		float goalY = 0;

		bool cursorCollision = false;

		Collider[] colliders = Physics.OverlapSphere(goalPoint, 0.1f, friendlyMask | enemyMask | indicatorMask);
		if (colliders.Length > 0)
		{
			cursorCollision = true;
			SetLargeCursor(true);
			goalY = 1f;
			if (gm.currentTurnPhase == GameManager.gamePhase.playerTurn)
			{
				if (Input.GetMouseButtonDown(1))
				{ // rmb, selects mech pawns during player turn
					if (1 << colliders[0].gameObject.layer == friendlyMask)
						colliders[0].GetComponent<PlayerMech>().SetActivePawn();
				}
				else if (Input.GetMouseButtonDown(0))
				{ // lmb, selects indicator/confirms action
					//if (gm.currentActionMode == GameManager.actionMode.move)
					//{
					if (1 << colliders[0].gameObject.layer == indicatorMask)
					{
						playerController.activeMech.Move(goalPoint);
					}
				}
				// else if (gm.currentActionMode == GameManager.actionMode.attack)
				// {
				// 	// idk fuck, grab the gameobject at this position and get the damage sys for it?
				// 	if (1 << colliders[0].gameObject.layer == enemyMask)
				// 	{
				// 		//playerController.activeMech.Attack(goalPoint);
				// 	}
				// }
				else
				{ // if we're doing nothing
					if (1 << colliders[0].gameObject.layer == enemyMask)
					{ // and we're over an enemy
						enemyRangeIndicator.SetActive(true);
						enemyRangeIndicator.transform.position = goalPoint;
					}
				}
			}
			else { }
		}
		else
		{
			SetLargeCursor(false);
		}

		if (Input.GetMouseButton(2))
		{
			DrawTargetting();
		}
		if (Input.GetMouseButtonUp(2))
		{
			ClearTargetting();
		}

		// now we set cursor position
		if (cursorCollision)
			transform.position = Vector3.Lerp(transform.position, goalPoint + goalY * Vector3.up, Time.deltaTime * curorLerpSpeed);
		else
			transform.position = worldPoint + goalY * Vector3.up;
	}

	private void OnApplicationFocus(bool focus)
	{
		Cursor.visible = false;
	}

	public void ShowCursor(bool isVisisble)
	{
		if (isVisisble)
		{
			gameObject.SetActive(true);
		}
		else
		{
			enemyRangeIndicator.SetActive(false);
			gameObject.SetActive(false);
		}
	}

	void SetLargeCursor(bool isLarge)
	{
		largeDiamondCursor.SetActive(isLarge);
		smallDiamondCursor.SetActive(!isLarge);
	}

	void DrawTargetting()
	{
		//todo: this method should be more easily accessed somehow
		cursorPawn.FindTargettableNodes(transform.position, 4);

		int exitindex = cursorPawn.targetablePoints.Count;
		for (int i = 0; i < targetIndicatorCache.Length; i++)
		{
			if (i < exitindex)
			{
				targetIndicatorCache[i].transform.position = cursorPawn.targetablePoints[i] + Vector3.up * 1.4f;
				targetIndicatorCache[i].SetActive(true);
			}
			else
			{
				targetIndicatorCache[i].SetActive(false);
			}
		}
	}

	void ClearTargetting()
	{
		foreach (GameObject t in targetIndicatorCache)
		{
			t.SetActive(false);
		}
	}
}