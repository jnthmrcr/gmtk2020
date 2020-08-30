﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaker : MonoBehaviour
{
	[SerializeField] int hitpointsStarting;
	[SerializeField] int hitPointsCurrent;
	[SerializeField] GameObject deathPrefab;
	[SerializeField] bool prefabWalkable;

	private void Start()
	{
		hitPointsCurrent = hitpointsStarting;
	}

	public void Damage(int points = 1)
	{
		hitPointsCurrent -= points;
		if (hitPointsCurrent <= 0)
		{
			// mark for death
			Die();
		}
	}

	void Die()
	{
		if (deathPrefab != null)
			Instantiate(deathPrefab, transform.position, deathPrefab.transform.rotation);
		else
		{
			GameManager.self.mainMap.grid.NodeFromWorldPosition(transform.position, Vector3.zero).walkable = prefabWalkable;
		}
		Destroy(gameObject);

	}
}