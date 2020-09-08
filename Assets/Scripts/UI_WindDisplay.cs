using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WindDisplay : MonoBehaviour
{
	[SerializeField] RectTransform[] nodeSmall;
	[SerializeField] RectTransform nodeLarge;
	[SerializeField] RectTransform connectorLine;

	RectTransform myTrans;

	private void Awake()
	{
		myTrans = GetComponent<RectTransform>();
	}

	public void SetDisplay(int x, int y)
	{
		nodeSmall[0].anchoredPosition = Vector2Int.zero;
		int index = 1;

		for (int i = 1; i < Mathf.Abs(x) + 1; i++)
		{
			nodeSmall[index].gameObject.SetActive(true);
			nodeSmall[index].anchoredPosition = new Vector2(Mathf.Sign(x) * i * 20f, 0);
			index++;
		}

		for (int i = 1; i < Mathf.Abs(y); i++)
		{
			nodeSmall[index].gameObject.SetActive(true);
			nodeSmall[index].anchoredPosition = new Vector2(x * 20f, Mathf.Sign(y) * i * 20f);
			index++;
		}

		while (index < 4)
		{
			nodeSmall[index].gameObject.SetActive(false);
			index++;
		}

		nodeLarge.anchoredPosition = new Vector2(x * 20f, y * 20f);

		connectorLine.transform.localScale = new Vector3(Vector2.Distance(Vector2.zero, new Vector2(x, y)), 1, 1);
		connectorLine.anchoredPosition = new Vector2(x * 10f, y * 10f);
		connectorLine.rotation = Quaternion.LookRotation(new Vector3(x, 0, y), Vector3.up) * Quaternion.Euler(90, 90, 0); //Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Acos((float) x / (float) y));
	}
}