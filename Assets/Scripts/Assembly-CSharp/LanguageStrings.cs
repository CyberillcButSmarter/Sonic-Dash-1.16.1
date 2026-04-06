using System.Collections.Generic;
using Localisation;
using UnityEngine;

public class LanguageStrings : MonoBehaviour
{
	private const string LocalisationResourcePath = "Localisation/";

	private const string DefaultStringFile = "Sonic Dash Strings";

	[SerializeField]
	private string m_stringFile = DefaultStringFile;

	private Strings m_strings = new Strings();

	private bool m_initialised;

	private bool m_hasLoaded;

	public static List<LanguageStrings> StringResources { get; private set; }

	public static LanguageStrings First
	{
		get
		{
			EnsureLanguageStringsInstance();
			return (StringResources != null && StringResources.Count > 0) ? StringResources[0] : null;
		}
	}

	public string GetString(string id)
	{
		return m_strings.GetString(id);
	}

	public string[] GetAllStrings()
	{
		TextAsset textAsset = LoadLanguageFile(false);
		string systemLanguage = GetSystemLanguage();
		string[] strings = null;
		string[] identifiers = null;
		bool stringEntries = m_strings.GetStringEntries(textAsset, systemLanguage, ref identifiers, ref strings);
		UnloadLanguageFile(textAsset);
		return (!stringEntries) ? null : strings;
	}

	public void ReloadStringFile()
	{
		string systemLanguage = GetSystemLanguage();
		TextAsset textAsset = LoadLanguageFile(false);
		if (textAsset != null)
		{
			m_strings.LoadXMLStringsFile(textAsset, systemLanguage, Strings.Type.Primary);
			UnloadLanguageFile(textAsset);
		}
		TextAsset textAsset2 = LoadLanguageFile(true);
		if (textAsset2 != null)
		{
			m_strings.LoadXMLStringsFile(textAsset2, systemLanguage, Strings.Type.Platform);
			UnloadLanguageFile(textAsset2);
		}
		m_hasLoaded = true;
	}

	private void Start()
	{
		EnsureInitialised();
	}

	private void OnNukingLevel()
	{
		StringResources = null;
	}

	private TextAsset LoadLanguageFile(bool loadPlatformText)
	{
		TextAsset textAsset = null;
		if (loadPlatformText)
		{
			string path = "Localisation/" + m_stringFile + Platform.PlatformPostFix;
			return Resources.Load(path) as TextAsset;
		}
		string path2 = "Localisation/" + m_stringFile + Platform.CommonPostFix;
		return Resources.Load(path2) as TextAsset;
	}

	private void UnloadLanguageFile(TextAsset languageFile)
	{
		if (languageFile != null)
		{
			Resources.UnloadAsset(languageFile);
		}
	}

	private string GetSystemLanguage()
	{
		return Language.GetLanguage().ToString();
	}

	private static void EnsureLanguageStringsInstance()
	{
		if (StringResources == null)
		{
			StringResources = new List<LanguageStrings>();
		}
		StringResources.RemoveAll((LanguageStrings entry) => entry == null);
		if (StringResources.Count > 0)
		{
			InitialiseAll(StringResources);
			return;
		}
		LanguageStrings[] array = Object.FindObjectsOfType<LanguageStrings>();
		if (array != null && array.Length > 0)
		{
			LanguageStrings[] array2 = array;
			foreach (LanguageStrings languageStrings in array2)
			{
				if (!(languageStrings == null))
				{
					languageStrings.EnsureInitialised();
				}
			}
		}
		if (StringResources.Count > 0)
		{
			return;
		}
		GameObject gameObject = new GameObject("LanguageStrings (Runtime)");
		Object.DontDestroyOnLoad(gameObject);
		LanguageStrings languageStrings2 = gameObject.AddComponent<LanguageStrings>();
		languageStrings2.m_stringFile = DefaultStringFile;
		languageStrings2.EnsureInitialised();
	}

	private static void InitialiseAll(List<LanguageStrings> instances)
	{
		for (int i = 0; i < instances.Count; i++)
		{
			LanguageStrings languageStrings = instances[i];
			if (!(languageStrings == null))
			{
				languageStrings.EnsureInitialised();
			}
		}
	}

	private void EnsureInitialised()
	{
		if (m_initialised)
		{
			EnsureStringsLoaded();
			return;
		}
		m_initialised = true;
		Object.DontDestroyOnLoad(base.gameObject);
		StoreThisInList();
		EnsureStringsLoaded();
	}

	private void EnsureStringsLoaded()
	{
		if (!m_hasLoaded)
		{
			ReloadStringFile();
		}
	}

	private void StoreThisInList()
	{
		if (StringResources == null)
		{
			StringResources = new List<LanguageStrings>();
		}
		if (!StringResources.Contains(this))
		{
			StringResources.Add(this);
		}
	}
}
