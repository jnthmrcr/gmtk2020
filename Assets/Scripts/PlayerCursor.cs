using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerCursor : MonoBehaviour
{
	[SerializeField] PlayerMech player;
	[SerializeField] GameObject enemyRangeIndicator;
	[SerializeField] GameObject playerTargettingPrefab;
	[SerializeField] LayerMask enemyMask;
	[SerializeField] LayerMask friendlyMask;
	[SerializeField] float curorLerpSpeed = 20f;
	Vector3 goalPoint;

	GameObject[] targetIndicatorCache;

	GameManager gm;

	private void Awake()
	{
		gm = GetComponentInParent<GameManager>();
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
		transform.position = Vector3.Lerp(transform.position, goalPoint, Time.deltaTime * curorLerpSpeed);

		if (Physics.CheckSphere(goalPoint, 0.1f, enemyMask))
		{
			enemyRangeIndicator.SetActive(true);
			enemyRangeIndicator.transform.position = goalPoint;
		}
		else
		{
			//enemyRangeIndicator.SetActive(false);
		}

		if (Input.GetMouseButton(2))
		{
			DrawTargetting();
		}
		if (Input.GetMouseButtonUp(2))
		{
			ClearTargetting();
		}

		if (gm.currentTurnPhase == GameManager.gamePhase.playerTurn)
		{
			// time to select something
			// find nearest player
			if (Input.GetMouseButtonDown(1))
			{
				Collider[] colliders = Physics.OverlapSphere(goalPoint, 0.1f, friendlyMask);
				if (colliders.Length > 0)
				{
					colliders[0].GetComponent<PlayerMech>().SetActiveMech();
				}
			}
		}
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

	void DrawTargetting()
	{
		player.FindTargettableNodes(transform.position, 4);

		int exitindex = player.targetablePoints.Count;
		for (int i = 0; i < targetIndicatorCache.Length; i++)
		{
			if (i < exitindex)
			{
				targetIndicatorCache[i].transform.position = player.targetablePoints[i].toV3() + Vector3.up * 1.4f;
				targetIndicatorCache[i].SetActive(true);
			} else
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
