using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStack : MonoBehaviour
{
	private enum TransitionDirection
	{
		Forward = 0,
		Backwards = 1
	}

	private static GuiTrigger s_requestedStartPage;

	private static MenuStack s_menuStack;

	[SerializeField]
	private GuiTrigger m_defaultStartPage;

	[SerializeField]
	private GuiTrigger m_defaultGamePage;

	[SerializeField]
	private GuiTrigger m_dcPage;

	[SerializeField]
	private iTweenPath m_dcTransition;

	[SerializeField]
	private iTweenPath m_storeTransition;

	private bool m_useDCPage;

	private bool m_dcPageDisplayed;

	private string m_propertyDCPageDisplayed = "DCPageDisplayed";

	[SerializeField]
	private GuiTrigger m_wheelOfFortunePage;

	[SerializeField]
	private GuiTrigger m_gcProgressPage;

	private Stack<GuiTrigger> m_menuStack;

	private Stack<iTweenPath> m_transitionStack;

	private int m_menuCapacity = 5;

	private GuiTrigger m_pendingPage;

	private iTweenPath m_pendingTransition;

	private bool m_replaceCurrentPage;

	private TransitionDirection m_transitionDirection;

	private MenuCamera m_menuCamera;

	private bool m_processNotificationsURLInResults;

	private int m_framesAfterTransition;

	public static GuiTrigger RequestPage
	{
		set
		{
			s_requestedStartPage = value;
		}
	}

	public static void MoveToPage(GuiTrigger menuPage, bool replaceCurrent)
	{
		s_menuStack.MoveToPage(menuPage, null, false, replaceCurrent);
	}

	public void MoveToPage(GuiTrigger menuPage, iTweenPath transitionPath, bool transitionPathInReverse, bool replaceCurrentPage)
	{
		if (!FeatureSupport.IsSupported("Extended Menus"))
		{
			transitionPath = null;
		}
		m_pendingPage = menuPage;
		m_pendingTransition = transitionPath;
		m_replaceCurrentPage = replaceCurrentPage;
		if (m_replaceCurrentPage)
		{
		}
		m_transitionDirection = (transitionPathInReverse ? TransitionDirection.Backwards : TransitionDirection.Forward);
		if (m_pendingPage == null)
		{
			GuiTrigger[] array = m_menuStack.ToArray();
			iTweenPath[] array2 = m_transitionStack.ToArray();
			m_pendingPage = array[1];
			m_pendingTransition = array2[0];
			m_transitionDirection = TransitionDirection.Backwards;
		}
		if (m_menuStack.Count > 0)
		{
			GuiTrigger guiTrigger = m_menuStack.Peek();
			guiTrigger.Hide();
		}
		StartCoroutine(UpdatePageTransitions(m_transitionDirection, m_pendingPage.CharacterStandPoint));
	}

	public void CloseMenuSystem()
	{
		if (m_menuStack.Count != 0)
		{
			GuiTrigger guiTrigger = m_menuStack.Peek();
			guiTrigger.Hide();
			m_menuStack.Clear();
		}
	}

	private void OnNukingLevel()
	{
		s_requestedStartPage = null;
		s_menuStack = null;
	}

	private void Start()
	{
		s_menuStack = this;
		EventDispatch.RegisterInterest("DisableGameState", this);
		EventDispatch.RegisterInterest("StartGameState", this);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this, EventDispatch.Priority.Low);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		s_requestedStartPage = null;
		m_pendingPage = null;
		m_menuStack = new Stack<GuiTrigger>(m_menuCapacity);
		m_transitionStack = new Stack<iTweenPath>(m_menuCapacity);
		m_transitionDirection = TransitionDirection.Forward;
		m_menuCamera = GetComponent<MenuCamera>();
		m_framesAfterTransition = 0;
		m_processNotificationsURLInResults = !FeatureSupport.IsSupported("ReduceMemUsageForSDKs");
	}

	private GuiTrigger GetDefaultPage(GameState.Mode forMode)
	{
		if (s_requestedStartPage != null)
		{
			return s_requestedStartPage;
		}
		return (forMode != GameState.Mode.PauseMenu) ? m_defaultStartPage : m_defaultGamePage;
	}

	private GuiTrigger GetPageFromName(string pageName)
	{
		GameObject gameObject = GameObject.Find("Menu Pages/" + pageName);
		if (gameObject != null)
		{
			GuiTrigger component = gameObject.GetComponent<GuiTrigger>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	private void ShowPage(GuiTrigger menuPage, iTweenPath pageTransition)
	{
		if (menuPage == null)
		{
			return;
		}
		if (m_transitionDirection == TransitionDirection.Backwards)
		{
			GuiTrigger[] array = m_menuStack.ToArray();
			int num = Array.IndexOf(array, menuPage);
			for (int i = 0; i < num; i++)
			{
				m_menuStack.Pop();
				m_transitionStack.Pop();
			}
		}
		else if (m_replaceCurrentPage)
		{
			m_menuStack.Pop();
			m_menuStack.Push(menuPage);
		}
		else
		{
			m_menuStack.Push(menuPage);
			m_transitionStack.Push(pageTransition);
		}
		menuPage.Show();
	}

	private IEnumerator UpdatePageTransitions(TransitionDirection direction, Transform characterStandPoint)
	{
		if (m_pendingPage != null)
		{
			CameraTypeSpline.Direction cameraDirection = ((direction != TransitionDirection.Forward) ? CameraTypeSpline.Direction.Backwards : CameraTypeSpline.Direction.Forward);
			yield return StartCoroutine(StartAndWaitForCameraTransition(cameraDirection));
			yield return StartCoroutine(WaitForOutboundPageTransition());
			ShowPage(m_pendingPage, m_pendingTransition);
			m_pendingPage = null;
			m_pendingTransition = null;
		}
	}

	private void DisplayMenuStack()
	{
	}

	private void Update()
	{
		if (m_pendingPage != null || m_menuStack.Count <= 0)
		{
			return;
		}
		if (m_menuStack.Peek().Transition)
		{
			m_framesAfterTransition = 0;
			return;
		}
		m_framesAfterTransition++;
		if (m_framesAfterTransition < 8 || (!(m_menuStack.Peek() == m_defaultStartPage) && !GameOverScreen.EndGameActiveAndFinished))
		{
			return;
		}
		bool flag = m_processNotificationsURLInResults;
		if (m_menuStack.Peek() == m_defaultStartPage)
		{
			flag = true;
		}
		DeepLink deepLink = DeepLink.GetDeepLink();
		bool flag2 = m_menuStack.Peek() == m_defaultStartPage || (GameOverScreen.EndGameActiveAndFinished && flag);
		string pageName;
		if (deepLink != null && flag2 && !deepLink.HasBeenUsed && deepLink.HasPageName(out pageName))
		{
			GuiTrigger pageFromName = GetPageFromName(pageName);
			if (pageFromName != null)
			{
				MoveToPage(pageFromName, null, false, false);
			}
			deepLink.HasBeenUsed = true;
		}
		else
		{
			ProcessNotifications(flag);
		}
	}

	private void ProcessNotifications(bool processURL)
	{
	}

	private void Event_StartGameState(GameState.Mode nextState)
	{
		if (nextState == GameState.Mode.Game)
		{
			CloseMenuSystem();
			return;
		}
		GuiTrigger defaultPage = GetDefaultPage(nextState);
		WheelOfFortuneSettings.Instance.GetSecondsRemaining();
		DeepLink deepLink = DeepLink.GetDeepLink();
		string pageName;
		if (deepLink != null && !deepLink.HasBeenUsed && deepLink.HasPageName(out pageName))
		{
			Characters.UpdateCharacterVisibility();
			GuiTrigger pageFromName = GetPageFromName(pageName);
			if (pageFromName == null)
			{
				MoveToRequestedOr(defaultPage);
			}
			else
			{
				m_menuStack.Push(defaultPage);
				MoveToPage(pageFromName, m_storeTransition, false, false);
			}
			deepLink.HasBeenUsed = true;
		}
		else if (GC6Progress.IsRewardDue() && nextState != GameState.Mode.PauseMenu)
		{
			m_menuStack.Push(defaultPage);
			MoveToPage(m_gcProgressPage, m_dcTransition, false, false);
		}
		else if (WheelOfFortuneSettings.Instance.HasFreeSpin && !WheelOfFortuneSettings.Instance.KnowAboutFreeSpin && nextState != GameState.Mode.PauseMenu && BoostersBreadcrumb.Instance.BoostersHaveArrivedDialogShown && !GCDialogManager.ShouldShowChallenegeActiveDialog() && PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Session) > 0)
		{
			m_menuStack.Push(defaultPage);
			MoveToPage(m_wheelOfFortunePage, m_dcTransition, false, false);
		}
		else if (m_useDCPage && !m_dcPageDisplayed && nextState != GameState.Mode.PauseMenu)
		{
			m_menuStack.Push(defaultPage);
			MoveToPage(m_dcPage, m_dcTransition, false, false);
			m_dcPageDisplayed = true;
		}
		else
		{
			MoveToRequestedOr(defaultPage);
		}
		s_requestedStartPage = null;
	}

	private void MoveToRequestedOr(GuiTrigger page)
	{
		if (s_requestedStartPage == null)
		{
			MoveToPage(page, null, false, false);
		}
		else
		{
			MoveToPage(s_requestedStartPage, null, false, false);
		}
	}

	private void Event_DisableGameState(GameState.Mode nextState)
	{
		CloseMenuSystem();
	}

	private IEnumerator StartAndWaitForCameraTransition(CameraTypeSpline.Direction direction)
	{
		if ((bool)m_menuCamera && (bool)m_pendingTransition)
		{
			m_menuCamera.StartCameraTransition(m_pendingTransition, iTween.EaseType.easeInOutSine, iTween.EaseType.easeInOutSine, direction);
			do
			{
				yield return null;
			}
			while (m_menuCamera.InTransition);
		}
	}

	private IEnumerator WaitForOutboundPageTransition()
	{
		GuiTrigger outboundPage = null;
		if (m_menuStack.Count > 0)
		{
			outboundPage = m_menuStack.Peek();
		}
		if (outboundPage != null)
		{
			do
			{
				yield return null;
			}
			while (outboundPage.Transition);
		}
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		m_useDCPage = false;
		m_dcPageDisplayed = activeProperties.GetBool(m_propertyDCPageDisplayed);
		if (!m_dcPageDisplayed)
		{
			PlayerStats.Stats currentStats = PlayerStats.GetCurrentStats();
			if (currentStats.m_trackedStats[0] > 3 && currentStats.m_trackedStats[83] == 0)
			{
				m_useDCPage = true;
			}
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store(m_propertyDCPageDisplayed, m_dcPageDisplayed);
	}
}
