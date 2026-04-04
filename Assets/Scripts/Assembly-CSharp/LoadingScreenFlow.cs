using System;
using System.Collections;
using UnityEngine;

public class LoadingScreenFlow : MonoBehaviour
{
	public delegate void TransitionEvent(int identIndex, int indexCount);

	private delegate void TransitionState();

	private const float FixedDelayTime = 0.5f;

	private const int DebugTapTargetCount = 3;

	private const float DebugTapWindowSeconds = 1f;

	private const float DebugToastSlideDuration = 0.35f;

	private const float DebugToastHoldSeconds = 1.1f;

	private const float DebugToastBottomOffset = 55f;

	private const float DebugToastHiddenOffset = -40f;

	private const float DebugToastShownOffset = 10f;

	[SerializeField]
	private UISprite m_background;

	private LoadingScreenType[] m_identDisplays;

	private float m_thisLoadingScreenTime;

	private bool m_firstScreen = true;

	private float m_transitionValue;

	private float m_transitionRate;

	private float m_displayTime;

	private float m_delayTime;

	private int m_currentIndex;

	private TransitionEvent m_transitionInStart;

	private TransitionEvent m_transitionInFinish;

	private TransitionEvent m_transitionOutStart;

	private TransitionEvent m_transitionOutFinish;

	private TransitionState m_transitionState;

	private bool showBanner;

	private int m_debugTapCount;

	private float m_debugTapTimer;

	private UILabel m_debugToastLabel;

	private GameObject m_debugToastRoot;

	public TransitionEvent TransitionInStart
	{
		set
		{
			m_transitionInStart = value;
		}
	}

	public TransitionEvent TransitionInEnd
	{
		set
		{
			m_transitionInFinish = value;
		}
	}

	public TransitionEvent TransitionOutStart
	{
		set
		{
			m_transitionOutStart = value;
		}
	}

	public TransitionEvent TransitionOutEnd
	{
		set
		{
			m_transitionOutFinish = value;
		}
	}

	public bool AssetsLoaded { private get; set; }

	public bool AllowDebugToggle { private get; set; }

	private bool OnLastIdent
	{
		get
		{
			return m_currentIndex == m_identDisplays.Length - 1;
		}
	}

	public void StartFlow(bool delayPresentation)
	{
		m_currentIndex = 0;
		if (delayPresentation)
		{
			m_transitionState = StartTransitionDelay;
		}
		else
		{
			m_transitionState = StartTransitionIn;
		}
	}

	private void Start()
	{
		m_identDisplays = GetComponentsInChildren<LoadingScreenType>(true);
		for (int i = 0; i < m_identDisplays.Length; i++)
		{
			m_identDisplays[i].SetTransitionState(0f);
			m_identDisplays[i].gameObject.SetActive(false);
		}
		Array.Sort(m_identDisplays, delegate(LoadingScreenType screen1, LoadingScreenType screen2)
		{
			LoadingScreenProperties component = screen1.GetComponent<LoadingScreenProperties>();
			LoadingScreenProperties component2 = screen2.GetComponent<LoadingScreenProperties>();
			return component.ScreenOrder.CompareTo(component2.ScreenOrder);
		});
	}

	private void Update()
	{
		if (m_transitionState != null)
		{
			m_thisLoadingScreenTime += Time.deltaTime;
			m_transitionState();
		}
		HandleDebugToggleInput();
	}

	private void StartTransitionDelay()
	{
		m_delayTime = 0f;
		m_transitionState = UpdateTransitionDelay;
	}

	private void UpdateTransitionDelay()
	{
		m_delayTime += Time.deltaTime;
		if (m_delayTime >= 0.5f)
		{
			m_transitionState = StartTransitionIn;
		}
	}

	private void StartTransitionIn()
	{
		m_transitionValue = 0f;
		LoadingScreenType loadingScreenType = m_identDisplays[m_currentIndex];
		loadingScreenType.gameObject.SetActive(true);
		loadingScreenType.SetTransitionState(m_transitionValue);
		LoadingScreenProperties component = loadingScreenType.GetComponent<LoadingScreenProperties>();
		m_transitionRate = 1f / component.TransitionTime;
		m_transitionState = UpdateTransitionIn;
		if (m_transitionInStart != null)
		{
			m_transitionInStart(m_currentIndex, m_identDisplays.Length - 1);
		}
		m_thisLoadingScreenTime = 0f;
	}

	private void UpdateTransitionIn()
	{
		LoadingScreenType loadingScreenType = m_identDisplays[m_currentIndex];
		m_transitionValue += m_transitionRate * Time.deltaTime;
		m_transitionValue = Mathf.Clamp(m_transitionValue, 0f, 1f);
		loadingScreenType.SetTransitionState(m_transitionValue);
		if (m_transitionValue.Equals(1f))
		{
			if (m_transitionInFinish != null)
			{
				m_transitionInFinish(m_currentIndex, m_identDisplays.Length - 1);
			}
			m_transitionState = StartDisplay;
		}
	}

	private void StartDisplay()
	{
		if (showBanner && !PaidUser.RemovedAds)
		{
			SLAds.ShowBannerAd(1);
		}
		m_displayTime = 0f;
		m_transitionState = UpdateDisplay;
	}

	private void UpdateDisplay()
	{
		m_displayTime += Time.deltaTime;
		bool flag = false;
		if (OnLastIdent)
		{
			flag = !AssetsLoaded;
		}
		LoadingScreenType loadingScreenType = m_identDisplays[m_currentIndex];
		LoadingScreenProperties component = loadingScreenType.GetComponent<LoadingScreenProperties>();
		if (m_displayTime >= component.DisplayTime && !flag)
		{
			m_transitionState = StartTransitionOut;
		}
	}

	private void StartTransitionOut()
	{
		if (showBanner && !PaidUser.RemovedAds)
		{
			SLAds.CloseBannerAd();
		}
		showBanner = true;
		m_transitionValue = 1f;
		m_transitionState = UpdateTransitionOut;
		if (m_transitionOutStart != null)
		{
			m_transitionOutStart(m_currentIndex, m_identDisplays.Length - 1);
		}
	}

	private void UpdateTransitionOut()
	{
		m_transitionValue -= m_transitionRate * Time.deltaTime;
		m_transitionValue = Mathf.Clamp(m_transitionValue, 0f, 1f);
		if ((bool)m_background && OnLastIdent)
		{
			Color color = m_background.color;
			color.a = m_transitionValue;
			m_background.color = color;
		}
		LoadingScreenType loadingScreenType = m_identDisplays[m_currentIndex];
		loadingScreenType.SetTransitionState(m_transitionValue);
		if (m_transitionValue.Equals(0f))
		{
			m_transitionState = FinishTransitionOut;
			if (m_transitionOutFinish != null)
			{
				m_transitionOutFinish(m_currentIndex, m_identDisplays.Length - 1);
			}
		}
	}

	private void OnDestroy()
	{
		if (m_transitionState != null)
		{
			FinishTransitionOut();
		}
	}

	private void FinishTransitionOut()
	{
		LoadingScreenType loadingScreenType = m_identDisplays[m_currentIndex];
		loadingScreenType.gameObject.SetActive(false);
		if (OnLastIdent)
		{
			m_transitionState = null;
		}
		else
		{
			m_currentIndex++;
			m_transitionState = StartTransitionIn;
		}
		LoadingScreenProperties component = loadingScreenType.GetComponent<LoadingScreenProperties>();
		GameAnalytics.NotifyLoadScreen(component, m_thisLoadingScreenTime, m_firstScreen);
		m_firstScreen = false;
	}

	private void HandleDebugToggleInput()
	{
		if (!AllowDebugToggle || DebugSession.Enabled)
		{
			return;
		}
		if (m_debugTapTimer > 0f)
		{
			m_debugTapTimer -= Time.deltaTime;
			if (m_debugTapTimer <= 0f)
			{
				m_debugTapTimer = 0f;
				m_debugTapCount = 0;
			}
		}
		if (!IsTapDown())
		{
			return;
		}
		if (m_debugTapTimer <= 0f)
		{
			m_debugTapTimer = DebugTapWindowSeconds;
			m_debugTapCount = 0;
		}
		m_debugTapCount++;
		if (m_debugTapCount >= DebugTapTargetCount)
		{
			m_debugTapCount = 0;
			m_debugTapTimer = 0f;
			EnableDebugModeForSession();
		}
	}

	private bool IsTapDown()
	{
		if (Input.GetMouseButtonDown(0))
		{
			return true;
		}
		for (int i = 0; i < Input.touchCount; i++)
		{
			if (Input.GetTouch(i).phase == TouchPhase.Began)
			{
				return true;
			}
		}
		return false;
	}

	private void EnableDebugModeForSession()
	{
		Debug.Log("Debug mode enabled for this session.");
		DebugSession.EnableForSession();
		StartCoroutine(PlayDebugEnabledToast());
	}

	private IEnumerator PlayDebugEnabledToast()
	{
		UILabel label = EnsureDebugToastLabel();
		if (label == null)
		{
			yield break;
		}
		label.text = "Debug Mode Enabled! (For this session)";
		label.gameObject.SetActive(true);
		Vector3 hiddenPosition = new Vector3(0f, DebugToastHiddenOffset, 0f);
		Vector3 shownPosition = new Vector3(0f, DebugToastShownOffset, 0f);
		label.transform.localPosition = hiddenPosition;
		TweenPosition tweenIn = TweenPosition.Begin(label.gameObject, DebugToastSlideDuration, shownPosition);
		tweenIn.method = UITweener.Method.EaseOut;
		tweenIn.ignoreTimeScale = true;
		yield return WaitRealSeconds(DebugToastHoldSeconds);
		TweenPosition tweenOut = TweenPosition.Begin(label.gameObject, DebugToastSlideDuration, hiddenPosition);
		tweenOut.method = UITweener.Method.EaseIn;
		tweenOut.ignoreTimeScale = true;
		yield return WaitRealSeconds(DebugToastSlideDuration);
		label.gameObject.SetActive(false);
	}

	private IEnumerator WaitRealSeconds(float seconds)
	{
		float endTime = Time.realtimeSinceStartup + seconds;
		while (Time.realtimeSinceStartup < endTime)
		{
			yield return null;
		}
	}

	private UILabel EnsureDebugToastLabel()
	{
		if (m_debugToastLabel != null)
		{
			return m_debugToastLabel;
		}
		UILabel template = GetComponentInChildren<UILabel>(true);
		if (template == null)
		{
			return null;
		}
		m_debugToastRoot = new GameObject("DebugToastAnchor");
		m_debugToastRoot.layer = base.gameObject.layer;
		m_debugToastRoot.transform.parent = base.transform;
		m_debugToastRoot.transform.localPosition = Vector3.zero;
		m_debugToastRoot.transform.localScale = Vector3.one;
		UIAnchor anchor = m_debugToastRoot.AddComponent<UIAnchor>();
		anchor.side = UIAnchor.Side.Bottom;
		anchor.runOnlyOnce = false;
		anchor.pixelOffset = new Vector2(0f, DebugToastBottomOffset);
		GameObject labelObject = new GameObject("DebugToastLabel");
		labelObject.layer = base.gameObject.layer;
		labelObject.transform.parent = m_debugToastRoot.transform;
		labelObject.transform.localPosition = Vector3.zero;
		labelObject.transform.localScale = Vector3.one;
		m_debugToastLabel = labelObject.AddComponent<UILabel>();
		CopyLabelStyle(template, m_debugToastLabel);
		m_debugToastLabel.alignment = NGUIText.Alignment.Center;
		m_debugToastLabel.pivot = UIWidget.Pivot.Bottom;
		m_debugToastLabel.overflowMethod = UILabel.Overflow.ClampContent;
		m_debugToastLabel.width = Mathf.Max(600, template.width);
		m_debugToastLabel.height = Mathf.Max(60, template.height);
		labelObject.SetActive(false);
		return m_debugToastLabel;
	}

	private void CopyLabelStyle(UILabel source, UILabel target)
	{
		target.bitmapFont = source.bitmapFont;
		target.trueTypeFont = source.trueTypeFont;
		target.fontSize = source.fontSize;
		target.fontStyle = source.fontStyle;
		target.keepCrispWhenShrunk = source.keepCrispWhenShrunk;
		target.effectStyle = source.effectStyle;
		target.effectColor = source.effectColor;
		target.effectDistance = source.effectDistance;
		target.applyGradient = source.applyGradient;
		target.gradientTop = source.gradientTop;
		target.gradientBottom = source.gradientBottom;
		target.spacingX = source.spacingX;
		target.spacingY = source.spacingY;
		target.color = source.color;
		target.depth = source.depth + 10;
	}
}
