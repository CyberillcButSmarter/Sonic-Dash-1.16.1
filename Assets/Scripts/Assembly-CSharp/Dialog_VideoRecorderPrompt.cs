using UnityEngine;

public class Dialog_VideoRecorderPrompt : MonoBehaviour
{
	private const string PopertyVideoRecorderPromptShownRun = "VideoRecorderPromptShownRun";

	[SerializeField]
	private int m_showEveryRuns;

	private static Dialog_VideoRecorderPrompt instance;

	public static int LastRunShown { get; set; }

	public static bool CheckIfDisplay()
	{
		return VideoRecorder.Useable && LastRunShown + instance.m_showEveryRuns <= PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total);
	}

	public static void Display()
	{
		LastRunShown = PlayerStats.GetStat(PlayerStats.StatNames.NumberOfRuns_Total);
		DialogStack.ShowDialog("VideoRecorder Prompt Dialog");
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		instance = this;
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("VideoRecorderPromptShownRun", LastRunShown);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		LastRunShown = activeProperties.GetInt("VideoRecorderPromptShownRun");
	}
}
