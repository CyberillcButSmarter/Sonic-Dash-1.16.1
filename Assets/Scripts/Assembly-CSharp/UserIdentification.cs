using System;
using System.Collections;
using UnityEngine;

public class UserIdentification : MonoBehaviour
{
	private static string s_playerId;

	public static string Current
	{
		get
		{
			return (s_playerId != null) ? s_playerId : "id_not_received";
		}
	}

	public static string Generate
	{
		get
		{
			return GenerateUniqueIdentifier();
		}
	}

	private void Start()
	{
		s_playerId = CurrentDeviceIdentifier();
		if (s_playerId == null)
		{
			StartCoroutine(QueryPlayerIdOverTime());
		}
	}

	private static string GenerateUniqueIdentifier()
	{
		string current = Current;
		string text = DateTime.UtcNow.ToUniversalTime().ToString();
		text = text.Replace('/', '-');
		text = text.Replace(' ', '-');
		text = text.Replace(':', '-');
		string text2 = Time.realtimeSinceStartup.ToString();
		text2 = text2.Replace('.', '-');
		string text3 = Application.platform.ToString().Substring(0, 2);
		string text4 = Language.CultureInfoIDs[(int)Language.GetLanguage()];
		return string.Format("{0}-{1}-{2}-{3}-{4}", current, text, text2, text3, text4).ToUpperInvariant();
	}

	private IEnumerator QueryPlayerIdOverTime()
	{
		do
		{
			yield return new WaitForSeconds(1f);
			s_playerId = CurrentDeviceIdentifier();
		}
		while (s_playerId == null);
	}

	private static string CurrentDeviceIdentifier()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.hardlightstudio.dev.sonicdash.plugin.SLGlobal");
		return androidJavaClass.CallStatic<string>("GetDeviceID", new object[0]);
	}
}
