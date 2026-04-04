using UnityEngine;

public class PromoSpecialEvent : MonoBehaviour
{
	private enum PromoEventID
	{
		NOTLOADED = -1,
		NOTSPECIFIED = 0
	}

	public class PromoEvent
	{
		public string itemId;

		public string[] titles;

		public string[] bodies;

		public bool storeLink;

		public Mesh mesh;

		public string page;

		public bool show;

		public bool shownOnce;

		public PromoEvent(string itemId, string[] titles, string[] bodies, bool storeLink, string meshName, string pageName)
		{
			this.itemId = itemId;
			this.titles = titles;
			this.bodies = bodies;
			this.storeLink = storeLink;
			mesh = null;
			if (meshName != null)
			{
				Mesh[] array = (Mesh[])Resources.FindObjectsOfTypeAll(typeof(Mesh));
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].name == meshName)
					{
						mesh = array[i];
						break;
					}
				}
			}
			page = pageName;
		}
	}

	private const string EventRoot = "events";

	private const string PromoEventProperty = "promoevent";

	private const string PromoEventIdProperty = "promoeventid";

	private const string PromoEventPlatformsProperty = "promoeventplatforms";

	private const string PromoEventItemIdProperty = "promoeventitemid";

	private const string PromoEventTitlesProperty = "promoeventtitles";

	private const string PromoEventBodiesProperty = "promoeventbodies";

	private const string PromoEventStoreLinkProperty = "promoeventstorelink";

	private const string PromoEventMeshProperty = "promoeventmesh";

	private const string PromoEventPageProperty = "promoeventpage";

	private bool display;

	private int lastEvent = -1;

	private int newEvent = -1;

	private static PromoSpecialEvent s_Instance;

	private PromoEvent m_event;

	private static string LastEventId
	{
		get
		{
			return "PromoLastEventId";
		}
	}

	public static PromoSpecialEvent GetInstance()
	{
		return s_Instance;
	}

	public void Awake()
	{
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
	}

	public void Start()
	{
		s_Instance = this;
		EventDispatch.RegisterInterest("MainMenuActive", this, EventDispatch.Priority.Highest);
		EventDispatch.RegisterInterest("FeatureStateReady", this);
	}

	public void Update()
	{
		if (display && Dialog_PromoSpecialEvent.Display(m_event))
		{
			display = false;
			m_event.show = false;
			m_event.shownOnce = true;
		}
	}

	public bool IsEventActive()
	{
		if (s_Instance != null)
		{
			return s_Instance.m_event != null;
		}
		return false;
	}

	public void Set(string platforms, string itemId, string titles, string bodies, bool storeLink, string meshName, string pageName)
	{
		m_event = new PromoEvent(itemId, titles.Split(';'), bodies.Split(';'), storeLink, meshName, pageName);
		if (PlatformVerify(platforms) && lastEvent != -1 && newEvent != -1 && ((lastEvent == 0 && newEvent == 0 && !m_event.shownOnce) || lastEvent != newEvent))
		{
			m_event.show = true;
			lastEvent = newEvent;
		}
	}

	private bool PlatformVerify(string platformSpecs)
	{
		string[] array = platformSpecs.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string text2 = text.ToLower();
			if (text2.Equals("google"))
			{
				return true;
			}
			if (text2.Equals("amazon") || text2.Equals("ios"))
			{
			}
		}
		return false;
	}

	public void UnSet()
	{
		m_event = null;
	}

	public void ShowIfShould()
	{
		display = IsEventActive() && m_event.show;
	}

	private void Event_MainMenuActive()
	{
		ShowIfShould();
	}

	private void Event_FeatureStateReady()
	{
		if (!FeatureState.Valid)
		{
			return;
		}
		LSON.Property stateProperty = FeatureState.GetStateProperty("events", "promoevent");
		if (stateProperty == null)
		{
			return;
		}
		bool boolValue = false;
		if (!LSONProperties.AsBool(stateProperty, out boolValue))
		{
			return;
		}
		if (boolValue)
		{
			stateProperty = FeatureState.GetStateProperty("events", "promoeventplatforms");
			if (stateProperty == null)
			{
				return;
			}
			string stringValue = null;
			if (!LSONProperties.AsString(stateProperty, out stringValue))
			{
				return;
			}
			string stringValue2 = null;
			stateProperty = FeatureState.GetStateProperty("events", "promoeventitemid");
			if (stateProperty == null || LSONProperties.AsString(stateProperty, out stringValue2))
			{
			}
			stateProperty = FeatureState.GetStateProperty("events", "promoeventtitles");
			if (stateProperty == null)
			{
				return;
			}
			string stringValue3 = null;
			if (!LSONProperties.AsString(stateProperty, out stringValue3))
			{
				return;
			}
			stateProperty = FeatureState.GetStateProperty("events", "promoeventbodies");
			if (stateProperty == null)
			{
				return;
			}
			string stringValue4 = null;
			if (!LSONProperties.AsString(stateProperty, out stringValue4))
			{
				return;
			}
			newEvent = 0;
			stateProperty = FeatureState.GetStateProperty("events", "promoeventid");
			if (stateProperty == null)
			{
				return;
			}
			if (LSONProperties.AsInt(stateProperty, out newEvent))
			{
			}
			bool boolValue2 = false;
			string stringValue5 = null;
			string stringValue6 = null;
			stateProperty = FeatureState.GetStateProperty("events", "promoeventstorelink");
			if (stateProperty == null || LSONProperties.AsBool(stateProperty, out boolValue2))
			{
			}
			stateProperty = FeatureState.GetStateProperty("events", "promoeventmesh");
			if (stateProperty == null || LSONProperties.AsString(stateProperty, out stringValue5))
			{
			}
			stateProperty = FeatureState.GetStateProperty("events", "promoeventpage");
			if (stateProperty == null || LSONProperties.AsString(stateProperty, out stringValue6))
			{
			}
			bool flag = true;
			if (stringValue2 != null)
			{
				StoreContent.StoreEntry storeEntryNoAsserts = StoreContent.GetStoreEntryNoAsserts(stringValue2, StoreContent.Identifiers.Name);
				if (storeEntryNoAsserts != null)
				{
					if ((storeEntryNoAsserts.m_state & StoreContent.StoreEntry.State.Purchased) == StoreContent.StoreEntry.State.Purchased)
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				Set(stringValue, stringValue2, stringValue3, stringValue4, boolValue2, stringValue5, stringValue6);
			}
		}
		else
		{
			UnSet();
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		if (lastEvent != -1)
		{
			PropertyStore.Store(LastEventId, lastEvent);
		}
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		lastEvent = activeProperties.GetInt(LastEventId);
	}
}
