using UnityEngine;

public class Dialog_GCPendingReward : MonoBehaviour
{
	private const string NormalReward = "GC2_REWARD_INFO";

	private const string StarReward = "GC2_REWARD_INFO2";

	private static Dialog_GCPendingReward instance;

	[SerializeField]
	private MoveToPageProperties m_properties;

	[SerializeField]
	private UILabel m_text;

	private iTweenPath m_storedPath;

	public static void Display()
	{
		if (GameState.GetMode() != GameState.Mode.Menu)
		{
			instance.m_properties.TransitionPath = null;
		}
		else
		{
			instance.m_properties.TransitionPath = instance.m_storedPath;
		}
		LocalisedStringProperties component = instance.m_text.GetComponent<LocalisedStringProperties>();
		string text = ((GC6Progress.GetGC6LocalTierCurrent() != 4 || GC6Progress.GetGC6GlobalTierCurrent() != 4) ? component.SetLocalisationID("GC2_REWARD_INFO") : component.SetLocalisationID("GC2_REWARD_INFO2"));
		instance.m_text.text = text;
		DialogStack.ShowDialog("GC Pending Reward Dialog");
	}

	private void Start()
	{
		instance = this;
		m_storedPath = m_properties.TransitionPath;
	}
}
