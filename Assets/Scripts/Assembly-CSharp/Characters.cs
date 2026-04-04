using UnityEngine;

public class Characters : MonoBehaviour
{
	public enum Type
	{
		Sonic = 0,
		Tails = 1,
		Knuckles = 2,
		Amy = 3,
		Shadow = 4,
		Blaze = 5,
		Silver = 6,
		Rouge = 7,
		Cream = 8,
		Espio = 9,
		Andronic = 10,
		Android = 11
	}

	public const int NoCharacter = -1;

	private const string CharacterStateSaveProperty = "CharacterState";

	public static string[] StoreEntries = new string[12]
	{
		null, "Character Tails", "Character Knuckles", "Character Amy", "Character Shadow", "Character Blaze", "Character Silver", "Character Rouge", "Character Cream", "Character Espio",
		"Character Andronic", "Character Android"
	};

	public static string[] IDStrings = new string[12]
	{
		"character_sonic", "character_tails", "character_knuckles", "character_amy", "character_shadow", "character_blaze", "character_silver", "character_rouge", "character_cream", "character_espio",
		"character_andronic", "character_android"
	};

	private static bool[] s_unlockState = null;

	public static int WhatCharacterIs(string id)
	{
		for (int i = 0; i < IDStrings.Length; i++)
		{
			if (id == IDStrings[i])
			{
				return i;
			}
		}
		return -1;
	}

	public static void UnlockCharacter(Type character)
	{
		s_unlockState[(int)character] = true;
	}

	public static bool CharacterUnlocked(Type character)
	{
		return s_unlockState[(int)character];
	}

	public static string GetCharacterSaveState()
	{
		string text = string.Empty;
		bool[] array = s_unlockState;
		foreach (bool flag in array)
		{
			text += ((!flag) ? '0' : '1');
		}
		return text;
	}

	private void Start()
	{
		EventDispatch.RegisterInterest("OnGameDataSaveRequest", this);
		EventDispatch.RegisterInterest("OnGameDataLoaded", this);
		EventDispatch.RegisterInterest("MainMenuActive", this, EventDispatch.Priority.High);
		InitialisePurchaseState();
	}

	private static void InitialisePurchaseState()
	{
		int enumCount = Utils.GetEnumCount<Type>();
		if (s_unlockState == null)
		{
			s_unlockState = new bool[enumCount];
		}
		ActiveProperties activeProperties = PropertyStore.ActiveProperties();
		if (!activeProperties.DoesPropertyExist("CharacterState"))
		{
			for (int i = 0; i < enumCount; i++)
			{
				s_unlockState[i] = false;
			}
		}
		else
		{
			string text = activeProperties.GetString("CharacterState");
			char[] array = text.ToCharArray();
			for (int j = 0; j < array.Length; j++)
			{
				s_unlockState[j] = array[j] == '1';
			}
		}
		s_unlockState[0] = true;
	}

	public static void UpdateCharacterVisibility()
	{
		CheckShadowVisibility();
		CheckBlazeVisibility();
		CheckSilverVisibility();
		CheckRougeVisibility();
		CheckCreamVisibility();
		CheckEspioVisibility();
		CheckAndroidVisibility();
		CheckAndronicVisibility();
	}

	private static void CheckShadowVisibility()
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(StoreEntries[4], StoreContent.Identifiers.Name);
		storeEntry.m_state &= ~StoreContent.StoreEntry.State.Hidden;
	}

	private static void CheckBlazeVisibility()
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(StoreEntries[5], StoreContent.Identifiers.Name);
		storeEntry.m_state &= ~StoreContent.StoreEntry.State.Hidden;
	}

	private static void CheckSilverVisibility()
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(StoreEntries[6], StoreContent.Identifiers.Name);
		storeEntry.m_state &= ~StoreContent.StoreEntry.State.Hidden;
	}

	private static void CheckRougeVisibility()
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(StoreEntries[7], StoreContent.Identifiers.Name);
		storeEntry.m_state &= ~StoreContent.StoreEntry.State.Hidden;
	}

	private static void CheckCreamVisibility()
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(StoreEntries[8], StoreContent.Identifiers.Name);
		storeEntry.m_state &= ~StoreContent.StoreEntry.State.Hidden;
	}

	private static void CheckEspioVisibility()
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(StoreEntries[9], StoreContent.Identifiers.Name);
		storeEntry.m_state |= StoreContent.StoreEntry.State.Hidden;
	}

	private static void CheckAndroidVisibility()
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(StoreEntries[11], StoreContent.Identifiers.Name);
		if (GoogleSpecialEvent.IsEventActive() && !CharacterUnlocked(Type.Andronic))
		{
			storeEntry.m_state &= ~StoreContent.StoreEntry.State.Hidden;
			StorePurchases.RequestReward("Character Android", 1, 17, StorePurchases.ShowDialog.No);
		}
		else
		{
			storeEntry.m_state |= StoreContent.StoreEntry.State.Hidden;
		}
	}

	private static void CheckAndronicVisibility()
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(StoreEntries[10], StoreContent.Identifiers.Name);
		if (CharacterUnlocked(Type.Andronic))
		{
			storeEntry.m_state &= ~StoreContent.StoreEntry.State.Hidden;
		}
		else if (!GoogleSpecialEvent.IsEventActive() && GoogleSpecialEvent.HasEverBeenActive())
		{
			storeEntry.m_state &= ~StoreContent.StoreEntry.State.Hidden;
		}
		else
		{
			storeEntry.m_state |= StoreContent.StoreEntry.State.Hidden;
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		string characterSaveState = GetCharacterSaveState();
		PropertyStore.Store("CharacterState", characterSaveState);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		InitialisePurchaseState();
	}

	private void Event_MainMenuActive()
	{
		UpdateCharacterVisibility();
	}
}
