using UnityEngine;

public class Dialog_PromoSpecialEvent : MonoBehaviour
{
	private static PromoSpecialEvent.PromoEvent m_promoEvent;

	public GameObject m_okButton;

	public GameObject m_showMeButton;

	public UILabel m_titleLabel;

	public UILabel m_bodyLabel;

	public MeshFilter m_mesh;

	public MoveToPageProperties m_moveToPageProps;

	public GuiTrigger[] m_pages;

	public GameObject[] m_grids;

	private StoreContent.StoreEntry m_pendingPurchase;

	public static bool Display(PromoSpecialEvent.PromoEvent promoEvent)
	{
		m_promoEvent = promoEvent;
		if ((m_promoEvent.itemId != null || m_promoEvent.mesh != null) && DialogStack.IsInitialized() && PreEnabler.gridsPreloaded && !DialogStack.IsDialogShown)
		{
			DialogStack.ShowDialog("Promo Special Event Dialog");
			return true;
		}
		return false;
	}

	private string Translate(string[] multiLanguageWord)
	{
		string value = Application.systemLanguage.ToString().ToLower();
		foreach (string text in multiLanguageWord)
		{
			string[] array = text.Split('=');
			if (array[0] != null && array[0].ToLower().Equals(value))
			{
				return array[1];
			}
		}
		return "No available";
	}

	private void OnEnable()
	{
		m_titleLabel.text = Translate(m_promoEvent.titles);
		m_bodyLabel.text = Translate(m_promoEvent.bodies);
		m_okButton.SetActive(!m_promoEvent.storeLink);
		m_showMeButton.SetActive(m_promoEvent.storeLink);
		if (m_promoEvent.itemId != null)
		{
			m_pendingPurchase = StoreContent.GetStoreEntryNoAsserts(m_promoEvent.itemId, StoreContent.Identifiers.Name);
			if (m_pendingPurchase == null)
			{
				return;
			}
			m_mesh.mesh = m_pendingPurchase.m_mesh;
			int num = -1;
			switch (m_pendingPurchase.m_group)
			{
			case StoreContent.Group.Currency:
				num = 0;
				break;
			case StoreContent.Group.PowerUps:
				num = 1;
				break;
			case StoreContent.Group.Upgrades:
				num = 2;
				break;
			case StoreContent.Group.Free:
				num = 3;
				break;
			case StoreContent.Group.Characters:
				num = 4;
				break;
			case StoreContent.Group.Wallpaper:
				num = 5;
				break;
			}
			if (num >= 0)
			{
				StoreFront component = m_grids[num].GetComponent<StoreFront>();
				if (component != null)
				{
					component.HighlightEntry(m_pendingPurchase);
				}
				m_moveToPageProps.DestinationPage = m_pages[num];
				if (m_promoEvent.page != null)
				{
					SetDestinationPage(m_promoEvent.page);
				}
			}
		}
		else if (m_promoEvent.mesh != null && m_promoEvent.page != null)
		{
			m_mesh.mesh = m_promoEvent.mesh;
			SetDestinationPage(m_promoEvent.page);
		}
	}

	private void SetDestinationPage(string pageName)
	{
		string text = "Menu Pages/" + pageName;
		GameObject gameObject = GameObject.Find(text);
		if (gameObject != null)
		{
			GuiTrigger component = gameObject.GetComponent<GuiTrigger>();
			if (component != null)
			{
				m_moveToPageProps.DestinationPage = component;
			}
		}
	}

	private void Trigger_ShowPromo()
	{
		DialogStack.HideDialog();
	}
}
