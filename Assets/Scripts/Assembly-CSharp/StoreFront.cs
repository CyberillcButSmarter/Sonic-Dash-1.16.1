using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoreFront : MonoBehaviour
{
	private class StoreListing
	{
		public StoreContent.StoreEntry m_storeEntry;

		public UIDragScrollView m_storePanel;

		public bool m_wasDownloading;
	}

	private const float preloadDelayFactor = 0.07f;

	private StoreContent.StoreEntry entryToHighlight;

	private bool started;

	private bool starting;

	private PreEnabler preEnabler;

	private int preloadIndex;

	private static int s_entryCount;

	private UIGrid m_displayGrid;

	private UIScrollView m_draggablePanel;

	private List<StoreListing> m_storeList;

	[SerializeField]
	private StoreContent m_storeContent;

	[SerializeField]
	private UIDragScrollView m_headingTemplate;

	[SerializeField]
	private GameObject m_entryTemplate;

	[SerializeField]
	private StoreContent.Group m_storeGroup = StoreContent.Group.None;

	[SerializeField]
	private GuiTrigger m_sourceTrigger;

	[SerializeField]
	private GameObject m_enableControlParent;

	public GameObject EnableControlParent
	{
		get
		{
			return m_enableControlParent;
		}
		set
		{
			m_enableControlParent = value;
		}
	}

	public StoreContent.StoreEntry FindStoreEntry(UIDragScrollView storeEntry)
	{
		StoreListing storeListing = m_storeList.Find((StoreListing thisList) => thisList.m_storePanel == storeEntry);
		return storeListing.m_storeEntry;
	}

	public UIDragScrollView FindStoreEntry(StoreContent.StoreEntry storeEntry)
	{
		StoreListing storeListing = m_storeList.Find((StoreListing thisList) => thisList.m_storeEntry == storeEntry);
		return storeListing.m_storePanel;
	}

	private void MyStart()
	{
		starting = true;
		m_displayGrid = Utils.FindBehaviourInTree(this, m_displayGrid);
		m_draggablePanel = Utils.FindBehaviourInTree(this, m_draggablePanel);
		EventDispatch.RegisterInterest("OnStorePurchaseStarted", this);
		EventDispatch.RegisterInterest("OnStorePurchaseCompleted", this);
		EventDispatch.RegisterInterest("OnStoreUpdateStarted", this);
		EventDispatch.RegisterInterest("OnStoreUpdateFinished", this);
		CreateStorePanelEntries();
	}

	private void Update()
	{
		if (!started && !starting)
		{
			MyStart();
		}
		else
		{
			if (starting || m_storeList == null)
			{
				return;
			}
			for (int i = 0; i < m_storeList.Count; i++)
			{
				StoreListing storeListing = m_storeList[i];
				bool flag = (storeListing.m_storeEntry.m_state & StoreContent.StoreEntry.State.Downloading) == StoreContent.StoreEntry.State.Downloading;
				if (flag || storeListing.m_wasDownloading)
				{
					StorePopulator.PopulateMinimalEntry(storeListing.m_storePanel.gameObject, storeListing.m_storeEntry, StorePopulator.Display.UpdateDownload);
					storeListing.m_wasDownloading = flag;
					if (!storeListing.m_wasDownloading)
					{
						StorePopulator.PopulateEntry(storeListing.m_storePanel.gameObject, storeListing.m_storeEntry);
					}
				}
			}
		}
	}

	private void OnEnable()
	{
		if (!starting)
		{
			StartCoroutine(StartPendingActivation());
		}
	}

	private IEnumerator StartPendingActivation()
	{
		yield return null;
		m_draggablePanel.ResetPosition();
		PopulatePanelEntries();
		m_draggablePanel.ResetPosition();
		if (entryToHighlight != null)
		{
			int index = 0;
			int index2 = -1;
			UIDragScrollView paneltoHighlight = FindStoreEntry(entryToHighlight);
			paneltoHighlight.transform.Find("Entry_glow").GetComponent<Animation>().enabled = true;
			bool found = false;
			while (!found)
			{
				GameObject entry = base.gameObject.transform.GetChild(index).gameObject;
				found = entry == paneltoHighlight.gameObject;
				index++;
				if (entry.activeSelf)
				{
					index2++;
				}
			}
			Vector3 movement = new Vector3(0f, (float)index2 * base.gameObject.GetComponent<UIGrid>().cellHeight, 0f);
			base.transform.parent.GetComponent<UIScrollView>().MoveRelative(movement);
			base.transform.parent.GetComponent<UIScrollView>().UpdatePosition();
			entryToHighlight = null;
			yield break;
		}
		foreach (StoreListing sl in m_storeList)
		{
			sl.m_storePanel.transform.Find("Entry_glow").GetComponent<Animation>().enabled = false;
		}
		base.transform.parent.GetComponent<UIScrollView>().ResetPosition();
	}

	private void CreateStorePanelEntries()
	{
		m_draggablePanel.ResetPosition();
		List<StoreContent.StoreEntry> stockList = StoreContent.StockList;
		m_storeList = new List<StoreListing>();
		IEnumerable<StoreContent.StoreEntry> storeContent = stockList.Where((StoreContent.StoreEntry thisEntry) => thisEntry.m_group == m_storeGroup);
		StartCoroutine(CreateIndividualEntries(storeContent, m_entryTemplate, m_headingTemplate));
	}

	private void PopulatePanelEntries()
	{
		for (int i = 0; i < m_storeList.Count; i++)
		{
			StoreListing storeListing = m_storeList[i];
			StoreContent.StoreEntry storeEntry = storeListing.m_storeEntry;
			UIDragScrollView storePanel = storeListing.m_storePanel;
			StoreContent.ValidateEntry(storeEntry);
			bool flag = (storeEntry.m_state & StoreContent.StoreEntry.State.Hidden) == StoreContent.StoreEntry.State.Hidden;
			bool flag2 = (storeEntry.m_state & StoreContent.StoreEntry.State.Purchased) == StoreContent.StoreEntry.State.Purchased;
			bool flag3 = (storeEntry.m_state & StoreContent.StoreEntry.State.ShowIfPurchased) == StoreContent.StoreEntry.State.ShowIfPurchased;
			if (flag || (flag2 && !flag3))
			{
				storePanel.gameObject.SetActive(false);
				continue;
			}
			storePanel.gameObject.SetActive(true);
			StorePopulator.PopulateEntry(storePanel.gameObject, storeEntry);
		}
		m_displayGrid.Reposition();
	}

	private IEnumerator CreateIndividualEntries(IEnumerable<StoreContent.StoreEntry> storeContent, GameObject entryTemplate, UIDragScrollView headerTemplate)
	{
		if (storeContent.Count() == 0)
		{
			yield break;
		}
		yield return null;
		foreach (StoreContent.StoreEntry thisEntry in storeContent)
		{
			GameObject newObject = NGUITools.AddChild(base.gameObject, entryTemplate.gameObject);
			newObject.name = string.Format("{0} {1}", s_entryCount.ToString("D3"), thisEntry.m_type.ToString());
			s_entryCount++;
			UIDragScrollView panel = newObject.GetComponent<UIDragScrollView>();
			AddStoreListing(thisEntry, panel);
			DeepLink dl = DeepLink.GetDeepLink();
			string pageName = string.Empty;
			if (dl == null || !dl.HasPageName(out pageName) || dl.HasBeenUsed)
			{
				yield return new WaitForSeconds(0.07f);
			}
		}
		m_displayGrid.sorting = UIGrid.Sorting.Alphabetic;
		m_displayGrid.Reposition();
		m_draggablePanel.ResetPosition();
		preEnabler.PreloadGridShopFinished(preloadIndex);
		starting = false;
		started = true;
	}

	public void Activate(PreEnabler preEnabler, int preloadIndex)
	{
		this.preEnabler = preEnabler;
		this.preloadIndex = preloadIndex;
		Vector3 localPosition = m_enableControlParent.transform.localPosition;
		localPosition.y -= 10000f;
		m_enableControlParent.transform.localPosition = localPosition;
		m_enableControlParent.SetActive(true);
	}

	public void DeActivate()
	{
		m_enableControlParent.SetActive(false);
		Vector3 localPosition = m_enableControlParent.transform.localPosition;
		localPosition.y += 10000f;
		m_enableControlParent.transform.localPosition = localPosition;
	}

	private void AddStoreListing(StoreContent.StoreEntry entry, UIDragScrollView panel)
	{
		StoreListing storeListing = new StoreListing();
		storeListing.m_storeEntry = entry;
		storeListing.m_storePanel = panel;
		storeListing.m_wasDownloading = false;
		m_storeList.Add(storeListing);
	}

	private void Event_OnStorePurchaseCompleted(StoreContent.StoreEntry purchasedEntry, StorePurchases.Result result)
	{
		if (purchasedEntry != null)
		{
			PopulatePanelEntries();
		}
	}

	private void Event_OnStorePurchaseStarted(StoreContent.StoreEntry purchasedEntry)
	{
		PopulatePanelEntries();
	}

	private void Event_OnStoreUpdateStarted()
	{
		PopulatePanelEntries();
	}

	private void Event_OnStoreUpdateFinished()
	{
		PopulatePanelEntries();
	}

	public void HighlightEntry(StoreContent.StoreEntry entry)
	{
		entryToHighlight = entry;
	}
}
