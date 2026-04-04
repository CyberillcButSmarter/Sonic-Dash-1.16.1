using System.Collections;
using UnityEngine;

public class Dialog_EspioSpecialEvent : MonoBehaviour
{
	public enum DialogMode
	{
		Progress = 0,
		Success = 1
	}

	private static DialogMode s_eDialogMode;

	[SerializeField]
	private GameObject m_contentProgressParent;

	[SerializeField]
	private GameObject m_contentResultParent;

	[SerializeField]
	private UILabel m_tokensNeededLabel;

	[SerializeField]
	private UILabel m_resultsInfoLabel;

	[SerializeField]
	private GameObject m_congratsObject;

	public static void Display(DialogMode eMode)
	{
		s_eDialogMode = eMode;
		DialogStack.ShowDialog("Espio Special Event Dialog");
	}

	private void OnEnable()
	{
		StartCoroutine(StartPendingActivation());
	}

	private IEnumerator StartPendingActivation()
	{
		yield return null;
		UpdateContent();
	}

	private void UpdateContent()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (s_eDialogMode == DialogMode.Progress)
		{
			NGUITools.SetActive(m_contentProgressParent, true);
			NGUITools.SetActive(m_contentResultParent, false);
			string text = ((EspioSpecialEvent.GetTokensNeeded() != EspioSpecialEvent.GetTotalTokensNeeded()) ? string.Format(LanguageStrings.First.GetString("ESPIO_EVENT_PROGRESS_BODY_LOWER"), EspioSpecialEvent.GetTokensNeeded()) : string.Format(LanguageStrings.First.GetString("ESPIO_EVENT_INTRO_BODY_LOWER"), EspioSpecialEvent.GetTokensNeeded()));
			m_tokensNeededLabel.text = text;
			return;
		}
		NGUITools.SetActive(m_contentProgressParent, false);
		NGUITools.SetActive(m_contentResultParent, true);
		if (s_eDialogMode == DialogMode.Success)
		{
			NGUITools.SetActive(m_congratsObject, true);
			m_resultsInfoLabel.text = LanguageStrings.First.GetString("ESPIO_EVENT_SUCCESS_BODY");
		}
	}

	private void Update()
	{
	}
}
