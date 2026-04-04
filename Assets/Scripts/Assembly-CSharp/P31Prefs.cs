using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P31Prefs
{
	private static AndroidJavaClass m_cloudInterface;

	private static Dictionary<string, string> m_dictionary;

	static P31Prefs()
	{
		m_cloudInterface = new AndroidJavaClass("com.hardlightstudio.dev.sonicdash.plugin.cloud.CloudInterface");
		m_dictionary = new Dictionary<string, string>();
	}

	public static bool isCloudConnected()
	{
		return m_cloudInterface.CallStatic<bool>("isCloudConnected", new object[0]);
	}

	public static bool synchronize()
	{
		m_cloudInterface.CallStatic("synchronize");
		rebuildDictionary();
		return true;
	}

	public static void setDictionary(string key, Hashtable table)
	{
		if (table != null && table.Count > 0)
		{
			m_cloudInterface.CallStatic("clearCloudData");
			m_dictionary.Clear();
			foreach (string key2 in table.Keys)
			{
				string text2 = table[key2].ToString();
				m_cloudInterface.CallStatic("addPair", key2, text2);
				m_dictionary.Add(key2, text2);
			}
		}
		m_cloudInterface.CallStatic("synchronize");
	}

	public static void rebuildDictionary()
	{
		int num = m_cloudInterface.CallStatic<int>("getHashmapSize", new object[0]);
		if (num <= 0)
		{
			return;
		}
		m_dictionary.Clear();
		for (int i = 0; i < num; i++)
		{
			string key = m_cloudInterface.CallStatic<string>("getHashmapKey", new object[1] { i });
			string value = m_cloudInterface.CallStatic<string>("getHashmapValue", new object[1] { i });
			if (!m_dictionary.ContainsKey(key))
			{
				m_dictionary.Add(key, value);
			}
		}
	}

	public static IDictionary getDictionary(string key)
	{
		if (m_dictionary.Count <= 0)
		{
			return null;
		}
		return m_dictionary;
	}
}
