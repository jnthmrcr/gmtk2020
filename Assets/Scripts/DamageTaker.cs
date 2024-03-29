﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaker : MonoBehaviour
{
	[SerializeField] int hitpointsStarting;
	[SerializeField] int hitPointsCurrent;
	[SerializeField] GameObject deathPrefab;

	Pawn pawn;

	private void Awake()
	{
		pawn = GetComponent<Pawn>();
	}

	private void Start()
	{
		hitPointsCurrent = hitpointsStarting;
		if (pawn != null)
			pawn.SetHPtext(hitPointsCurrent);
	}

	public void Damage(int points = 1)
	{
		GameManager.self.PawnActionFinished();

		hitPointsCurrent -= points;

		if (pawn != null)
			pawn.SetHPtext(hitPointsCurrent);

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

		GameManager.self.mainMap.RescanNodeAtPointAfterWait(transform.position);
		Destroy(gameObject);
	}
}