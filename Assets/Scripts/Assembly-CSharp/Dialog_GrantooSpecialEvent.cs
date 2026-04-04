using UnityEngine;

public class Dialog_GrantooSpecialEvent : MonoBehaviour
{
	public UILabel[] m_CountdownLabels;

	public static bool Display()
	{
		if (DialogStack.IsInitialized())
		{
			DialogStack.ShowDialog("Grantoo Special Event Dialog");
			return true;
		}
		return false;
	}

	private void OnEnable()
	{
		if (m_CountdownLabels != null)
		{
			UILabel[] countdownLabels = m_CountdownLabels;
			foreach (UILabel uILabel in countdownLabels)
			{
				uILabel.GetComponent<MultiPlayerEventCountDown>().RunCountDown();
			}
		}
	}
}
