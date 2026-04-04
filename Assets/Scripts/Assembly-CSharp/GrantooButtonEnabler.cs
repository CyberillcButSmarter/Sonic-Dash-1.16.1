using UnityEngine;

public class GrantooButtonEnabler : MonoBehaviour
{
	[SerializeField]
	private UISprite m_ButtonSprite;

	[SerializeField]
	private GameObject m_NotificationCounter;

	protected bool m_bEnabled = true;

	private void Start()
	{
		UIButtonSound[] components = GetComponents<UIButtonSound>();
		components[2].enabled = false;
	}

	private void Update()
	{
		bool flag = Application.internetReachability != NetworkReachability.NotReachable;
		bool flag2 = TutorialSystem.instance().isTrackTutorialEnabled();
		int testValueAsInt = ABTesting.GetTestValueAsInt(ABTesting.Tests.BOOSTERS_RunsBeforeUse);
		bool flag3 = PlayerStats.GetStat(PlayerStats.StatNames.NumberOfSessions_Total) < 2 && PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total) < testValueAsInt;
		if (flag2 || flag3)
		{
			flag = false;
		}
		if (flag != m_bEnabled)
		{
			m_bEnabled = flag;
			NGUITools.SetActive(m_NotificationCounter, flag);
			m_ButtonSprite.spriteName = ((!flag) ? "button_disabled" : "button_orange_med");
			GetComponent<GuiButtonBlocker>().Blocked = !flag;
			GetComponent<UIButtonScale>().enabled = flag;
			GetComponent<UIButtonMessage>().enabled = flag;
			GetComponent<UIButtonColor>().enabled = flag;
			UIButtonSound[] components = GetComponents<UIButtonSound>();
			components[0].enabled = flag;
			components[1].enabled = flag;
			components[2].enabled = !flag;
		}
	}
}
