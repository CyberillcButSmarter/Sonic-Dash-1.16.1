using UnityEngine;

public class MakePurchaseProperties : MonoBehaviour
{
	[SerializeField]
	private string m_storeOfferID;

	public string OfferID
	{
		get
		{
			return m_storeOfferID;
		}
	}
}
