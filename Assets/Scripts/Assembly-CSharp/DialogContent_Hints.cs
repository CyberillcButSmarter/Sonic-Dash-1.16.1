using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogContent_Hints : MonoBehaviour
{
	public enum Reason
	{
		General = 0,
		Featured = 1,
		LowRings = 2,
		LowScore = 3,
		Important = 4
	}

	[Serializable]
	public class Hint
	{
		public enum State
		{
			UseStore = 1
		}

		public const int AvailableStoreEntries = 4;

		public Reason m_reason;

		public string m_title = string.Empty;

		public string m_description = string.Empty;

		public Mesh m_mesh;

		public string[] m_storeEntry = new string[4]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};

		public State m_state;
	}

	public const int InvalidStoreIndex = -1;

	private const int m_runsWithoutReuse = 5;

	[SerializeField]
	public int m_showImportantEvery = 2;

	[SerializeField]
	public int m_percentGeneral = 15;

	[SerializeField]
	public int m_percentFeatured = 30;

	[SerializeField]
	public int m_percentSpecific = 55;

	private static DialogContent_Hints s_hintContent = null;

	[SerializeField]
	private List<Hint> m_currentHintList;

	private int[] m_useOfHints;

	private Reason[] m_hintType;

	private int m_hintTypeIndex;

	private System.Random m_rand = new System.Random();

	private static string[] s_hintsWithoutRealMoneyStoreEntries = new string[36]
	{
		"HINT_3", "HINT_4", "HINT_5", "HINT_10", "HINT_11", "HINT_12", "HINT_13", "HINT_14", "HINT_15", "HINT_16",
		"HINT_18", "HINT_20", "HINT_22", "HINT_23", "HINT_24", "HINT_25", "HINT_29", "HINT_30", "HINT_36", "HINT_37",
		"HINT_38", "HINT_39", "HINT_41", "HINT_42", "HINT_43", "HINT_44", "HINT_45", "HINT_46", "HINT_47", "HINT_48",
		"HINT_49", "HINT_50", "HINT_51", "HINT_52", "HINT_53", "HINT_54"
	};

	private static string[] s_hintsWithoutStoreEntries = new string[21]
	{
		"HINT_5", "HINT_13", "HINT_14", "HINT_15", "HINT_16", "HINT_18", "HINT_20", "HINT_25", "HINT_36", "HINT_42",
		"HINT_43", "HINT_45", "HINT_46", "HINT_47", "HINT_48", "HINT_49", "HINT_50", "HINT_51", "HINT_52", "HINT_53",
		"HINT_54"
	};

	private static string[] s_hintsNoTopRevenue = new string[31]
	{
		"HINT_3", "HINT_5", "HINT_10", "HINT_11", "HINT_12", "HINT_13", "HINT_14", "HINT_15", "HINT_16", "HINT_18",
		"HINT_20", "HINT_22", "HINT_23", "HINT_24", "HINT_25", "HINT_29", "HINT_30", "HINT_36", "HINT_41", "HINT_42",
		"HINT_43", "HINT_45", "HINT_46", "HINT_47", "HINT_48", "HINT_49", "HINT_50", "HINT_51", "HINT_52", "HINT_53",
		"HINT_54"
	};

	public static List<Hint> HintList
	{
		get
		{
			s_hintContent.InitialiseHintList(ABTesting.Ready);
			return s_hintContent.m_currentHintList;
		}
	}

	public static void Display()
	{
		Hint hint = GetHint();
		int storeEntry = GetStoreEntry(hint);
		Display(hint, storeEntry);
	}

	public static void Display(Hint hintToUse, int storeIndex)
	{
		GuiTrigger guiTrigger = null;
		guiTrigger = ((storeIndex != -1) ? DialogStack.ShowDialog("Hint (With Store)") : DialogStack.ShowDialog("Hint (No Store)"));
		Dialog_Hint componentInChildren = Utils.GetComponentInChildren<Dialog_Hint>(guiTrigger.Trigger.gameObject);
		Dialog_Hint.SetNextContent(hintToUse, storeIndex);
	}

	public static Hint GetHint()
	{
		return s_hintContent.GetSuggestedHint(false);
	}

	public static Hint GetLoadingHint()
	{
		return s_hintContent.GetSuggestedHint(true);
	}

	public static int GetStoreEntry(Hint hintToUse)
	{
		return s_hintContent.GetRandomStoreEntry(hintToUse);
	}

	private void Awake()
	{
		InitialiseHintList(ABTesting.Ready);
		EventDispatch.RegisterInterest("ABTestingReady", this);
	}

	private void Start()
	{
		s_hintContent = this;
	}

	private Hint GetSuggestedHint(bool isForLoading)
	{
		List<Hint> outGeneralHints = new List<Hint>(m_currentHintList);
		List<Hint> list = null;
		for (int num = outGeneralHints.Count - 1; num >= 0; num--)
		{
			if (outGeneralHints[num].m_reason != Reason.Important && m_useOfHints[num] > 0)
			{
				outGeneralHints.RemoveAt(num);
			}
		}
		int testValueAsInt = ABTesting.GetTestValueAsInt(ABTesting.Tests.HINT_OnlyStore);
		string[] array = new string[0];
		if (testValueAsInt != 0)
		{
			switch (testValueAsInt)
			{
			case 1:
				array = s_hintsWithoutStoreEntries;
				break;
			case 2:
				array = s_hintsNoTopRevenue;
				break;
			case 3:
				array = s_hintsWithoutRealMoneyStoreEntries;
				break;
			}
			for (int num2 = outGeneralHints.Count - 1; num2 >= 0; num2--)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (outGeneralHints[num2].m_description == array[i])
					{
						outGeneralHints.RemoveAt(num2);
						break;
					}
				}
			}
		}
		if (m_hintTypeIndex == -1)
		{
			bool flag = m_hintType[m_hintType.Length - 1] == Reason.Important;
			List<Reason> list2 = new List<Reason>(Utils.Shuffle(m_hintType, m_rand));
			m_hintType = list2.ToArray();
			bool flag2 = m_hintType[0] == Reason.Important;
			if (flag && flag2)
			{
				m_hintType[0] = m_hintType[1];
				m_hintType[1] = Reason.Important;
			}
			m_hintTypeIndex = m_hintType.Length - 1;
		}
		List<string> adding;
		List<string> removing;
		GenerateSpecifics(out adding, out removing);
		List<Hint> featuredHints;
		List<Hint> specificHints;
		List<Hint> importantHints;
		BuildHintsLists(outGeneralHints, out outGeneralHints, out featuredHints, out specificHints, out importantHints, adding, removing, isForLoading);
		if (m_hintType[m_hintTypeIndex--] != Reason.Important)
		{
			int num3 = UnityEngine.Random.Range(0, 100);
			bool flag3 = false;
			if (num3 > 100 - m_percentSpecific)
			{
				if (specificHints.Count == 0)
				{
					flag3 = true;
				}
				else
				{
					list = specificHints;
				}
			}
			if ((num3 > m_percentGeneral && num3 <= 100 - m_percentSpecific) || flag3)
			{
				if (featuredHints.Count == 0)
				{
					flag3 = true;
				}
				else
				{
					list = featuredHints;
				}
			}
			if (num3 <= m_percentGeneral || flag3)
			{
				list = outGeneralHints;
			}
			for (int j = 0; j < m_useOfHints.Length; j++)
			{
				if (m_useOfHints[j] > 0)
				{
					m_useOfHints[j]--;
				}
			}
		}
		else
		{
			list = ((importantHints.Count <= 0) ? outGeneralHints : importantHints);
		}
		int count = list.Count;
		int index = UnityEngine.Random.Range(0, count);
		Hint hintToUse = list[index];
		int num4 = m_currentHintList.FindIndex((Hint h) => h.m_description == hintToUse.m_description);
		m_useOfHints[num4] += 5;
		return hintToUse;
	}

	private void GenerateSpecifics(out List<string> adding, out List<string> removing)
	{
		adding = new List<string>();
		removing = new List<string>();
		PlayerStats.Stats currentStats = PlayerStats.GetCurrentStats();
		if ((double)((float)currentStats.m_trackedStats[49] / (float)currentStats.m_trackedStats[1]) < 0.2)
		{
			adding.Add("HINT_4");
		}
		int min;
		List<PowerUps.Type> lowestLevelHintablePowerUps = PowerUpsInventory.GetLowestLevelHintablePowerUps(out min);
		foreach (PowerUps.Type item in lowestLevelHintablePowerUps)
		{
			switch (item)
			{
			case PowerUps.Type.Magnet:
				adding.Add("HINT_11");
				break;
			case PowerUps.Type.HeadStart:
				adding.Add("HINT_12");
				break;
			case PowerUps.Type.DashLength:
				adding.Add("HINT_29");
				break;
			case PowerUps.Type.DashIncrease:
				adding.Add("HINT_30");
				break;
			case PowerUps.Type.Shield:
				adding.Add("HINT_41");
				break;
			}
		}
		if (min == 0)
		{
			adding.Add("HINT_33");
		}
		if (!DCs.AllPiecesCollected())
		{
			adding.Add("HINT_43");
		}
		if (GCState.IsCurrentChallengeActive())
		{
			adding.Add("HINT_45");
			adding.Add("HINT_46");
		}
		if (currentStats.m_trackedStats[63] == 57)
		{
			removing.Add("HINT_3");
			removing.Add("HINT_5");
			removing.Add("HINT_10");
			removing.Add("HINT_27");
			removing.Add("HINT_47");
		}
		if (currentStats.m_trackedStats[50] > 0 || ABTesting.GetTestValueAsInt(ABTesting.Tests.REVIVE_RSRRevive) > 0)
		{
			removing.Add("HINT_4");
			adding.Remove("HINT_4");
		}
		if (PowerUpsInventory.GetPowerUpCount(PowerUps.Type.DoubleRing) > 0)
		{
			removing.Add("HINT_6");
		}
		if (PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.Magnet) == 6)
		{
			removing.Add("HINT_11");
			adding.Remove("HINT_11");
		}
		if (PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.HeadStart) == 6)
		{
			removing.Add("HINT_12");
			adding.Remove("HINT_12");
		}
		if (PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.DashLength) == 6)
		{
			removing.Add("HINT_24");
			removing.Add("HINT_29");
			adding.Remove("HINT_29");
		}
		if (PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.DashIncrease) == 6)
		{
			removing.Add("HINT_22");
			removing.Add("HINT_23");
			removing.Add("HINT_30");
			adding.Remove("HINT_30");
		}
		if (PowerUpsInventory.GetPowerUpLevel(PowerUps.Type.Shield) == 6)
		{
			removing.Add("HINT_41");
			adding.Remove("HINT_41");
		}
		if (min == 6)
		{
			removing.Add("HINT_33");
			adding.Remove("HINT_33");
		}
		if (Characters.CharacterUnlocked(Characters.Type.Tails))
		{
			removing.Add("HINT_37");
		}
		if (Characters.CharacterUnlocked(Characters.Type.Amy))
		{
			removing.Add("HINT_38");
		}
		if (Characters.CharacterUnlocked(Characters.Type.Knuckles))
		{
			removing.Add("HINT_39");
		}
		if (StoreContent.StoreInitialised())
		{
			StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(Characters.StoreEntries[4], StoreContent.Identifiers.Name);
			if (Characters.CharacterUnlocked(Characters.Type.Shadow) || (storeEntry.m_state & StoreContent.StoreEntry.State.Hidden) == StoreContent.StoreEntry.State.Hidden)
			{
				removing.Add("HINT_44");
			}
		}
		else
		{
			removing.Add("HINT_44");
		}
		if (!GCState.IsCurrentChallengeActive())
		{
			removing.Add("HINT_45");
			adding.Remove("HINT_45");
			removing.Add("HINT_46");
			adding.Remove("HINT_46");
		}
		if (!WheelOfFortuneSettings.Instance.HasFreeSpin)
		{
			removing.Add("HINT_48");
		}
		if (Boosters.GetBoostersSelected[0] != -1)
		{
			removing.Add("HINT_49");
			removing.Add("HINT_50");
			removing.Add("HINT_51");
			removing.Add("HINT_52");
			removing.Add("HINT_53");
			removing.Add("HINT_54");
		}
	}

	private void ValidateImportantHints(List<Hint> importantHints)
	{
		for (int num = importantHints.Count - 1; num >= 0; num--)
		{
			Hint hint = importantHints[num];
			if (hint.m_description == "HINT_55")
			{
				SLAds.PrepareVideoAd("INTERSTITIAL_AD");
				if (FeatureSupport.IsLowEndDevice() || !SLAds.IsVideoAvailable() || !SLAds.IsVideoReady("INTERSTITIAL_AD"))
				{
					importantHints.RemoveAt(num);
				}
			}
		}
	}

	private void BuildHintsLists(List<Hint> generalHints, out List<Hint> outGeneralHints, out List<Hint> featuredHints, out List<Hint> specificHints, out List<Hint> importantHints, List<string> adding, List<string> removing, bool isForLoading)
	{
		featuredHints = new List<Hint>();
		specificHints = new List<Hint>();
		importantHints = new List<Hint>();
		string hintDesc;
		foreach (string item in removing)
		{
			hintDesc = item;
			Hint hint = generalHints.Find((Hint h) => h.m_description == hintDesc);
			if (hint != null)
			{
				generalHints.Remove(hint);
			}
		}
		if (!isForLoading)
		{
			if (RingPerMinute.Current < 30f)
			{
				foreach (Hint generalHint in generalHints)
				{
					if (generalHint.m_reason == Reason.LowRings)
					{
						featuredHints.Add(generalHint);
					}
				}
			}
			if ((double)((float)ScoreTracker.CurrentScore / (float)ScoreTracker.HighScore) < 0.75)
			{
				foreach (Hint generalHint2 in generalHints)
				{
					if (generalHint2.m_reason == Reason.LowScore)
					{
						featuredHints.Add(generalHint2);
					}
				}
			}
		}
		foreach (Hint generalHint3 in generalHints)
		{
			if (generalHint3.m_reason == Reason.Featured)
			{
				featuredHints.Add(generalHint3);
			}
		}
		string hintDesc2;
		foreach (string item2 in adding)
		{
			hintDesc2 = item2;
			Hint hint = generalHints.Find((Hint h) => h.m_description == hintDesc2);
			if (hint != null)
			{
				specificHints.Add(hint);
			}
		}
		foreach (Hint generalHint4 in generalHints)
		{
			if (generalHint4.m_reason == Reason.Important)
			{
				importantHints.Add(generalHint4);
			}
		}
		ValidateImportantHints(importantHints);
		outGeneralHints = generalHints;
	}

	private Hint GetRandomHint()
	{
		int count = m_currentHintList.Count;
		int index = UnityEngine.Random.Range(0, count);
		return m_currentHintList[index];
	}

	private int GetRandomStoreEntry(Hint hintToUse)
	{
		int num = -1;
		if ((hintToUse.m_state & Hint.State.UseStore) == Hint.State.UseStore)
		{
			int num2 = 0;
			string[] storeEntry = hintToUse.m_storeEntry;
			foreach (string text in storeEntry)
			{
				if (text != null && text.Length != 0)
				{
					StoreContent.StoreEntry storeEntry2 = StoreContent.GetStoreEntry(text, StoreContent.Identifiers.Name);
					if (storeEntry2 != null)
					{
						num2++;
					}
				}
			}
			if (num2 > 0)
			{
				bool flag = false;
				do
				{
					num = UnityEngine.Random.Range(0, num2);
					string text2 = hintToUse.m_storeEntry[num];
					if (text2 != null && text2.Length > 0)
					{
						StoreContent.StoreEntry storeEntry3 = StoreContent.GetStoreEntry(text2, StoreContent.Identifiers.Name);
						if (storeEntry3 != null)
						{
							flag = true;
						}
					}
				}
				while (!flag);
			}
		}
		return num;
	}

	private void InitialiseHintList(bool abReady)
	{
		int num = ((!abReady) ? (-1) : ABTesting.GetTestValueAsInt(ABTesting.Tests.HINT_ImportantInterval));
		int showImportantEvery = m_showImportantEvery;
		int num2 = ((num > 0) ? num : showImportantEvery);
		if (m_hintType == null)
		{
			m_hintType = new Reason[num2 + 1];
			m_hintType[0] = Reason.Important;
			for (int i = 1; i < m_hintType.Length; i++)
			{
				m_hintType[i] = Reason.General;
			}
			m_hintTypeIndex = -1;
		}
		if (m_currentHintList == null)
		{
			m_currentHintList = new List<Hint>();
		}
		if (m_useOfHints == null)
		{
			m_useOfHints = new int[m_currentHintList.Count];
		}
	}

	private void Event_ABTestingReady()
	{
		if (m_hintType != null)
		{
			m_hintType = null;
		}
		InitialiseHintList(true);
	}
}
