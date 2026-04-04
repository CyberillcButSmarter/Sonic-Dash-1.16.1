using UnityEngine;

public class MenuMainMenu : MonoBehaviour
{
	public UILabel m_CountdownLabel;

	private void OnEnable()
	{
		EventDispatch.GenerateEvent("MainMenuActive");
		CloudStorage.Sync();
		SLAnalytics.UpdatePushTags();
		if (OfferState.CanDisplay())
		{
			OfferState.RegisterDisplay();
		}
	}
}
