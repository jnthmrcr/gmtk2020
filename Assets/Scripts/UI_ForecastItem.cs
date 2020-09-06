using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ForecastItem : MonoBehaviour
{
	[SerializeField] Text turnDisplay;
	[SerializeField] Text actionDisplay;
	[SerializeField] UI_WindDisplay windDisplay;

	private void Awake()
	{
		windDisplay = GetComponentInChildren<UI_WindDisplay>();
	}

	private void Start()
	{
		SetDisplay(2, 2, 2, 2);
	}

	public void SetDisplay(int turnCount, int actionCount, int windX, int windY)
	{
		turnDisplay.text = turnCount.ToString();
		actionDisplay.text = actionCount.ToString();
		windDisplay.SetDisplay(windX, windY);
	}
}