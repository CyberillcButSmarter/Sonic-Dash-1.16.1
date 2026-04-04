using UnityEngine;

public class Dialog_NewThisVersion : MonoBehaviour
{
	private const string m_newThisVersionDialogSaveEntry = "NewThisVersionDialogShown";

	private bool[] m_hasBeenShownThisUpdate = new bool[2];

	private string m_hasBeenShownThisUpdateState = string.Empty;

	private bool m_checkedThisSession;

	[SerializeField]
	public bool m_showThisUpdate;

	[SerializeField]
	public bool m_showForPointUpdates;

	public static void Display()
	{
		DialogStack.ShowDialog("New This Update Dialog");
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		EventDispatch.RegisterInterest("MainMenuActive", this);
	}

	private bool ValidToDisplay()
	{
		m_checkedThisSession = true;
		m_hasBeenShownThisUpdateState = "True,1.16.1";
		return false;
	}

	private void Event_MainMenuActive()
	{
		if (!m_checkedThisSession && ValidToDisplay())
		{
			DialogStack.ShowDialog("New This Update Dialog");
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("NewThisVersionDialogShown", m_hasBeenShownThisUpdateState);
	}

	private void Event_OnGameDataLoaded(ActiveProperties ap)
	{
		m_hasBeenShownThisUpdateState = ap.GetString("NewThisVersionDialogShown");
		if (m_hasBeenShownThisUpdateState == null || m_hasBeenShownThisUpdateState == string.Empty)
		{
			m_hasBeenShownThisUpdateState = "True,1.16.1";
		}
	}
}
