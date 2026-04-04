using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighScorePopulator : MonoBehaviour
{
	private const float preloadDelayFactor = 0f;

	private bool started;

	private bool starting;

	private PreEnabler preEnabler;

	private int preloadIndex;

	private UIGrid m_displayGrid;

	private UIScrollView m_draggablePanel;

	private List<GameObject> m_panelEntries;

	private GameObject m_facebookLoginPrompt;

	[SerializeField]
	private int m_entriesToShow = 11;

	[SerializeField]
	private UIDragScrollView m_entryTemplate;

	[SerializeField]
	private UIDragScrollView m_facebookEntry;

	[SerializeField]
	private Leaderboards.Types m_leaderboardToShow;

	[SerializeField]
	private Leaderboards.Request.Filter m_filterType = Leaderboards.Request.Filter.Friends;

	[SerializeField]
	private Texture2D m_defaultAvatar;

	[SerializeField]
	private GameObject m_loadingIndicator;

	[SerializeField]
	private GameObject m_emptyBoardContent;

	[SerializeField]
	private GameObject m_notSignedInContent;

	[SerializeField]
	private int m_maximumEntriesForEmptyContent = 4;

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

	private void MyStart()
	{
		starting = true;
		m_displayGrid = Utils.FindBehaviourInTree(this, m_displayGrid);
		m_draggablePanel = Utils.FindBehaviourInTree(this, m_draggablePanel);
		CreateStorePanelEntries();
	}

	public void Update()
	{
		if (!started && !starting)
		{
			MyStart();
		}
	}

	private void OnEnable()
	{
		if (!starting)
		{
			StartCoroutine(StartPendingActivation());
		}
	}

	private void OnDisable()
	{
		DisableAllEntries();
	}

	private IEnumerator StartPendingActivation()
	{
		yield return null;
		if (m_notSignedInContent != null)
		{
			m_notSignedInContent.SetActive(false);
		}
		DisableAllEntries();
		RequestLeaderboardEntries();
		GameState.g_gameState.CacheLeaderboards(m_entriesToShow);
	}

	private void CreateStorePanelEntries()
	{
		m_panelEntries = new List<GameObject>(m_entriesToShow);
		m_draggablePanel.ResetPosition();
		StartCoroutine(CreateIndividualEntries());
	}

	private IEnumerator CreateIndividualEntries()
	{
		yield return null;
		for (int i = 0; i < m_entriesToShow; i++)
		{
			GameObject newObject = NGUITools.AddChild(base.gameObject, m_entryTemplate.gameObject);
			newObject.name = string.Format("LB Entry {0}", i.ToString("D3"));
			m_panelEntries.Add(newObject);
			newObject.SetActive(false);
			yield return new WaitForSeconds(0f);
		}
		m_displayGrid.sorting = UIGrid.Sorting.Alphabetic;
		m_displayGrid.Reposition();
		GameObject facebookPrompt = NGUITools.AddChild(base.gameObject, m_facebookEntry.gameObject);
		m_facebookLoginPrompt = facebookPrompt;
		UIButtonMessage button = m_facebookLoginPrompt.GetComponentInChildren(typeof(UIButtonMessage)) as UIButtonMessage;
		if (button != null)
		{
			button.target = Community.g_instance.gameObject;
		}
		m_facebookLoginPrompt.SetActive(false);
		m_draggablePanel.ResetPosition();
		preEnabler.PreloadGridFacebookFinished(preloadIndex);
		starting = false;
		started = true;
		EventDispatch.RegisterInterest("LeaderboardRequestComplete", this);
		EventDispatch.RegisterInterest("LeaderboardCacheComplete", this);
		EventDispatch.RegisterInterest("CacheLeaderboard", this);
		EventDispatch.RegisterInterest("OnFacebookLogin", this);
		EventDispatch.RegisterInterest("OnGameCenterLogin", this);
	}

	private void DisableAllEntries()
	{
		if (m_panelEntries == null)
		{
			return;
		}
		foreach (GameObject panelEntry in m_panelEntries)
		{
			if (panelEntry != null)
			{
				panelEntry.SetActive(false);
			}
		}
	}

	private void PopulateHighScoreEntries(int entryCount, Leaderboards.Entry[] entries)
	{
		m_draggablePanel.ResetPosition();
		int num = -1;
		for (int i = 0; i < m_entriesToShow; i++)
		{
			GameObject gameObject = m_panelEntries[i];
			if (i < entryCount)
			{
				Leaderboards.Entry entry = entries[i];
				gameObject.SetActive(true);
				m_panelEntries[i] = SetEntryContent(entry.m_rank, entry, gameObject);
				if (entry.m_playersRank)
				{
					num = i;
				}
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
		bool isFacebookAuthenticated = ((HLLocalUser)Social.localUser).isFacebookAuthenticated;
		m_facebookLoginPrompt.SetActive(!isFacebookAuthenticated);
		if (!isFacebookAuthenticated && num != -1)
		{
			m_facebookLoginPrompt.name = m_panelEntries[num].name + "2";
		}
		else
		{
			m_facebookLoginPrompt.name = "aaaaaa";
		}
		m_displayGrid.sorting = UIGrid.Sorting.Alphabetic;
		m_displayGrid.Reposition();
		m_draggablePanel.ResetPosition();
		int num2 = ((!isFacebookAuthenticated) ? (entryCount + 1) : entryCount);
		if (num != -1)
		{
			int num3 = Mathf.FloorToInt(m_draggablePanel.panel.clipRange.w / m_displayGrid.cellHeight);
			int num4 = (num3 - 1) / 2;
			int val = Math.Min(num - num4, num2 - num3);
			val = Math.Max(val, 0);
			float y = m_panelEntries[val].transform.localPosition.y;
			m_draggablePanel.MoveRelative(Vector3.down * y);
			m_draggablePanel.RestrictWithinBounds(true);
		}
		if (m_emptyBoardContent != null)
		{
			m_emptyBoardContent.SetActive(false);
		}
	}

	private GameObject SetEntryContent(int friendRank, Leaderboards.Entry entryData, GameObject entry)
	{
		GameObject gameObject = Utils.FindTagInChildren(entry, "HighScore_Name");
		if ((bool)gameObject)
		{
			UILabel component = gameObject.GetComponent<UILabel>();
			if (component.text.CompareTo(entryData.m_user) != 0)
			{
				string text = entry.name;
				entry.name += "deleted";
				entry.SetActive(false);
				UnityEngine.Object.Destroy(entry);
				GameObject gameObject2 = NGUITools.AddChild(base.gameObject, m_entryTemplate.gameObject);
				gameObject2.name = text;
				entry = gameObject2;
				gameObject = Utils.FindTagInChildren(entry, "HighScore_Name");
			}
		}
		GameObject gameObject3 = Utils.FindTagInChildren(entry, "HighScore_Avatar");
		GameObject gameObject4 = Utils.FindTagInChildren(entry, "HighScore_Score");
		GameObject gameObject5 = Utils.FindTagInChildren(entry, "HighScore_Rank");
		GameObject gameObject6 = Utils.FindTagInChildren(entry, "HighScore_Player");
		GameObject gameObject7 = Utils.FindTagInChildren(entry, "HighScore_Brag");
		GameObject gameObject8 = Utils.FindTagInChildren(entry, "Leaderboard FB");
		GameObject gameObject9 = Utils.FindTagInChildren(entry, "Leaderboard GC");
		GameObject gameObject10 = Utils.FindTagInChildren(entry, "FriendScore_Invite");
		if ((bool)gameObject9)
		{
			UISprite component2 = gameObject9.GetComponent<UISprite>();
			component2.spriteName = "icon_gpg_tiny";
		}
		if ((bool)gameObject)
		{
			UILabel component3 = gameObject.GetComponent<UILabel>();
			component3.text = entryData.m_user;
		}
		if (entryData.m_score == -1)
		{
			if ((bool)gameObject10)
			{
				gameObject10.SetActive(true);
			}
			if ((bool)gameObject4)
			{
				gameObject4.SetActive(false);
			}
		}
		else
		{
			if ((bool)gameObject4)
			{
				gameObject4.gameObject.SetActive(true);
				UILabel componentInChildren = gameObject4.GetComponentInChildren<UILabel>();
				componentInChildren.text = LanguageUtils.FormatNumber(entryData.m_score);
			}
			if ((bool)gameObject10)
			{
				gameObject10.SetActive(false);
			}
		}
		if ((bool)gameObject5)
		{
			UILabel component4 = gameObject5.GetComponent<UILabel>();
			if (friendRank == -1)
			{
				component4.text = string.Empty;
			}
			else if (entryData.m_score == -1)
			{
				component4.text = "?";
			}
			else
			{
				component4.text = friendRank.ToString();
			}
		}
		if ((bool)gameObject3)
		{
			gameObject3.SetActive(false);
			gameObject3.SetActive(true);
			UITexture component5 = gameObject3.GetComponent<UITexture>();
			component5.mainTexture = ((!(entryData.m_avatar == null)) ? entryData.m_avatar : m_defaultAvatar);
		}
		if ((bool)gameObject6)
		{
			if (entryData.m_playersRank)
			{
				gameObject6.SetActive(true);
			}
			else
			{
				gameObject6.SetActive(false);
			}
		}
		if ((bool)gameObject7)
		{
			gameObject7.SetActive(false);
		}
		if ((bool)gameObject9)
		{
			bool active = entryData.m_source == HLUserProfile.ProfileSource.GameCenter;
			gameObject9.SetActive(active);
		}
		if ((bool)gameObject8)
		{
			bool active2 = entryData.m_source == HLUserProfile.ProfileSource.Facebook;
			gameObject8.SetActive(active2);
		}
		return entry;
	}

	private void RequestLeaderboardEntries()
	{
		EventDispatch.GenerateEvent("RequestLeaderboard", Leaderboards.Types.sdHighestScore.ToString());
	}

	private void Event_OnFacebookLogin()
	{
		GameState.g_gameState.CacheLeaderboards();
	}

	private void Event_OnGameCenterLogin()
	{
		GameState.g_gameState.CacheLeaderboards();
	}

	private void Event_CacheLeaderboard(Leaderboards.Request request)
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_loadingIndicator.SetActive(true);
		}
	}

	private void Event_LeaderboardCacheComplete(string leaderboardID, bool leaderboardLoaded)
	{
		if (base.gameObject.activeInHierarchy)
		{
			RequestLeaderboardEntries();
		}
	}

	private void Event_LeaderboardRequestComplete(string leaderboardID, Leaderboards.Entry[] entries)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		m_loadingIndicator.SetActive(false);
		int num = ((entries != null) ? entries.Count((Leaderboards.Entry entry2) => entry2 != null && entry2.m_valid) : 0);
		if (num > 0)
		{
			Leaderboards.Entry entry = entries[entries.Length - 1];
			if (entry != null && entry.m_valid && !entry.m_playersRank)
			{
				num--;
			}
		}
		PopulateHighScoreEntries(num, entries);
		EventDispatch.GenerateEvent("OnLeaderboardPopulated");
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
}
