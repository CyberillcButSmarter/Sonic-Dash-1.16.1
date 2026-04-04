using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class FileDownloader
{
	public enum Files
	{
		SegaTime = 0,
		GC6 = 1,
		OfferState = 2,
		ABConfig = 3,
		FeatureStateAB = 4,
		FeatureStateDefault = 5,
		ABTestingAB = 6,
		ABTestingDefault = 7,
		StoreModifierAB = 8,
		StoreModifierDefault = 9
	}

	private const float DefaultTimeout = 15f;

	private const string DefaultState = "state_default.lson";

	private const string DefaultABTesting = "abtesting_default.lson";

	private const string DefaultStoreModifier = "storemodifier_default.lson";

	private const string FileIDString = "FileIdentifier:";

	private const string FileFlavour = "Cupcake";

	// All legacy endpoints except tp.sega.com are dead, so we rebuild URLs off that host.
	// If you're running a replacement backend, point this at your server OR map tp.sega.com in
	// your hosts file to your server IP so the game keeps the same URL.
	private const string ServerBaseUrl = "http://tp.sega.com";

	private const string SegaTimePath = "/";

	// AB/Feature/Store endpoints expected by the game. Host these on your replacement server.
	private const string OfferStatePath = "/SonicDash/GetOffer.hl";

	private const string ABConfigPath = "/SonicDash/GetConfig.hl";

	private const string FeatureStatePath = "/ab/production/state_default.lson";

	private const string ABTestingPath = "/ab/production/abtesting_default.lson";

	private const string StoreModifierPath = "/ab/production/storemodifier_default.lson";

	// Global Challenge (GC6) is disabled by default.
	// GC6 expects a plain integer in gc6progress.txt representing the community's total points.
	// Re-enable it by setting EnableGlobalChallengeDownloads = true and hosting:
	//   /global+challenges/production/gc6progress.txt
	// on the same server (or update GlobalChallengePath).
	private const bool EnableGlobalChallengeDownloads = false;

	private const string GlobalChallengePath = "/global+challenges/production/gc6progress.txt";

	private static readonly string[] s_urls = new string[10]
	{
		BuildUrl(SegaTimePath),
		BuildUrl(GlobalChallengePath),
		BuildUrl(OfferStatePath),
		BuildUrl(ABConfigPath),
		BuildUrl(FeatureStatePath),
		BuildUrl(FeatureStatePath),
		BuildUrl(ABTestingPath),
		BuildUrl(ABTestingPath),
		BuildUrl(StoreModifierPath),
		BuildUrl(StoreModifierPath)
	};

	private static GameObject s_auxObject;

	private static FileDownloaderBehaviour s_auxBehaviour;

	public string Text { get; private set; }

	public string Error { get; private set; }

	public Coroutine Loading { get; private set; }

	public FileDownloader(Files file, bool keepAndUseLocalCopy)
	{
		if (s_auxBehaviour == null)
		{
			s_auxObject = new GameObject();
			s_auxObject.name = "Aux Object For FileDownloader";
			s_auxBehaviour = s_auxObject.AddComponent<FileDownloaderBehaviour>();
			UnityEngine.Object.DontDestroyOnLoad(s_auxObject);
		}
		Loading = s_auxBehaviour.StartCoroutine(DownloadServerFile(file, keepAndUseLocalCopy));
	}

	public static void TweakABTestingURLs(Files file, string fileName)
	{
		string text = s_urls[(int)file];
		switch (file)
		{
		case Files.ABTestingAB:
			text = text.Replace("abtesting_default.lson", fileName);
			break;
		case Files.StoreModifierAB:
			text = text.Replace("storemodifier_default.lson", fileName);
			break;
		case Files.FeatureStateAB:
			text = text.Replace("state_default.lson", fileName);
			break;
		}
		s_urls[(int)file] = text;
	}

	private IEnumerator DownloadServerFile(Files file, bool loadLocal)
	{
		string finalUrl = GetFinalURL(file);
		if (string.IsNullOrEmpty(finalUrl))
		{
			Error = "FileDownloader: download disabled for " + file;
			Text = null;
			if (loadLocal)
			{
				LoadLocalCopy(file);
			}
			GameAnalytics.NotifyFileDownload(file, 0f);
			yield break;
		}
		WWW www = new WWW(finalUrl);
		float timeTaken = 0f;
		bool timeOut = false;
		while (!timeOut && !www.isDone)
		{
			timeTaken += Time.deltaTime;
			if (timeTaken > 15f)
			{
				timeOut = true;
			}
			yield return null;
		}
		Error = ((!timeOut) ? www.error : "FileDownloader timed out");
		if (!timeOut && www.error == null)
		{
			Text = www.text;
			Error = null;
			if (loadLocal)
			{
				SaveLocalCopy(file);
			}
		}
		else
		{
			Text = null;
			if (loadLocal)
			{
				LoadLocalCopy(file);
			}
		}
		GameAnalytics.NotifyFileDownload(file, timeTaken);
	}

	private string GetFinalURL(Files file)
	{
		if (file == Files.GC6 && !EnableGlobalChallengeDownloads)
		{
			// GC6 (Global Challenge) is intentionally disabled. See notes above to re-enable.
			return string.Empty;
		}
		string text = s_urls[(int)file];
		int num = UnityEngine.Random.Range(0, 1234);
		text += string.Format("?random={0}", num);
		if (file == Files.OfferState || file == Files.ABConfig)
		{
			text = text + "&mid=" + UserIdentification.Current;
		}
		if (file == Files.ABConfig)
		{
			text = text + "&new=" + (PlayerStats.GetStat(PlayerStats.StatNames.TimePlayed_Total) < 100).ToString().ToLower();
			text = text + "&missions=" + PlayerStats.GetStat(PlayerStats.StatNames.MissionsCompleted_Total);
			text = text + "&runs=" + PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total);
			text = text + "&distance=" + (int)(PlayerStats.GetDistance(PlayerStats.DistanceNames.DistanceRun_Total) / 1000f);
			text = text + "&shop_purchases=" + PlayerStats.GetStat(PlayerStats.StatNames.ShopPurchases_Total);
			text = text + "&inapp_purchases=" + PlayerStats.GetStat(PlayerStats.StatNames.InAppPurchases_Total);
			text = text + "&time_played=" + ((float)PlayerStats.GetStat(PlayerStats.StatNames.TimePlayed_Total) * 1f / 36000f).ToString("N");
			string text2 = Platform.PlatformPostFix.Replace(" - ", string.Empty).ToLowerInvariant();
			text = text + "&platform=" + text2;
			string text3 = "1.16.1".Replace(".", "_");
			text = text + "&version=" + text3;
			text = text + "&actual_cohort=" + ABTesting.Cohort;
		}
		return text;
	}

	private static string BuildUrl(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return ServerBaseUrl;
		}
		if (!path.StartsWith("/"))
		{
			path = "/" + path;
		}
		return ServerBaseUrl.TrimEnd('/') + path;
	}

	private void SaveLocalCopy(Files file)
	{
		string text = Text + "FileIdentifier:" + CRC32.Generate(Text + "Cupcake" + file, CRC32.Case.AsIs);
		string filePath = GetFilePath(file);
		File.WriteAllText(filePath, ConvertStringToHex(text));
	}

	private void LoadLocalCopy(Files file)
	{
		string filePath = GetFilePath(file);
		if (File.Exists(filePath))
		{
			string input = ConvertHexToString(File.ReadAllText(filePath));
			string[] array = Regex.Split(input, "FileIdentifier:");
			if (array.Length != 2)
			{
				Error = "Couldn't find CRC of local file.";
			}
			else if (array[1] == CRC32.Generate(array[0] + "Cupcake" + file, CRC32.Case.AsIs).ToString())
			{
				Text = array[0];
				Error = null;
			}
			else
			{
				Error = "Wrong CRC of local file.";
			}
		}
	}

	private string GetFilePath(Files file)
	{
		return string.Format("{0}/{1}", Application.persistentDataPath, CRC32.Generate(file.ToString() + "Cupcake", CRC32.Case.AsIs));
	}

	private string ConvertStringToHex(string text)
	{
		string text2 = string.Empty;
		for (int i = 0; i < text.Length; i++)
		{
			text2 += string.Format("{0:x2}", Convert.ToUInt32(((int)text[i]).ToString()));
		}
		return text2;
	}

	private string ConvertHexToString(string hexText)
	{
		string text = string.Empty;
		while (hexText.Length > 0)
		{
			text += Convert.ToChar(Convert.ToUInt32(hexText.Substring(0, 2), 16));
			hexText = hexText.Substring(2, hexText.Length - 2);
		}
		return text;
	}

	private class FileDownloaderBehaviour : MonoBehaviour
	{
	}
}
