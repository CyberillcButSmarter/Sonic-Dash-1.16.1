using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoreModifier : MonoBehaviour
{
	// Toggle this to disable all local store tweaks without removing code.
	private const bool EnableLocalStoreTweaks = true;

	// Cost (in red rings / star rings) used when converting money-only items.
	private const int StarRingCost_RingBundles = 50;
	private const int StarRingCost_PremiumPowerUps = 50;

	[Flags]
	private enum State
	{
		Ready = 1,
		Error = 2,
		ReadyToModify = 4,
		ItemsModified = 8
	}

	private enum ModificationType
	{
		Global = 0,
		Specific = 1
	}

	private class Modifier
	{
		[Flags]
		public enum Valid
		{
			Hidden = 2,
			OnSale = 4,
			RingAwardedModifier = 8,
			StarAwardedModifier = 0x10,
			CostModifier = 0x20,
			RingsToAward = 0x40,
			StarsToAward = 0x80,
			AmountToAward = 0x100,
			Cost = 0x200,
			SellText = 0x400,
			iTunesID = 0x800,
			RemoveAds = 0x1000,
			Currency = 0x2000
		}

		public Valid m_validity;

		public uint m_entryNameCRC;

		public bool m_hidden;

		public bool m_onSale;

		public bool m_removeAds;

		public float m_ringAwardedModifier;

		public float m_starAwardedModifier;

		public float m_costModifier;

		public int m_ringsToAward;

		public int m_starsToAward;

		public int m_amountToAdd;

		public int[] m_cost;

		public StoreContent.PaymentMethod m_currency;

		public string m_sellText;

		public string m_iTunesID;
	}

	private const int InvalidModifier = -1;

	private static StoreModifier s_modifier;

	private State m_state;

	// Local fallback timer to force modifications ready if remote config never arrives (prevents shop blocking).
	private float m_forceReadyCountdown = 3f;

	private List<Modifier> m_modifiers = new List<Modifier>();

	private Modifier m_globalModifier;

	public static bool Ready
	{
		get
		{
			return (s_modifier.m_state & State.Ready) == State.Ready || (s_modifier.m_state & State.Error) == State.Error;
		}
	}

	public static bool ModificationsComplete()
	{
		bool flag = (s_modifier.m_state & State.ItemsModified) == State.ItemsModified;
		bool flag2 = (s_modifier.m_state & State.Error) == State.Error;
		return flag || flag2;
	}

	public static void Restart()
	{
		s_modifier.m_modifiers.Clear();
		s_modifier.StartCoroutine(s_modifier.DownloadServerFile());
	}

	private void Start()
	{
		s_modifier = this;
		EventDispatch.RegisterInterest("OnStoreInitialised", this, EventDispatch.Priority.Highest);
	}

	private void Update()
	{
		// If remote modifier download stalls, force ready so the store unblocks.
		if ((m_state & State.ItemsModified) != State.ItemsModified && (m_state & State.Error) != State.Error)
		{
			m_forceReadyCountdown -= Time.deltaTime;
			if (m_forceReadyCountdown <= 0f)
			{
				m_state |= State.Ready;
				ModifyStockList();
			}
		}
	}

	private void ModifyStockList()
	{
		bool flag = false;
		m_state &= ~State.ReadyToModify;
		List<StoreContent.StoreEntry> stockList = StoreContent.StockList;
		foreach (StoreContent.StoreEntry item in stockList)
		{
			flag |= ModifyStoreEntry(item);
		}
		flag |= ApplyLocalStoreTweaks(stockList);
		if (flag)
		{
			EventDispatch.GenerateEvent("RequestStoreRefresh");
		}
		m_state |= State.ItemsModified;
	}

	private bool ModifyStoreEntry(StoreContent.StoreEntry storeEntry)
	{
		bool flag = false;
		if (m_modifiers != null)
		{
			uint entryCRC = CRC32.Generate(storeEntry.m_identifier, CRC32.Case.Lower);
			Modifier modifier = m_modifiers.FirstOrDefault((Modifier entry) => entry.m_entryNameCRC == entryCRC);
			if (modifier != null)
			{
				flag |= UpdateStoreEntry(storeEntry, modifier, ModificationType.Specific);
			}
		}
		if (m_globalModifier != null)
		{
			flag |= UpdateStoreEntry(storeEntry, m_globalModifier, ModificationType.Global);
		}
		return flag;
	}

	private bool UpdateStoreEntry(StoreContent.StoreEntry entry, Modifier modifier, ModificationType modificationType)
	{
		bool result = false;
		if (modificationType == ModificationType.Global)
		{
			if ((modifier.m_validity & Modifier.Valid.RingAwardedModifier) == Modifier.Valid.RingAwardedModifier)
			{
				entry.m_awards.m_playerRings = Mathf.CeilToInt((float)entry.m_awards.m_playerRings * modifier.m_ringAwardedModifier);
			}
			if ((modifier.m_validity & Modifier.Valid.StarAwardedModifier) == Modifier.Valid.StarAwardedModifier)
			{
				entry.m_awards.m_playerStars = Mathf.CeilToInt((float)entry.m_awards.m_playerStars * modifier.m_starAwardedModifier);
			}
			if ((modifier.m_validity & Modifier.Valid.CostModifier) == Modifier.Valid.CostModifier)
			{
				for (int i = 0; i < entry.m_cost.m_playerCost.Length; i++)
				{
					entry.m_cost.m_playerCost[i] = Mathf.CeilToInt((float)entry.m_cost.m_playerCost[i] * modifier.m_costModifier);
				}
			}
		}
		else
		{
			bool flag = false;
			if ((modifier.m_validity & Modifier.Valid.OnSale) == Modifier.Valid.OnSale)
			{
				if (modifier.m_onSale)
				{
					flag = true;
					entry.m_state |= StoreContent.StoreEntry.State.OnSale;
				}
				else
				{
					flag = false;
					entry.m_state &= ~StoreContent.StoreEntry.State.OnSale;
				}
			}
			if ((modifier.m_validity & Modifier.Valid.Hidden) == Modifier.Valid.Hidden)
			{
				if (modifier.m_hidden)
				{
					entry.m_state |= StoreContent.StoreEntry.State.Hidden;
				}
				else
				{
					entry.m_state &= ~StoreContent.StoreEntry.State.Hidden;
				}
			}
			if ((modifier.m_validity & Modifier.Valid.SellText) == Modifier.Valid.SellText)
			{
				entry.m_offerSellText = modifier.m_sellText;
			}
			if ((modifier.m_validity & Modifier.Valid.RemoveAds) == Modifier.Valid.RemoveAds)
			{
				if (modifier.m_removeAds)
				{
					entry.m_state |= StoreContent.StoreEntry.State.RemoveAds;
				}
				else
				{
					entry.m_state &= ~StoreContent.StoreEntry.State.RemoveAds;
				}
			}
			if ((modifier.m_validity & Modifier.Valid.RingsToAward) == Modifier.Valid.RingsToAward)
			{
				entry.m_awards.m_playerRings = modifier.m_ringsToAward;
				if (!flag)
				{
					entry.m_awards.m_baseRings = modifier.m_ringsToAward;
				}
			}
			if ((modifier.m_validity & Modifier.Valid.StarsToAward) == Modifier.Valid.StarsToAward)
			{
				entry.m_awards.m_playerStars = modifier.m_starsToAward;
				if (!flag)
				{
					entry.m_awards.m_baseStars = modifier.m_starsToAward;
				}
			}
			if ((modifier.m_validity & Modifier.Valid.AmountToAward) == Modifier.Valid.AmountToAward)
			{
				entry.m_powerUpCount = modifier.m_amountToAdd;
				if (!flag)
				{
					entry.m_powerUpCount = modifier.m_amountToAdd;
				}
			}
			if ((modifier.m_validity & Modifier.Valid.Cost) == Modifier.Valid.Cost)
			{
				for (int j = 0; j < entry.m_cost.m_playerCost.Length; j++)
				{
					entry.m_cost.m_playerCost[j] = modifier.m_cost[j];
					if (!flag)
					{
						entry.m_cost.m_baseCost[j] = modifier.m_cost[j];
					}
				}
			}
			if ((modifier.m_validity & Modifier.Valid.Currency) == Modifier.Valid.Currency)
			{
				entry.m_payment = modifier.m_currency;
			}
			if ((modifier.m_validity & Modifier.Valid.iTunesID) == Modifier.Valid.iTunesID)
			{
				entry.m_osStore.m_playeriTunesId = modifier.m_iTunesID;
				if (!flag)
				{
					entry.m_osStore.m_baseiTunesId = modifier.m_iTunesID;
				}
				result = true;
			}
		}
		return result;
	}

	/// <summary>
	/// Local, easily reversible tweaks requested by design:
	/// - Allow buying ring bundles with red rings (StarRings) instead of real money.
	/// - Hide red ring (StarRing) products from the storefront.
	/// - Make Double Rings and Free Revive purchasable with 50 red rings (no real money).
	/// Toggle <see cref="EnableLocalStoreTweaks"/> to revert quickly.
	/// </summary>
	private bool ApplyLocalStoreTweaks(List<StoreContent.StoreEntry> stockList)
	{
		if (!EnableLocalStoreTweaks || stockList == null)
		{
			return false;
		}
		bool changed = false;
		foreach (StoreContent.StoreEntry entry in stockList)
		{
			bool localChange = false;
			bool isRingBundle = entry.m_type == StoreContent.EntryType.Currency && entry.m_awards.m_playerRings > 0;
			bool isStarRingProduct = entry.m_type == StoreContent.EntryType.Currency && entry.m_awards.m_playerStars > 0;
			bool isDoubleRings = entry.m_type == StoreContent.EntryType.PowerUp && entry.m_powerUpType == PowerUps.Type.DoubleRing;
			bool isFreeRevive = entry.m_type == StoreContent.EntryType.PowerUp && entry.m_powerUpType == PowerUps.Type.FreeRevive;
			bool isMoneyOnlyVariant = entry.m_payment == StoreContent.PaymentMethod.Money;

			// Hide any offers that sell red rings directly (they conflict with our red-ring-as-currency tweak).
			if (isStarRingProduct)
			{
				entry.m_state |= StoreContent.StoreEntry.State.Hidden;
				localChange = true;
			}

			// Allow buying ring bundles with red rings (StarRings) instead of money.
			if (isRingBundle && isMoneyOnlyVariant)
			{
				entry.m_payment = StoreContent.PaymentMethod.StarRings;
				EnsureCostArray(entry);
				entry.m_cost.m_playerCost[0] = StarRingCost_RingBundles;
				entry.m_cost.m_baseCost[0] = StarRingCost_RingBundles;
				localChange = true;
			}

			// Make Double Rings and Free Revive cost red rings instead of money, priced at 50.
			if ((isDoubleRings || isFreeRevive) && isMoneyOnlyVariant)
			{
				entry.m_payment = StoreContent.PaymentMethod.StarRings;
				EnsureCostArray(entry);
				entry.m_cost.m_playerCost[0] = StarRingCost_PremiumPowerUps;
				entry.m_cost.m_baseCost[0] = StarRingCost_PremiumPowerUps;
				entry.m_state &= ~StoreContent.StoreEntry.State.Hidden; // Make sure they stay visible.
				localChange = true;
			}

			// Remove duplicate money variants of Double Rings so only the red-ring version shows up once.
			if (isDoubleRings && isMoneyOnlyVariant)
			{
				entry.m_state |= StoreContent.StoreEntry.State.Hidden;
				localChange = true;
			}

			if (localChange)
			{
				changed = true;
			}
		}
		return changed;
	}

	private void EnsureCostArray(StoreContent.StoreEntry entry)
	{
		if (entry.m_cost == null)
		{
			entry.m_cost = new StoreContent.StoreEntry.Cost();
		}
		if (entry.m_cost.m_playerCost == null || entry.m_cost.m_playerCost.Length == 0)
		{
			entry.m_cost.m_playerCost = new int[7];
		}
		if (entry.m_cost.m_baseCost == null || entry.m_cost.m_baseCost.Length == 0)
		{
			entry.m_cost.m_baseCost = new int[7];
		}
	}

	private void CacheStoreModifiers(LSON.Root[] lsonRoot)
	{
		if (lsonRoot == null)
		{
			return;
		}
		GetModifierCount(lsonRoot, m_modifiers);
		LSON.Root root = lsonRoot.FirstOrDefault((LSON.Root root2) => root2.m_name == "global settings");
		if (root != null)
		{
			if (m_globalModifier == null)
			{
				m_globalModifier = new Modifier();
			}
			PopulateModifier(root, m_globalModifier);
		}
	}

	private void GetModifierCount(LSON.Root[] lsonRoot, List<Modifier> modifiers)
	{
		foreach (LSON.Root root in lsonRoot)
		{
			if (root.m_name == null || root.m_properties == null || !(root.m_name == "store entry"))
			{
				continue;
			}
			LSON.Property property = LSONProperties.GetProperty(root.m_properties, "name");
			if (property != null)
			{
				string crcSource = StoreContent.FormatIdentifier(property.m_value);
				uint offerNameCRC = CRC32.Generate(crcSource, CRC32.Case.AsIs);
				Modifier modifier = modifiers.FirstOrDefault((Modifier entry) => entry.m_entryNameCRC == offerNameCRC);
				if (modifier == null)
				{
					modifiers.Add(new Modifier());
					modifier = modifiers.Last();
				}
				modifier.m_entryNameCRC = offerNameCRC;
				PopulateModifier(root, modifier);
			}
		}
	}

	private void PopulateModifier(LSON.Root root, Modifier modifier)
	{
		if (root == null || root.m_properties == null)
		{
			return;
		}
		LSON.Property property = LSONProperties.GetProperty(root.m_properties, "hidden");
		if (property != null && LSONProperties.AsBool(property, out modifier.m_hidden))
		{
			modifier.m_validity |= Modifier.Valid.Hidden;
		}
		LSON.Property property2 = LSONProperties.GetProperty(root.m_properties, "on sale");
		if (property2 != null && LSONProperties.AsBool(property2, out modifier.m_onSale))
		{
			modifier.m_validity |= Modifier.Valid.OnSale;
		}
		LSON.Property property3 = LSONProperties.GetProperty(root.m_properties, "remove ads");
		if (property3 != null && LSONProperties.AsBool(property3, out modifier.m_removeAds))
		{
			modifier.m_validity |= Modifier.Valid.RemoveAds;
		}
		LSON.Property property4 = LSONProperties.GetProperty(root.m_properties, "cost modifier");
		if (property4 != null && LSONProperties.AsFloat(property4, out modifier.m_costModifier))
		{
			modifier.m_validity |= Modifier.Valid.CostModifier;
		}
		LSON.Property property5 = LSONProperties.GetProperty(root.m_properties, "rings to award modifier");
		if (property5 != null && LSONProperties.AsFloat(property5, out modifier.m_ringAwardedModifier))
		{
			modifier.m_validity |= Modifier.Valid.RingAwardedModifier;
		}
		LSON.Property property6 = LSONProperties.GetProperty(root.m_properties, "stars to award modifier");
		if (property6 != null && LSONProperties.AsFloat(property6, out modifier.m_starAwardedModifier))
		{
			modifier.m_validity |= Modifier.Valid.StarAwardedModifier;
		}
		LSON.Property property7 = LSONProperties.GetProperty(root.m_properties, "rings to award");
		if (property7 != null && LSONProperties.AsInt(property7, out modifier.m_ringsToAward))
		{
			modifier.m_validity |= Modifier.Valid.RingsToAward;
		}
		LSON.Property property8 = LSONProperties.GetProperty(root.m_properties, "stars to award");
		if (property8 != null && LSONProperties.AsInt(property8, out modifier.m_starsToAward))
		{
			modifier.m_validity |= Modifier.Valid.StarsToAward;
		}
		LSON.Property property9 = LSONProperties.GetProperty(root.m_properties, "amount to add");
		if (property9 != null && LSONProperties.AsInt(property9, out modifier.m_amountToAdd))
		{
			modifier.m_validity |= Modifier.Valid.AmountToAward;
		}
		LSON.Property property10 = LSONProperties.GetProperty(root.m_properties, "cost");
		if (property10 != null)
		{
			ValidateCostArray(modifier);
			if (LSONProperties.AsInt(property10, out modifier.m_cost[0]))
			{
				modifier.m_validity |= Modifier.Valid.Cost;
			}
		}
		for (int i = 1; i < 7; i++)
		{
			string propertyName = string.Format("cost 0{0}", i);
			LSON.Property property11 = LSONProperties.GetProperty(root.m_properties, propertyName);
			if (property11 != null)
			{
				ValidateCostArray(modifier);
				if (LSONProperties.AsInt(property11, out modifier.m_cost[i]))
				{
					modifier.m_validity |= Modifier.Valid.Cost;
				}
			}
		}
		LSON.Property property12 = LSONProperties.GetProperty(root.m_properties, "offer sell text");
		if (property12 != null && LSONProperties.AsString(property12, out modifier.m_sellText))
		{
			modifier.m_validity |= Modifier.Valid.SellText;
		}
		LSON.Property property13 = LSONProperties.GetProperty(root.m_properties, "itunesid");
		if (property13 != null && LSONProperties.AsString(property13, out modifier.m_iTunesID))
		{
			modifier.m_validity |= Modifier.Valid.iTunesID;
		}
		LSON.Property property14 = LSONProperties.GetProperty(root.m_properties, "currency");
		string stringValue;
		if (property14 != null && LSONProperties.AsString(property14, out stringValue))
		{
			modifier.m_validity |= Modifier.Valid.Currency;
			if (stringValue.ToLower() == "rings")
			{
				modifier.m_currency = StoreContent.PaymentMethod.Rings;
			}
			else if (stringValue.ToLower() == "rsr")
			{
				modifier.m_currency = StoreContent.PaymentMethod.StarRings;
			}
			else if (stringValue.ToLower() == "money")
			{
				modifier.m_currency = StoreContent.PaymentMethod.Money;
			}
		}
	}

	private void ValidateCostArray(Modifier modifier)
	{
		if (modifier.m_cost == null)
		{
			modifier.m_cost = new int[7];
		}
	}

	private IEnumerator DownloadServerFile()
	{
		while (!ABTesting.URLReady)
		{
			yield return null;
		}
		FileDownloader abFile = new FileDownloader(FileDownloader.Files.StoreModifierAB, true);
		yield return abFile.Loading;
		FileDownloader defaultFile = new FileDownloader(FileDownloader.Files.StoreModifierDefault, true);
		yield return defaultFile.Loading;
		if (!string.IsNullOrEmpty(abFile.Error) && !string.IsNullOrEmpty(defaultFile.Error))
		{
			m_state |= State.Error;
			yield break;
		}
		if (string.IsNullOrEmpty(defaultFile.Error))
		{
			LSON.Root[] lsonRoot = LSONReader.Parse(defaultFile.Text);
			if (lsonRoot != null)
			{
				CacheStoreModifiers(lsonRoot);
			}
		}
		if (string.IsNullOrEmpty(abFile.Error))
		{
			LSON.Root[] lsonRoot2 = LSONReader.Parse(abFile.Text);
			if (lsonRoot2 != null)
			{
				CacheStoreModifiers(lsonRoot2);
			}
		}
		m_state |= State.Ready;
		if ((m_state & State.ReadyToModify) == State.ReadyToModify || (m_state & State.ItemsModified) == State.ItemsModified)
		{
			ModifyStockList();
		}
	}

	private void Event_OnStoreInitialised()
	{
		if ((m_state & State.Ready) == State.Ready)
		{
			ModifyStockList();
		}
		else
		{
			m_state |= State.ReadyToModify;
		}
	}
}
