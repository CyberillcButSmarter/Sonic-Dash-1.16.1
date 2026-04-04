using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog_RequirePurchase : MonoBehaviour
{
	private const string RingsRequestedString = "STORE_PURCHASE_NO_RINGS_01";

	private const string StarsRequestedString = "STORE_PURCHASE_NO_STAR_RINGS_01";

	private const string RingsDisclaimerString = "STORE_PURCHASE_DISCLAIMER_RINGS";

	private const string StarsDisclaimerString = "STORE_PURCHASE_DISCLAIMER_STAR_RINGS";

	private static int s_bundlesToShow = 1;

	private static StoreContent.StoreEntry[] s_entriesToDisplay;

	private static StoreContent.PaymentMethod s_paymentMethod;

	private List<UIDragScrollView> m_storePanels;

	[SerializeField]
	private UILabel m_description;

	[SerializeField]
	private UILabel m_disclaimer;

	[SerializeField]
	private GameObject[] m_storeEntry;

	[SerializeField]
	private GameObject m_entryListPrefab;

	[SerializeField]
	private UIScrollView m_dragablePanel;

	[SerializeField]
	private UIGrid m_displayGrid;

	public static void Display(StoreContent.StoreEntry[] entries, StoreContent.PaymentMethod paymentMethod)
	{
		s_entriesToDisplay = entries;
		s_paymentMethod = paymentMethod;
		s_bundlesToShow = Mathf.Clamp(ABTesting.GetTestValueAsInt(ABTesting.Tests.STORE_MaximumBundleOffers), 1, 5);
		s_bundlesToShow = Math.Min(s_bundlesToShow, entries.Length);
		string dialogName = string.Format("Dialog Require Purchase 0{0}", s_bundlesToShow);
		DialogStack.ShowDialog(dialogName);
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		StartCoroutine(StartPendingActivation());
	}

	private IEnumerator StartPendingActivation()
	{
		yield return null;
		string descriptionString = "STORE_PURCHASE_NO_RINGS_01";
		string disclaimerString = "STORE_PURCHASE_DISCLAIMER_RINGS";
		if (s_paymentMethod == StoreContent.PaymentMethod.StarRings)
		{
			descriptionString = "STORE_PURCHASE_NO_STAR_RINGS_01";
			disclaimerString = "STORE_PURCHASE_DISCLAIMER_STAR_RINGS";
		}
		string textToShow = LanguageStrings.First.GetString(descriptionString);
		m_description.text = textToShow;
		textToShow = LanguageStrings.First.GetString(disclaimerString);
		m_disclaimer.text = textToShow;
		if (m_entryListPrefab != null)
		{
			PopulatePanelWithEntries();
			yield break;
		}
		for (int i = 0; i < s_bundlesToShow; i++)
		{
			StorePopulator.PopulateEntry(m_storeEntry[i], s_entriesToDisplay[i], StorePopulator.Display.IgnoreTimeOut);
		}
	}

	private void PopulatePanelWithEntries()
	{
		m_dragablePanel.ResetPosition();
		UpdatePanelContents();
		for (int i = 0; i < s_bundlesToShow; i++)
		{
			if (i < s_entriesToDisplay.Length)
			{
				m_storePanels[i].gameObject.SetActive(true);
				StorePopulator.PopulateEntry(m_storePanels[i].gameObject, s_entriesToDisplay[i], StorePopulator.Display.IgnoreTimeOut);
				IntStorage componentInChildren = Utils.GetComponentInChildren<IntStorage>(m_storePanels[i].gameObject);
				componentInChildren.Value = i;
			}
			else
			{
				m_storePanels[i].gameObject.SetActive(false);
			}
		}
		m_displayGrid.sorting = UIGrid.Sorting.Alphabetic;
		m_displayGrid.Reposition();
		m_dragablePanel.ResetPosition();
	}

	private void UpdatePanelContents()
	{
		if (m_storePanels == null)
		{
			m_storePanels = new List<UIDragScrollView>(s_entriesToDisplay.Length);
		}
		if (m_storePanels.Count < s_bundlesToShow)
		{
			int num = s_entriesToDisplay.Length - m_storePanels.Count;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = NGUITools.AddChild(m_displayGrid.gameObject, m_entryListPrefab.gameObject);
				gameObject.name = i.ToString("D3");
				UIButtonMessage componentInChildren = Utils.GetComponentInChildren<UIButtonMessage>(gameObject);
				componentInChildren.target = base.gameObject;
				UIDragScrollView component = gameObject.GetComponent<UIDragScrollView>();
				m_storePanels.Add(component);
			}
		}
	}

	private void Trigger_CompleteIAP(GameObject callingObject)
	{
		int num = 0;
		if (callingObject != null)
		{
			IntStorage component = callingObject.GetComponent<IntStorage>();
			if (component != null)
			{
				num = component.Value;
			}
		}
		EventDispatch.GenerateEvent("CompletePendingIAP", s_entriesToDisplay[num]);
		DialogStack.HideDialog();
	}

	private void Trigger_CancelIAP()
	{
		EventDispatch.GenerateEvent("CancelPendingIAP");
		DialogStack.HideDialog();
	}
}
