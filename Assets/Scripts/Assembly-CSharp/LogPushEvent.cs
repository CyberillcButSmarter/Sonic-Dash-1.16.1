using UnityEngine;

public class LogPushEvent : MonoBehaviour
{
	[SerializeField]
	private string Param1 = string.Empty;

	[SerializeField]
	private string Param2 = string.Empty;

	private void OnClick()
	{
		SLAnalytics.LogPushEventWithParameters(Param1, Param2);
	}
}
