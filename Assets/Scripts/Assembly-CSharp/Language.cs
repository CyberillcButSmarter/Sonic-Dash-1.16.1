using System.Globalization;
using UnityEngine;

public class Language
{
	public enum ID
	{
		English_UK = 0,
		English_US = 1,
		French = 2,
		Italian = 3,
		German = 4,
		Spanish = 5,
		Portuguese_Brazil = 6,
		Russian = 7,
		Korean = 8,
		Chinese = 9,
		Japanese = 10
	}

	public enum Locale
	{
		Other = 0,
		US = 1,
		Brazil = 2,
		Japan = 3,
		Korea = 4,
		China = 5,
		Noof = 6
	}

	public static string[] CultureInfoIDs = new string[11]
	{
		"en-GB", "en-US", "fr-FR", "it-IT", "de-DE", "es-ES", "pt-BR", "ru-RU", "ko-KR", "zh-CHS",
		"ja-JP"
	};

	private static readonly string[] LanguageExtensions = new string[11]
	{
		"uk", "us", "fr", "it", "ge", "sp", "br", "ru", "ko", "ch",
		"jp"
	};

	public static ID GetLanguage()
	{
		if ((bool)LanguageDebugging.Debugger && LanguageDebugging.Debugger.OverrideSystemLanguage)
		{
			return LanguageDebugging.Debugger.ForcedLanguage;
		}
		SystemLanguage unityLanguage = ResolveSystemLanguage();
		Locale locale = GetLocale();
		return GetGameLanguage(unityLanguage, locale);
	}

	public static Locale GetLocale()
	{
		int num = GetCurrentLocale();
		if (num < 0 || num >= (int)Locale.Noof)
		{
			num = 0;
		}
		return (Locale)num;
	}

	public static string GetExtension()
	{
		ID language = GetLanguage();
		return LanguageExtensions[(int)language];
	}

	public static string GetExtensionGroup()
	{
		string result = string.Empty;
		switch (GetLanguage())
		{
		case ID.English_UK:
		case ID.English_US:
		case ID.French:
		case ID.Italian:
		case ID.German:
		case ID.Spanish:
		case ID.Portuguese_Brazil:
		case ID.Russian:
			result = "efigs";
			break;
		case ID.Korean:
		case ID.Chinese:
		case ID.Japanese:
			result = GetExtension();
			break;
		}
		return result;
	}

	private static ID GetGameLanguage(SystemLanguage unityLanguage, Locale currentLocale)
	{
		ID result = ID.English_UK;
		switch (unityLanguage)
		{
		case SystemLanguage.English:
			result = ((currentLocale == Locale.US) ? ID.English_US : ID.English_UK);
			break;
		case SystemLanguage.French:
			result = ID.French;
			break;
		case SystemLanguage.Italian:
			result = ID.Italian;
			break;
		case SystemLanguage.German:
			result = ID.German;
			break;
		case SystemLanguage.Spanish:
			result = ID.Spanish;
			break;
		case SystemLanguage.Portuguese:
			result = ID.Portuguese_Brazil;
			break;
		case SystemLanguage.Russian:
			result = ID.Russian;
			break;
		case SystemLanguage.Korean:
			result = ID.Korean;
			break;
		case SystemLanguage.Chinese:
			result = ID.Chinese;
			break;
		case SystemLanguage.Japanese:
			result = ID.Japanese;
			break;
		}
		return result;
	}

	private static SystemLanguage ResolveSystemLanguage()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		if (systemLanguage != SystemLanguage.Unknown)
		{
			return systemLanguage;
		}
		SystemLanguage systemLanguage2 = SystemLanguage.English;
		try
		{
			CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
			string text = ((currentUICulture != null) ? currentUICulture.TwoLetterISOLanguageName : CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
			switch (text)
			{
			case "fr":
				systemLanguage2 = SystemLanguage.French;
				break;
			case "it":
				systemLanguage2 = SystemLanguage.Italian;
				break;
			case "de":
				systemLanguage2 = SystemLanguage.German;
				break;
			case "es":
				systemLanguage2 = SystemLanguage.Spanish;
				break;
			case "pt":
				systemLanguage2 = SystemLanguage.Portuguese;
				break;
			case "ru":
				systemLanguage2 = SystemLanguage.Russian;
				break;
			case "ko":
				systemLanguage2 = SystemLanguage.Korean;
				break;
			case "zh":
				systemLanguage2 = SystemLanguage.Chinese;
				break;
			case "ja":
				systemLanguage2 = SystemLanguage.Japanese;
				break;
			case "en":
				systemLanguage2 = SystemLanguage.English;
				break;
			}
		}
		catch
		{
		}
		return systemLanguage2;
	}

	private static int GetCurrentLocale()
	{
		int result = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.hardlightstudio.dev.sonicdash.plugin.SLGlobal");
			result = androidJavaClass.CallStatic<int>("GetCurrentLocale", new object[0]);
		}
		catch (System.Exception ex)
		{
			Debug.LogWarning("GetCurrentLocale() failed, falling back to culture info: " + ex.Message);
		}
#endif
		if (result == 0)
		{
			result = LocaleFromCultureInfo();
		}
		return result;
	}

	private static int LocaleFromCultureInfo()
	{
		try
		{
			CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
			string text = ((currentUICulture != null) ? currentUICulture.Name : CultureInfo.CurrentCulture.Name).ToLower();
			if (text.StartsWith("pt"))
			{
				return (int)Locale.Brazil;
			}
			if (text.StartsWith("ja"))
			{
				return (int)Locale.Japan;
			}
			if (text.StartsWith("ko"))
			{
				return (int)Locale.Korea;
			}
			if (text.StartsWith("zh"))
			{
				return (int)Locale.China;
			}
			if (text.StartsWith("en-us"))
			{
				return (int)Locale.US;
			}
		}
		catch
		{
		}
		return (int)Locale.Other;
	}
}
