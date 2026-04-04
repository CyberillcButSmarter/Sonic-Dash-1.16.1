using UnityEngine;

public class DialogTriggers : MonoBehaviour
{
	private bool m_finishedOpening;

	private void Trigger_ShowDialog(GameObject callerObject)
	{
		if (callerObject != null)
		{
			ShowDialogProperties component = callerObject.GetComponent<ShowDialogProperties>();
			if (component != null)
			{
				DialogStack.ShowDialog(component.DialogToShow);
			}
		}
	}

	private void Trigger_HideDialog(GameObject callerObject)
	{
		DialogStack.HideDialog();
	}

	private void Trigger_FillBoosters(Object callerObject)
	{
		if (m_finishedOpening)
		{
			m_finishedOpening = false;
			MenuBoosters.s_FillBoosters = true;
		}
		else
		{
			m_finishedOpening = true;
		}
	}
}
