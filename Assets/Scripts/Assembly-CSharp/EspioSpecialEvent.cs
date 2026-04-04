using UnityEngine;

public class EspioSpecialEvent : MonoBehaviour
{
	public enum RewardType
	{
		Espio = 0,
		Rings = 1
	}

	private const int c_TokensRequired = 10;

	public const string EspioTokenSaveVar = "EspioTokenSaveVar";

	public const string EspioEventDialogShownSaveVar = "EspioEventDialogShownSaveVar";

	public const string EspioAwardedSaveVar = "EspioAwarded";

	private static EspioSpecialEvent s_Instance;

	private SpawnPool m_TokenSpawnPool;

	public int EspioTokens { get; private set; }

	public bool EspioEventIntroDialogShown { get; private set; }

	public bool EspioAwarded { get; private set; }

	public static bool IsEventActive()
	{
		return false;
	}

	public static RewardType GetRewardType()
	{
		if (Characters.CharacterUnlocked(Characters.Type.Espio))
		{
			return RewardType.Rings;
		}
		return RewardType.Espio;
	}

	public static int GetTokensNeeded()
	{
		if (!IsEventActive())
		{
			return 0;
		}
		int num = 10 - s_Instance.EspioTokens;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public static int GetTotalTokensNeeded()
	{
		if (!IsEventActive())
		{
			return 0;
		}
		return 10;
	}

	public static void ShowProgressDialog()
	{
		if (!Characters.CharacterUnlocked(Characters.Type.Espio))
		{
			Dialog_EspioSpecialEvent.Display(Dialog_EspioSpecialEvent.DialogMode.Progress);
		}
	}

	public static void CheckForSuccess()
	{
		if (s_Instance.EspioTokens >= 10 && !s_Instance.EspioAwarded)
		{
			s_Instance.Win();
		}
	}

	public static SpawnPool GetTokenSpawnPool()
	{
		return s_Instance.m_TokenSpawnPool;
	}

	public static void AwardEspioToken()
	{
		s_Instance.EspioTokens++;
	}

	private void Win()
	{
		EspioAwarded = true;
		StorePurchases.RequestReward("Character Espio", 1, 18, StorePurchases.ShowDialog.No);
		CharacterManager.Singleton.SetPendingCharacterSelection(Characters.Type.Espio);
		PropertyStore.Save();
		Dialog_EspioSpecialEvent.Display(Dialog_EspioSpecialEvent.DialogMode.Success);
	}

	private void Awake()
	{
		EspioTokens = 0;
		EspioEventIntroDialogShown = false;
		EspioAwarded = false;
		s_Instance = this;
		m_TokenSpawnPool = GetComponent<SpawnPool>();
	}

	private void Event_MainMenuActive()
	{
		CheckForSuccess();
	}

	private void Event_OnCharacterSelection(bool active)
	{
		if (IsEventActive() && !EspioEventIntroDialogShown && CharacterManager.Singleton.GetCurrentCharacterSelection() == Characters.Type.Espio)
		{
			if (!Characters.CharacterUnlocked(Characters.Type.Espio))
			{
				Dialog_EspioSpecialEvent.Display(Dialog_EspioSpecialEvent.DialogMode.Progress);
			}
			EspioEventIntroDialogShown = true;
		}
	}

	private void Event_OnGameDataSaveRequest()
	{
		PropertyStore.Store("EspioTokenSaveVar", EspioTokens);
		PropertyStore.Store("EspioEventDialogShownSaveVar", EspioEventIntroDialogShown);
		PropertyStore.Store("EspioAwarded", EspioAwarded);
	}

	private void Event_OnGameDataLoaded(ActiveProperties activeProperties)
	{
		EspioTokens = Mathf.Max(0, activeProperties.GetInt("EspioTokenSaveVar"));
		EspioEventIntroDialogShown = activeProperties.GetBool("EspioEventDialogShownSaveVar");
		EspioAwarded = activeProperties.GetBool("EspioAwarded");
	}
}
