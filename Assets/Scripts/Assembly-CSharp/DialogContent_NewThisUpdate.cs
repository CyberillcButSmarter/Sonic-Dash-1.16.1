using UnityEngine;

public class DialogContent_NewThisUpdate : MonoBehaviour
{
	[SerializeField]
	public GameObject m_KamcordEntry;

	private void OnEnable()
	{
		m_KamcordEntry.SetActive(false);
	}
}
