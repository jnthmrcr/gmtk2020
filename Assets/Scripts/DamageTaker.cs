﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaker : MonoBehaviour
{
	[SerializeField] int hitpointsStarting;
	[SerializeField] int hitPointsCurrent;
	[SerializeField] GameObject deathPrefab;

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
		Instantiate(deathPrefab, transform.position, deathPrefab.transform.rotation);
		Destroy(gameObject);
	}
}