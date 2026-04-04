using UnityEngine;

public class IntStorage : MonoBehaviour
{
	[SerializeField]
	private int m_intValue;

	public int Value
	{
		get
		{
			return m_intValue;
		}
		set
		{
			m_intValue = value;
		}
	}
}
