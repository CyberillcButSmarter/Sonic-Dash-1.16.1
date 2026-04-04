using System.Collections;
using UnityEngine;

public class GC6ButtonAnimationDisplayer : MonoBehaviour
{
	public static bool m_keepActive;

	[SerializeField]
	private UISprite m_target;

	[SerializeField]
	private Animation m_animation;

	private void OnEnable()
	{
		if (GC6Progress.PageVisitedFromResult || !m_keepActive)
		{
			StartCoroutine(HideButtons());
		}
	}

	private IEnumerator HideButtons()
	{
		do
		{
			yield return null;
		}
		while (!m_target.gameObject.activeInHierarchy);
		m_target.alpha = 0f;
		m_animation.enabled = false;
	}
}
