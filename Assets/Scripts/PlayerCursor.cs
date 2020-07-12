using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
	[SerializeField] GameObject enemyRangeIndicator;
	[SerializeField] LayerMask enemyMask;

	private void Start()
	{
		Cursor.visible = false;
		enemyRangeIndicator.transform.parent = null;
	}

	private void Update()
	{
		Vector2 mouse = Input.mousePosition;
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, Camera.main.transform.position.y));
		transform.position = new Vector3(Mathf.Round(worldPoint.x), 0, Mathf.Round(worldPoint.z));

		if (Physics.CheckSphere(transform.position, 0.1f, enemyMask))
		{
			enemyRangeIndicator.SetActive(true);
			enemyRangeIndicator.transform.position = transform.position;
		}
		else
		{
			//enemyRangeIndicator.SetActive(false);
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		Cursor.visible = false;
	}
}
