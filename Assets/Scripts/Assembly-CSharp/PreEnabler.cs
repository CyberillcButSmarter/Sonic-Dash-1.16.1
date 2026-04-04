using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreEnabler : MonoBehaviour
{
	public StoreFront[] m_ShopGrids;

	public HighScorePopulator[] m_FacebookGrids;

	private Hashtable m_Shoptweeners = new Hashtable();

	private Hashtable m_Shoptranslators = new Hashtable();

	private Hashtable m_Facebooktweeners = new Hashtable();

	private Hashtable m_Facebooktranslators = new Hashtable();

	private bool m_shopPreloadStarted;

	private bool m_facebookPreloadStarted;

	private bool m_facebookGridsPreloaded;

	public static bool gridsPreloaded;

	private void Update()
	{
		if (!m_shopPreloadStarted)
		{
			m_shopPreloadStarted = true;
			PreloadShopGrids(0);
		}
		if (!m_facebookPreloadStarted)
		{
			m_facebookPreloadStarted = true;
			PreloadFacebookGrid(0);
		}
		if (gridsPreloaded && m_facebookGridsPreloaded)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void PreloadShopGrids(int index)
	{
		if (m_ShopGrids == null || m_ShopGrids.Length == 0)
		{
			gridsPreloaded = true;
			return;
		}
		if (index >= m_ShopGrids.Length)
		{
			gridsPreloaded = true;
			return;
		}
		StoreFront storeFront = m_ShopGrids[index];
		List<UITweener> tweenerList;
		if (m_Shoptweeners.ContainsKey(storeFront))
		{
			tweenerList = (List<UITweener>)m_Shoptweeners[storeFront];
			tweenerList.Clear();
		}
		else
		{
			tweenerList = new List<UITweener>();
			m_Shoptweeners.Add(storeFront, tweenerList);
		}
		List<LocalisedString> translatorList;
		if (m_Shoptranslators.ContainsKey(storeFront))
		{
			translatorList = (List<LocalisedString>)m_Shoptranslators[storeFront];
			translatorList.Clear();
		}
		else
		{
			translatorList = new List<LocalisedString>();
			m_Shoptranslators.Add(storeFront, translatorList);
		}
		List<UITweener> tweens = CollectComponents<UITweener>(storeFront.EnableControlParent);
		foreach (UITweener uITweener in tweens)
		{
			if (uITweener.enabled)
			{
				uITweener.enabled = false;
			}
			tweenerList.Add(uITweener);
		}
		List<LocalisedString> translators = CollectComponents<LocalisedString>(storeFront.EnableControlParent);
		foreach (LocalisedString localisedString in translators)
		{
			if (localisedString.enabled)
			{
				localisedString.enabled = false;
			}
			translatorList.Add(localisedString);
		}
		storeFront.Activate(this, index);
	}

	public void PreloadGridShopFinished(int index)
	{
		StoreFront storeFront = m_ShopGrids[index];
		storeFront.DeActivate();
		foreach (UITweener item in m_Shoptweeners[storeFront] as List<UITweener>)
		{
			item.enabled = true;
		}
		foreach (LocalisedString item2 in m_Shoptranslators[storeFront] as List<LocalisedString>)
		{
			item2.enabled = true;
		}
		if (index + 1 < m_ShopGrids.Length)
		{
			PreloadShopGrids(index + 1);
		}
		else
		{
			gridsPreloaded = true;
		}
	}

	private void PreloadFacebookGrid(int index)
	{
		if (m_FacebookGrids == null || m_FacebookGrids.Length == 0)
		{
			m_facebookGridsPreloaded = true;
			return;
		}
		if (index >= m_FacebookGrids.Length)
		{
			m_facebookGridsPreloaded = true;
			return;
		}
		HighScorePopulator highScorePopulator = m_FacebookGrids[index];
		List<UITweener> tweenerList;
		if (m_Facebooktweeners.ContainsKey(highScorePopulator))
		{
			tweenerList = (List<UITweener>)m_Facebooktweeners[highScorePopulator];
			tweenerList.Clear();
		}
		else
		{
			tweenerList = new List<UITweener>();
			m_Facebooktweeners.Add(highScorePopulator, tweenerList);
		}
		List<LocalisedString> translatorList;
		if (m_Facebooktranslators.ContainsKey(highScorePopulator))
		{
			translatorList = (List<LocalisedString>)m_Facebooktranslators[highScorePopulator];
			translatorList.Clear();
		}
		else
		{
			translatorList = new List<LocalisedString>();
			m_Facebooktranslators.Add(highScorePopulator, translatorList);
		}
		List<UITweener> tweens = CollectComponents<UITweener>(highScorePopulator.EnableControlParent);
		foreach (UITweener uITweener in tweens)
		{
			if (uITweener.enabled)
			{
				uITweener.enabled = false;
			}
			tweenerList.Add(uITweener);
		}
		List<LocalisedString> translators = CollectComponents<LocalisedString>(highScorePopulator.EnableControlParent);
		foreach (LocalisedString localisedString in translators)
		{
			if (localisedString.enabled)
			{
				localisedString.enabled = false;
			}
			translatorList.Add(localisedString);
		}
		highScorePopulator.Activate(this, index);
	}

	public void PreloadGridFacebookFinished(int index)
	{
		HighScorePopulator highScorePopulator = m_FacebookGrids[index];
		highScorePopulator.DeActivate();
		foreach (UITweener item in m_Facebooktweeners[highScorePopulator] as List<UITweener>)
		{
			item.enabled = true;
		}
		foreach (LocalisedString item2 in m_Facebooktranslators[highScorePopulator] as List<LocalisedString>)
		{
			item2.enabled = true;
		}
		if (index + 1 < m_FacebookGrids.Length)
		{
			PreloadFacebookGrid(index + 1);
		}
		else
		{
			m_facebookGridsPreloaded = true;
		}
	}

	private List<T> CollectComponents<T>(GameObject root) where T : Component
	{
		if (root == null)
		{
			return new List<T>();
		}
		List<T> list = new List<T>(root.GetComponents<T>());
		T[] children = root.GetComponentsInChildren<T>();
		foreach (T component in children)
		{
			if (!list.Contains(component))
			{
				list.Add(component);
			}
		}
		return list;
	}
}
