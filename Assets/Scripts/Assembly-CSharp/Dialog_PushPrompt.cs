using UnityEngine;

public class Dialog_PushPrompt : MonoBehaviour
{
	private const string PopertyPushPromptShown = "PushPromptShown";

	public static bool PushPromptShown { get; private set; }

	public static void Display()
	{
		DialogStack.ShowDialog("Push Prompt Dialog");
		PushPromptShown = true;
	}

	private void Start()
	{
	}

	private void Event_MainMenuActive()
	{
		int testValueAsInt = ABTesting.GetTestValueAsInt(ABTesting.Tests.PUSH_RunsBeforeAskMainMenu);
		if (testValueAsInt >= 0 && PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total) >= testValueAsInt && !PushPromptShown)
		{
			Display();
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("PushPromptShown", PushPromptShown);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		if (!activeProperties.DoesPropertyExist("PushPromptShown") && "1.16.1".Equals("1.12.0"))
		{
			PushPromptShown = true;
		}
		else
		{
			PushPromptShown = activeProperties.GetBool("PushPromptShown");
		}
	}

	private void Trigger_AcceptPushNotifications()
	{
	}
}
