using UnityEngine;

public class EspioSpecialEventCharacterOverlay : MonoBehaviour
{
	[SerializeField]
	private UILabel m_labelRunCount;

	public void Refresh()
	{
		int totalTokensNeeded = EspioSpecialEvent.GetTotalTokensNeeded();
		int tokensNeeded = EspioSpecialEvent.GetTokensNeeded();
		int num = totalTokensNeeded - tokensNeeded;
		m_labelRunCount.text = string.Format(LanguageStrings.First.GetString("ESPIO_EVENT_TOKEN_COUNTER"), num, totalTokensNeeded);
	}
}
