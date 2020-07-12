using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : MonoBehaviour
{
	private void Start()
	{
		Cursor.visible = false;
	}

	private void Update()
	{
		print(Input.mousePosition);
		Vector2 mouse = Input.mousePosition;
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, Camera.main.transform.position.y));
		transform.position = new Vector3(Mathf.Round(worldPoint.x), 0, Mathf.Round(worldPoint.z));
	}
}
