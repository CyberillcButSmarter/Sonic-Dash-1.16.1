using UnityEngine;

public class Dialog_AmazonBday : MonoBehaviour
{
	private const string PopertyAmazonBdayShown = "AmazonBdayShown";

	private bool shouldShowAmazonBdayDialog;

	public static bool AmazonBdayShown { get; private set; }

	public static void Display()
	{
		DialogStack.ShowDialog("Amazon Bday Dialog");
		AmazonBdayShown = true;
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("FeatureStateReady", this);
		EventDispatch.RegisterInterest("MainMenuActive", this);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
	}

	private void Event_MainMenuActive()
	{
		if (!AmazonBdayShown && shouldShowAmazonBdayDialog)
		{
			Display();
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("AmazonBdayShown", AmazonBdayShown);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		AmazonBdayShown = activeProperties.GetBool("AmazonBdayShown");
	}

	private void Event_FeatureStateReady()
	{
		LSON.Property stateProperty = FeatureState.GetStateProperty("events", "show_amazon_bday_dialog");
		if (stateProperty != null)
		{
		}
	}
}
