using System.Collections;
using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
	public bool immediately;

	private void OnEnable()
	{
		if (immediately)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			StartCoroutine(DelayDisable());
		}
	}

	private IEnumerator DelayDisable()
	{
		yield return null;
		base.gameObject.SetActive(false);
	}
}
