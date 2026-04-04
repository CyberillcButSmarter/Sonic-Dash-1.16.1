using System.Collections;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
	private enum State
	{
		First = 0,
		Loading_First = 1,
		Loading_Next = 2,
		Loading_Previous = 3,
		Waiting = 4
	}

	public class CharacterFiles
	{
		public int sceneId;

		public string spriteName;

		public string characterName;

		public Characters.Type characterType;

		public CharacterFiles(int sceneId, string spriteName, string characterName, Characters.Type characterType)
		{
			this.sceneId = sceneId;
			this.spriteName = spriteName;
			this.characterName = characterName;
			this.characterType = characterType;
		}
	}

	private const float m_fPadlockY = 55f;

	private const float m_fPadlockYWithEventOverlay = 130f;

	public const string PropertyCharacterSelection = "CharacterSelection";

	public GameObject m_nextButtonTrigger;

	public GameObject m_prevButtonTrigger;

	public UISprite m_spriteCharacter;

	public UILabel m_characterNameLabel;

	public UISprite m_spriteLockedCharacter;

	public UISprite m_spritePrevCharacter;

	public UILabel m_prevCharacterNameLabel;

	public UISprite m_spriteLockedPrevCharacter;

	public UISprite m_spriteNextCharacter;

	public UILabel m_nextCharacterNameLabel;

	public UISprite m_spriteLockedNextCharacter;

	public GoogleSpecialEventCharacterOverlay m_googleEventCharacterOverlay;

	public GoogleSpecialEventCharacterOverlay m_googleEventCharacterOverlayPrev;

	public GoogleSpecialEventCharacterOverlay m_googleEventCharacterOverlayNext;

	public EspioSpecialEventCharacterOverlay m_espioEventCharacterOverlay;

	public EspioSpecialEventCharacterOverlay m_espioEventCharacterOverlayPrev;

	public EspioSpecialEventCharacterOverlay m_espioEventCharacterOverlayNext;

	public CharacterFiles[] m_characterFiles = new CharacterFiles[12]
	{
		new CharacterFiles(8, Characters.IDStrings[0], "CHARACTER_SONIC", Characters.Type.Sonic),
		new CharacterFiles(9, Characters.IDStrings[1], "CHARACTER_TAILS", Characters.Type.Tails),
		new CharacterFiles(10, Characters.IDStrings[2], "CHARACTER_KNUCKLES", Characters.Type.Knuckles),
		new CharacterFiles(11, Characters.IDStrings[3], "CHARACTER_AMY", Characters.Type.Amy),
		new CharacterFiles(12, Characters.IDStrings[4], "CHARACTER_SHADOW", Characters.Type.Shadow),
		new CharacterFiles(13, Characters.IDStrings[5], "CHARACTER_BLAZE", Characters.Type.Blaze),
		new CharacterFiles(14, Characters.IDStrings[6], "CHARACTER_SILVER", Characters.Type.Silver),
		new CharacterFiles(15, Characters.IDStrings[7], "CHARACTER_ROUGE", Characters.Type.Rouge),
		new CharacterFiles(16, Characters.IDStrings[8], "CHARACTER_CREAM", Characters.Type.Cream),
		new CharacterFiles(17, Characters.IDStrings[9], "CHARACTER_ESPIO", Characters.Type.Espio),
		new CharacterFiles(18, Characters.IDStrings[10], "CHARACTER_ANDRONIC", Characters.Type.Andronic),
		new CharacterFiles(19, Characters.IDStrings[11], "CHARACTER_ANDROID", Characters.Type.Android)
	};

	private bool m_forceLoad = true;

	private bool m_hasReset;

	private bool m_loading;

	private int m_characterIndex;

	private int m_nextCharacterIndex;

	private static CharacterManager s_singleton;

	private int m_pendingCharacterSelection = -1;

	public static CharacterManager Singleton
	{
		get
		{
			return s_singleton;
		}
	}

	public bool Loading
	{
		get
		{
			return m_loading;
		}
	}

	public Characters.Type GetCurrentCharacter()
	{
		return (Characters.Type)m_characterIndex;
	}

	public Characters.Type GetCurrentCharacterSelection()
	{
		return (Characters.Type)m_nextCharacterIndex;
	}

	public void SetPendingCharacterSelection(Characters.Type character)
	{
		m_pendingCharacterSelection = (int)character;
		if (GameState.GetMode() == GameState.Mode.Menu)
		{
			Event_MainMenuActive();
		}
	}

	public string GetCharacterName(Characters.Type character)
	{
		string characterName = m_characterFiles[(int)character].characterName;
		string stringToCapitalise = LanguageStrings.First.GetString(characterName);
		return LanguageUtils.TitleCaseString(stringToCapitalise);
	}

	private void Awake()
	{
		s_singleton = this;
		m_hasReset = false;
		m_loading = false;
		m_characterIndex = 0;
		m_nextCharacterIndex = m_characterIndex;
		m_forceLoad = true;
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("ResetGameState", this);
		EventDispatch.RegisterInterest("CharacterLoaded", this);
		EventDispatch.RegisterInterest("OnCharacterSelectEnabled", this);
		EventDispatch.RegisterInterest("MainMenuActive", this, EventDispatch.Priority.Low);
		int num = m_characterFiles.Length;
		int enumCount = Utils.GetEnumCount<Characters.Type>();
	}

	public void Update()
	{
		if (!m_hasReset)
		{
		}
	}

	public void Load()
	{
		if ((!m_loading && m_nextCharacterIndex != m_characterIndex) || m_forceLoad)
		{
			m_characterIndex = m_nextCharacterIndex;
			EventDispatch.GenerateEvent("OnCharacterLoad", m_characterFiles[m_characterIndex].sceneId);
			PropertyStore.Save();
			m_loading = true;
			m_forceLoad = false;
		}
	}

	private int ValidateCharacterSelectionIndex(bool increase)
	{
		int num = m_nextCharacterIndex;
		bool flag = true;
		do
		{
			flag = true;
			num = ((!increase) ? (num - 1) : (num + 1));
			if (num < 0)
			{
				num = m_characterFiles.Length - 1;
			}
			if (num >= m_characterFiles.Length)
			{
				num = 0;
			}
			string text = Characters.StoreEntries[(int)m_characterFiles[num].characterType];
			if (text != null)
			{
				StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(text, StoreContent.Identifiers.Name);
				if (storeEntry != null && (storeEntry.m_state & StoreContent.StoreEntry.State.Hidden) == StoreContent.StoreEntry.State.Hidden)
				{
					flag = false;
				}
			}
		}
		while (!flag);
		return num;
	}

	private void UpdateSpriteImageAndName(UISprite sprite, UILabel label, UISprite spriteLocked, GoogleSpecialEventCharacterOverlay googleEventOverlay, EspioSpecialEventCharacterOverlay espioEventOverlay, CharacterFiles characterFiles)
	{
		sprite.spriteName = characterFiles.spriteName;
		sprite.MarkAsChanged();
		LocalisedStringProperties.SetLocalisedString(label.gameObject, characterFiles.characterName);
		spriteLocked.enabled = false;
		bool flag = false;
		if (googleEventOverlay != null)
		{
			bool flag2 = characterFiles.characterType == Characters.Type.Android && GoogleSpecialEvent.IsEventActive();
			NGUITools.SetActive(googleEventOverlay.gameObject, flag2);
			googleEventOverlay.Refresh();
			flag = flag || flag2;
		}
		if (espioEventOverlay != null)
		{
			bool flag3 = characterFiles.characterType == Characters.Type.Espio && EspioSpecialEvent.IsEventActive();
			if (Characters.CharacterUnlocked(Characters.Type.Espio))
			{
				flag3 = false;
			}
			NGUITools.SetActive(espioEventOverlay.gameObject, flag3);
			espioEventOverlay.Refresh();
			flag = flag || flag3;
		}
		if (!StoreContent.StoreInitialised())
		{
			return;
		}
		string text = Characters.StoreEntries[(int)characterFiles.characterType];
		if (text != null)
		{
			StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(text, StoreContent.Identifiers.Name);
			if (storeEntry != null)
			{
				spriteLocked.enabled = (storeEntry.m_state & StoreContent.StoreEntry.State.Purchased) != StoreContent.StoreEntry.State.Purchased;
				Vector3 localPosition = spriteLocked.transform.localPosition;
				localPosition.y = ((!flag) ? 55f : 130f);
				spriteLocked.transform.localPosition = localPosition;
			}
		}
	}

	private void UpdateCharacterUI(int characterIndex)
	{
		UpdateSpriteImageAndName(m_spriteCharacter, m_characterNameLabel, m_spriteLockedCharacter, m_googleEventCharacterOverlay, m_espioEventCharacterOverlay, m_characterFiles[characterIndex]);
		if (StoreContent.StoreInitialised())
		{
			characterIndex = ValidateCharacterSelectionIndex(false);
			UpdateSpriteImageAndName(m_spritePrevCharacter, m_prevCharacterNameLabel, m_spriteLockedPrevCharacter, m_googleEventCharacterOverlayPrev, m_espioEventCharacterOverlayPrev, m_characterFiles[characterIndex]);
			characterIndex = ValidateCharacterSelectionIndex(true);
			UpdateSpriteImageAndName(m_spriteNextCharacter, m_nextCharacterNameLabel, m_spriteLockedNextCharacter, m_googleEventCharacterOverlayNext, m_espioEventCharacterOverlayNext, m_characterFiles[characterIndex]);
		}
	}

	private IEnumerator CoroutineFunction(GameObject trigger)
	{
		yield return null;
		trigger.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
	}

	public void Next()
	{
		m_nextCharacterIndex = ValidateCharacterSelectionIndex(true);
		UpdateCharacterUI(m_nextCharacterIndex);
	}

	public void Previous()
	{
		m_nextCharacterIndex = ValidateCharacterSelectionIndex(false);
		UpdateCharacterUI(m_nextCharacterIndex);
	}

	private void Event_CharacterLoaded()
	{
		m_loading = false;
	}

	private void Event_OnCharacterSelectEnabled()
	{
		UpdateCharacterUI(m_nextCharacterIndex);
	}

	private void Event_ResetGameState(GameState.Mode resetState)
	{
		if (resetState == GameState.Mode.Menu)
		{
			Load();
			UpdateCharacterUI(m_nextCharacterIndex);
			m_hasReset = true;
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		if (m_pendingCharacterSelection != -1)
		{
			PropertyStore.Store("CharacterSelection", m_pendingCharacterSelection);
		}
		else
		{
			PropertyStore.Store("CharacterSelection", m_characterIndex);
		}
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		if (activeProperties.DoesPropertyExist("CharacterSelection"))
		{
			m_nextCharacterIndex = activeProperties.GetInt("CharacterSelection");
			if (m_nextCharacterIndex < 0 || m_nextCharacterIndex > m_characterFiles.Length - 1)
			{
				m_nextCharacterIndex = 0;
			}
		}
		else
		{
			m_nextCharacterIndex = 0;
		}
	}

	private void Event_MainMenuActive()
	{
		if (m_pendingCharacterSelection != -1)
		{
			m_nextCharacterIndex = m_pendingCharacterSelection;
			m_pendingCharacterSelection = -1;
		}
		if (m_nextCharacterIndex == 11 && (StoreContent.GetStoreEntry("Character Android", StoreContent.Identifiers.Name).m_state & StoreContent.StoreEntry.State.Hidden) == StoreContent.StoreEntry.State.Hidden)
		{
			m_nextCharacterIndex = 10;
		}
		if (m_nextCharacterIndex == 10 && (StoreContent.GetStoreEntry("Character Andronic", StoreContent.Identifiers.Name).m_state & StoreContent.StoreEntry.State.Hidden) == StoreContent.StoreEntry.State.Hidden)
		{
			m_nextCharacterIndex = 0;
		}
		if (m_nextCharacterIndex == 9 && (StoreContent.GetStoreEntry("Character Espio", StoreContent.Identifiers.Name).m_state & StoreContent.StoreEntry.State.Hidden) == StoreContent.StoreEntry.State.Hidden)
		{
			m_nextCharacterIndex = 0;
		}
		UpdateCharacterUI(m_nextCharacterIndex);
	}
}
