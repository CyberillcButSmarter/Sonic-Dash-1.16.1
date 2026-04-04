using System;
using UnityEngine;

public class DialogContent_GlobalChallenegeActive : MonoBehaviour
{
	[SerializeField]
	private UILabel m_announceDescLabel;

	private void OnEnable()
	{
		UpdateDescription();
	}

	private void UpdateDescription()
	{
		LocalisedStringProperties component = m_announceDescLabel.GetComponent<LocalisedStringProperties>();
		DateTime challengeDate = GCState.GetChallengeDate(GCState.Challenges.gc6);
		string text = string.Format("{0: h:mm tt (UTC), dddd d MMMM}", challengeDate);
		Language.Locale locale = Language.GetLocale();
		if (locale == Language.Locale.US)
		{
			challengeDate = challengeDate.AddHours(-8.0);
			text = string.Format("{0: h:mm tt (PST), dddd d MMMM}", challengeDate);
		}
		text = text.ToUpper();
		string text2 = string.Format(component.GetLocalisedString(), text);
		m_announceDescLabel.text = text2;
	}
}
