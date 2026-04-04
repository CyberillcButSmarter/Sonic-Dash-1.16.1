using System;
using System.Collections;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
	private enum Stage
	{
		Rewards = 0,
		DailyChallenge = 1,
		Missions = 2,
		ScoreCount = 3,
		Hint = 4,
		Leaderboards = 5,
		Buttons = 6
	}

	[Flags]
	private enum State
	{
		Finished = 1
	}

	private const float DefaultSpeedfactor = 3f;

	private const string StateRoot = "socialreminders";

	private const string PropertyRunsBeforeRemindFacebook = "runsbeforeremindfacebook";

	private const string PropertyRunsBetweenRemindFacebook = "runsbetweenremindfacebook";

	private const string PropertyRunsBeforeRemindGPlus = "runsbeforeremindgplus";

	private const string PropertyRunsBetweenRemindGPlus = "runsbetweenremindgplus";

	private const string PropertyLastRunRemindFacebook = "LastRunRemindFacebook";

	private const string PropertyLastRunRemindGPlus = "LastRunRemindGPlus";

	private const float m_DelayBeforeGrantooResults = 4f;

	private State m_state;

	private float m_transitionMultiplier = 3f;

	private AdsDisplay m_resultsAdsDisplay;

	private bool m_requiresGameOverProcess = true;

	private bool[] m_componentActive = new bool[Utils.GetEnumCount<Stage>()];

	private GameOver_Component[] m_components = new GameOver_Component[7]
	{
		new GameOver_Rewards(),
		new GameOver_DailyChallenge(),
		new GameOver_Missions(),
		new GameOver_ScoreCount(),
		new GameOver_Hint(),
		new GameOver_Leaderboards(),
		new GameOver_Buttons()
	};

	[SerializeField]
	private AudioClip m_audioCountingScore;

	[SerializeField]
	private AudioClip m_audioCountingRings;

	[SerializeField]
	private AudioClip m_audioHighScoreAchieved;

	[SerializeField]
	private AudioClip m_audioBoosterScoreBonus;

	[SerializeField]
	private AudioClip m_audioDoubleRingsShown;

	[SerializeField]
	private AudioClip m_audioPanelShown;

	[SerializeField]
	private float m_hintDisplayTime = 3f;

	[SerializeField]
	private float m_speedUpFactor = 3f;

	[SerializeField]
	private OfferRegion_Timed m_offerResultsEnd;

	[SerializeField]
	private int m_runsBeforeRemindFacebook = 1;

	[SerializeField]
	private int m_runsBeforeRemindGPlus = 1;

	[SerializeField]
	private int m_runsBetweenRemindFacebook = 1;

	[SerializeField]
	private int m_runsBetweenRemindGPlus = 1;

	[SerializeField]
	private GameObject m_startGameTrigger;

	[SerializeField]
	private GameObject m_nextMenuTrigger;

	private int m_lastRunRemindFacebook;

	private int m_lastRunRemindGPlus;

	public static bool EndGameActiveAndFinished { get; private set; }

	private void Awake()
	{
		m_componentActive[0] = true;
		m_componentActive[1] = true;
		m_componentActive[2] = true;
		m_componentActive[3] = true;
		m_componentActive[4] = ABTesting.GetTestValueAsInt(ABTesting.Tests.HINT_SkipHints) == 0;
		m_componentActive[5] = true;
		m_componentActive[6] = true;
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		GetSavedValues();
	}

	public void Start()
	{
		EventDispatch.RegisterInterest("OnNewGameStarted", this);
		EventDispatch.RegisterInterest("OnDialogHidden", this);
		EventDispatch.RegisterInterest("OnLeaderboardPopulated", this);
		GameOver_ScoreCount.SetAudioProperties(m_audioCountingScore, m_audioCountingRings, m_audioHighScoreAchieved, m_audioBoosterScoreBonus, m_audioDoubleRingsShown, m_audioPanelShown);
		GameOver_Leaderboards.SetAudioProperties(m_audioPanelShown);
		GameOver_Buttons.SetAudioProperties(m_audioPanelShown);
		GameOver_Hint.SetDisplayTime(m_hintDisplayTime);
	}

	private void OnEnable()
	{
		GetStateValues();
		m_transitionMultiplier = 3f;
		UpdateTweenPositionSpeed(m_transitionMultiplier);
		if (m_requiresGameOverProcess)
		{
			m_state &= ~State.Finished;
			StartCoroutine(RunGameOverFlow());
			m_requiresGameOverProcess = false;
		}
		else
		{
			GameOver_Component[] components = m_components;
			foreach (GameOver_Component gameOver_Component in components)
			{
				gameOver_Component.Show();
			}
			m_state |= State.Finished;
			EndGameActiveAndFinished = true;
		}
		if (m_resultsAdsDisplay == null)
		{
			UIPanel componentInParent = Utils.GetComponentInParent<UIPanel>(base.gameObject);
			if (componentInParent != null)
			{
				m_resultsAdsDisplay = Utils.GetComponentInChildren<AdsDisplay>(componentInParent.gameObject);
			}
		}
		if (m_resultsAdsDisplay != null)
		{
			m_resultsAdsDisplay.Disabled = MenuReviveScreen.VideoReviveUsed;
		}
		int stat = PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total);
		if (((stat > m_runsBeforeRemindGPlus && m_lastRunRemindGPlus == 0) || stat > m_lastRunRemindGPlus + m_runsBetweenRemindGPlus) && !HLSocialPluginAndroid.IsGooglePlusSignedIn() && SLPlugin.IsNetworkConnected())
		{
			DialogStack.ShowDialog("Reminder GPlus Dialog");
			m_lastRunRemindGPlus = stat;
		}
		else if (((stat > m_runsBeforeRemindFacebook && m_lastRunRemindFacebook == 0) || stat > m_lastRunRemindFacebook + m_runsBetweenRemindFacebook) && !((HLLocalUser)Social.localUser).isFacebookAuthenticated && SLPlugin.IsNetworkConnected())
		{
			DialogStack.ShowDialog("Reminder Facebook Dialog");
			m_lastRunRemindFacebook = stat;
		}
		else if (ABTesting.GetTestValueAsInt(ABTesting.Tests.PUSH_AskOnPersonalBest) == 1 && ScoreTracker.HighScoreAchived && !Dialog_PushPrompt.PushPromptShown)
		{
			Dialog_PushPrompt.Display();
		}
	}

	private void OnDisable()
	{
		EndGameActiveAndFinished = false;
		GameOver_Component[] components = m_components;
		foreach (GameOver_Component gameOver_Component in components)
		{
			gameOver_Component.Hide();
		}
	}

	private void Update()
	{
		if (EndGameActiveAndFinished && GrantooManager.GetInstance().ShouldMatchBegin())
		{
			GrantooManager.GetInstance().BeginMatch();
			Trigger_Resume();
		}
	}

	private int FindConcurrentStage(int currentIndex)
	{
		for (int i = currentIndex + 1; i < m_components.Length; i++)
		{
			if (m_componentActive[i])
			{
				return i;
			}
		}
		return -1;
	}

	private IEnumerator RunGameOverFlow()
	{
		int nextStageBack = -1;
		yield return null;
		for (int i = 0; i < m_components.Length; i++)
		{
			if (!m_componentActive[i])
			{
				continue;
			}
			int nextStage = -1;
			if (i == 3)
			{
				nextStage = FindConcurrentStage(i);
			}
			m_components[i].Reset();
			if (nextStage != -1)
			{
				m_components[nextStage].Reset();
			}
			bool sectionFinished = false;
			do
			{
				float timeDelta = IndependantTimeDelta.Delta * m_transitionMultiplier;
				sectionFinished = m_components[i].Update(timeDelta);
				if (nextStage != -1)
				{
					bool concurrentFinished = m_components[nextStage].Update(timeDelta);
					if (concurrentFinished)
					{
						nextStageBack = nextStage;
						nextStage = FindConcurrentStage(nextStage);
						if (nextStage != -1)
						{
							m_components[nextStage].Reset();
							concurrentFinished = false;
						}
					}
					sectionFinished = sectionFinished && concurrentFinished;
				}
				if (GrantooManager.GetInstance().IsActive() && i == 3 && sectionFinished)
				{
					GrantooManager.GetInstance().OnRoundCompleted();
					while (GrantooManager.GetInstance().IsActive() || GrantooManager.GetInstance().IsInGrantooUI())
					{
						yield return null;
					}
				}
				yield return null;
			}
			while (!sectionFinished);
			if (nextStageBack != -1)
			{
				i = nextStageBack;
			}
		}
		GameOver_Component[] components = m_components;
		foreach (GameOver_Component thisComponent in components)
		{
			thisComponent.ProcessFinished();
		}
		m_offerResultsEnd.Visit();
		m_transitionMultiplier = 3f;
		UpdateTweenPositionSpeed(m_transitionMultiplier);
		PropertyStore.Save();
		CloudStorage.Sync();
		m_state |= State.Finished;
		EndGameActiveAndFinished = true;
	}

	private void Event_OnLeaderboardPopulated()
	{
		if (OfferState.CanDisplay())
		{
			OfferState.RegisterDisplay();
		}
	}

	private void UpdateTweenPositionSpeed(float transitionSpeed)
	{
		float num = 0.5f;
		float num2 = 1f / transitionSpeed;
		UIPanel componentInParent = Utils.GetComponentInParent<UIPanel>(base.gameObject);
		UITweener[] componentsInChildren = componentInParent.GetComponentsInChildren<UITweener>(true);
		UITweener[] array = componentsInChildren;
		foreach (UITweener uITweener in array)
		{
			if (uITweener.tweenFactor == 1f || uITweener.tweenFactor == 0f)
			{
				uITweener.duration = num * num2;
			}
		}
	}

	private void Trigger_TransitionFinished(object[] par)
	{
		MonoBehaviour transitioningObject = (MonoBehaviour)par[0];
		GameOver_Component[] components = m_components;
		foreach (GameOver_Component gameOver_Component in components)
		{
			gameOver_Component.TransitionFinished(transitioningObject);
		}
	}

	private void Trigger_BuyDoubleRings()
	{
		if (PowerUpsInventory.GetPowerUpCount(PowerUps.Type.DoubleRing) <= 0 && (m_state & State.Finished) == State.Finished)
		{
			DialogStack.ShowDialog("Buy Double Rings");
		}
	}

	private void Trigger_ComponentClosed()
	{
		GameOver_Component[] components = m_components;
		foreach (GameOver_Component gameOver_Component in components)
		{
			gameOver_Component.ComponentClosed();
		}
	}

	private void Trigger_SpeedUp()
	{
		m_transitionMultiplier = m_speedUpFactor;
		UpdateTweenPositionSpeed(m_transitionMultiplier);
	}

	private void Trigger_Resume()
	{
		int testValueAsInt = ABTesting.GetTestValueAsInt(ABTesting.Tests.BOOSTERS_RunsBeforeUse);
		if (PlayerStats.GetStat(PlayerStats.StatNames.NumberOfSessions_Total) < 2 && PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total) < testValueAsInt)
		{
			m_startGameTrigger.SendMessage("OnClick");
		}
		else
		{
			m_nextMenuTrigger.SendMessage("OnClick");
		}
	}

	private void Trigger_Grantoo()
	{
		GrantooManager.GetInstance().Launch();
	}

	private void Event_OnNewGameStarted()
	{
		m_requiresGameOverProcess = true;
	}

	private void Event_OnDialogHidden()
	{
		GameOver_Rewards.DialogsHidden();
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("LastRunRemindFacebook", m_lastRunRemindFacebook);
		PropertyStore.Store("LastRunRemindGPlus", m_lastRunRemindGPlus);
	}

	private void GetSavedValues()
	{
		ActiveProperties activeProperties = PropertyStore.ActiveProperties();
		m_lastRunRemindFacebook = activeProperties.GetInt("LastRunRemindFacebook");
		m_lastRunRemindGPlus = activeProperties.GetInt("LastRunRemindGPlus");
	}

	private void GetStateValues()
	{
		if (FeatureState.Valid)
		{
			LSON.Property stateProperty = FeatureState.GetStateProperty("socialreminders", "runsbeforeremindfacebook");
			int intValue;
			if (stateProperty != null && LSONProperties.AsInt(stateProperty, out intValue) && intValue >= 0)
			{
				m_runsBeforeRemindFacebook = intValue;
			}
			LSON.Property stateProperty2 = FeatureState.GetStateProperty("socialreminders", "runsbetweenremindfacebook");
			if (stateProperty2 != null && LSONProperties.AsInt(stateProperty2, out intValue) && intValue >= 0)
			{
				m_runsBetweenRemindFacebook = intValue;
			}
			LSON.Property stateProperty3 = FeatureState.GetStateProperty("socialreminders", "runsbeforeremindgplus");
			if (stateProperty3 != null && LSONProperties.AsInt(stateProperty3, out intValue) && intValue >= 0)
			{
				m_runsBeforeRemindGPlus = intValue;
			}
			LSON.Property stateProperty4 = FeatureState.GetStateProperty("socialreminders", "runsbetweenremindgplus");
			if (stateProperty4 != null && LSONProperties.AsInt(stateProperty4, out intValue) && intValue >= 0)
			{
				m_runsBetweenRemindGPlus = intValue;
			}
		}
	}
}
