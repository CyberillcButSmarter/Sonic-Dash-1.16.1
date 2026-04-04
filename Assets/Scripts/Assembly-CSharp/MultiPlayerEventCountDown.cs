using System;
using System.Collections;
using UnityEngine;

public class MultiPlayerEventCountDown : MonoBehaviour
{
	public string textId;

	public bool mainMenu;

	private string precedingText = string.Empty;

	private bool restartCountDown;

	private void Start()
	{
		precedingText = LanguageStrings.First.GetString(textId);
		if (mainMenu)
		{
			EventDispatch.RegisterInterest("MainMenuActive", this);
		}
		RunCountDown();
	}

	public void RunCountDown()
	{
		if (GrantooSpecialEvent.GetInstance().IsEventActive())
		{
			StartCoroutine(UpdateCountDown());
		}
	}

	private IEnumerator UpdateCountDown()
	{
		while (true)
		{
			TimeSpan remaining = GrantooSpecialEvent.GetInstance().GetRemainingTime();
			if (remaining.TotalHours <= 24.0)
			{
				GetComponent<UILabel>().text = LanguageStrings.First.GetString("GC_FINAL_DAY");
			}
			else
			{
				GetComponent<UILabel>().text = string.Format(precedingText, remaining.Days * 24 + remaining.Hours + ":" + remaining.Minutes.ToString("00") + ":" + remaining.Seconds.ToString("00"));
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public void Update()
	{
		if (restartCountDown)
		{
			restartCountDown = false;
			RunCountDown();
		}
	}

	public void Event_MainMenuActive()
	{
		restartCountDown = true;
	}
}
