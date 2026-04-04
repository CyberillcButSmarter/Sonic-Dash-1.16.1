using UnityEngine;

public class GoogleSpecialEventCharacterOverlay : MonoBehaviour
{
	[SerializeField]
	private UILabel m_labelRunCount;

	public void Refresh()
	{
		int totalRunsNeeded = GoogleSpecialEvent.GetTotalRunsNeeded();
		int runsNeeded = GoogleSpecialEvent.GetRunsNeeded();
		int num = totalRunsNeeded - runsNeeded;
		m_labelRunCount.text = string.Format(LanguageStrings.First.GetString("GOOGLE_EVENT_RUNS_COUNTER"), num, totalRunsNeeded);
	}
}
