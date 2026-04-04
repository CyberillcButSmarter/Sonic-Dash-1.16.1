using System;
using System.Collections;
using UnityEngine;

public class DCTime : MonoBehaviour
{
	private static DateTime s_currentTime;

	private static bool s_updateTime;

	public static bool TriedNTPTime { get; set; }

	public static DateTime GetCurrentTime()
	{
		DateTime result = s_currentTime;
		Language.Locale locale = Language.GetLocale();
		if (locale == Language.Locale.US)
		{
			result = result.AddHours(-8.0);
		}
		if (DCTimeValidation.TrustedTime)
		{
			return result.AddHours(DCServerTimeModification.HoursToAddToServerTime);
		}
		return result;
	}

	private void Awake()
	{
		s_currentTime = DateTime.UtcNow;
	}

	private void Start()
	{
		s_currentTime = DateTime.UtcNow;
		s_updateTime = true;
		TriedNTPTime = false;
		SetInternalTime();
	}

	private void Update()
	{
		if (s_updateTime)
		{
			s_currentTime = s_currentTime.AddTicks((long)(IndependantTimeDelta.Delta * 10000000f));
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			TriedNTPTime = false;
		}
		else
		{
			SetInternalTime();
		}
	}

	private void SetInternalTime()
	{
		s_updateTime = true;
		StartCoroutine(GetWWWServerTime());
	}

	private IEnumerator GetWWWServerTime()
	{
		FileDownloader request = new FileDownloader(FileDownloader.Files.SegaTime, false);
		yield return request.Loading;
		TriedNTPTime = true;
		long seconds;
		if (request.Error == null && long.TryParse(request.Text, out seconds))
		{
			s_currentTime = new DateTime(1970, 1, 1).AddSeconds(seconds);
			DCTimeValidation.TrustedTime = true;
		}
		else
		{
			s_currentTime = DateTime.UtcNow;
			DCTimeValidation.TrustedTime = false;
		}
	}
}
