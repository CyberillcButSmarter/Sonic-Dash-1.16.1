using UnityEngine;

public class SLStorePlugin
{
	private static AndroidJavaClass m_SLStoreInterfaceClass = new AndroidJavaClass("com.hardlightstudio.dev.sonicdash.plugin.billing.SLStoreInterface");

	private static string m_lastProductID;

	private static bool m_inventoryQueryRequired = false;

	public static void Start()
	{
	}

	public static void SetReceiptVerification(bool verify, bool metrics, bool secureConnection)
	{
		m_SLStoreInterfaceClass.CallStatic("SetReceiptVerification", verify, metrics, secureConnection);
	}

	public static void Update()
	{
		int num = m_SLStoreInterfaceClass.CallStatic<int>("StoreUpdate", new object[0]);
		for (int i = 0; i < num; i++)
		{
			int num2 = m_SLStoreInterfaceClass.CallStatic<int>("StoreTransactionPop", new object[0]);
			string text = m_SLStoreInterfaceClass.CallStatic<string>("StoreTransactionProductID", new object[0]);
			string text2 = m_SLStoreInterfaceClass.CallStatic<string>("StoreTransactionID", new object[0]);
			if (num2 != 0)
			{
				int num3 = m_SLStoreInterfaceClass.CallStatic<int>("StoreTransactionType", new object[0]);
				int num4 = m_SLStoreInterfaceClass.CallStatic<int>("StoreTransactionQuantity", new object[0]);
				object[] parameters = new object[6]
				{
					text,
					text2,
					num4,
					(ProvideContentSource)num3,
					0,
					StorePurchases.ShowDialog.Yes
				};
				EventDispatch.GenerateEvent("ProvideContent", parameters);
				if (num3 == 0)
				{
					SLAnalytics.LogTrackingEvent("IAP", text);
					SLAnalytics.LogPushEventWithParameters("Purchase", text);
					SLAnalytics.LogPushTagWithParameters("Purchase Bundle", text, "string");
					SLAnalytics.LogPushTagWithTimestamp("iap_purchases_ldate");
				}
			}
			else
			{
				int num5 = m_SLStoreInterfaceClass.CallStatic<int>("StoreTransactionError", new object[0]);
				object[] parameters2 = new object[2]
				{
					text,
					(PaymentErrorCode)num5
				};
				EventDispatch.GenerateEvent("PaymentFailed", parameters2);
			}
		}
		if (m_inventoryQueryRequired)
		{
			m_SLStoreInterfaceClass.CallStatic("StoreRefreshInventory");
			m_inventoryQueryRequired = false;
		}
	}

	public static ProductStateCode GetProductState(string ProductID)
	{
		ProductStateCode productStateCode = (ProductStateCode)m_SLStoreInterfaceClass.CallStatic<int>("StoreGetProductInfo", new object[1] { ProductID });
		if (productStateCode == ProductStateCode.ProductInfoNone)
		{
			m_inventoryQueryRequired = true;
		}
		return productStateCode;
	}

	public static string GetProductCost(string ProductID)
	{
		ProductStateCode productState = GetProductState(ProductID);
		string result = string.Empty;
		if (productState == ProductStateCode.ProductInfoDone)
		{
			result = m_SLStoreInterfaceClass.CallStatic<string>("StoreGetProductInfoPrice", new object[0]);
		}
		return result;
	}

	public static ProductStateCode GetProductInfo(string ProductID, out string Price, out string CurrencyCode, out string NumericalPrice)
	{
		ProductStateCode productState = GetProductState(ProductID);
		bool flag = productState == ProductStateCode.ProductInfoDone || productState == ProductStateCode.ProductInfoDoneRrefershing;
		Price = ((!flag) ? string.Empty : GetProductCost(ProductID));
		CurrencyCode = ((!flag) ? string.Empty : GetProductCurrencyCode(ProductID));
		NumericalPrice = ((!flag) ? string.Empty : GetProductNumericalPrice(ProductID));
		return productState;
	}

	public static string GetProductName(string ProductID)
	{
		return m_SLStoreInterfaceClass.CallStatic<string>("StoreGetProductInfoName", new object[0]);
	}

	public static string GetProductDesc(string ProductID)
	{
		return m_SLStoreInterfaceClass.CallStatic<string>("StoreGetProductInfoDesc", new object[0]);
	}

	public static string GetProductCurrencyCode(string ProductID)
	{
		return m_SLStoreInterfaceClass.CallStatic<string>("StoreGetProductInfoCurrencyCode", new object[0]);
	}

	public static string GetProductNumericalPrice(string ProductID)
	{
		return m_SLStoreInterfaceClass.CallStatic<string>("StoreGetProductInfoNumericalPrice", new object[0]);
	}

	public static bool RequestPayment(string ProductID, int quantity)
	{
		m_lastProductID = ProductID;
		return m_SLStoreInterfaceClass.CallStatic<bool>("StoreRequestPayment", new object[2] { m_lastProductID, quantity });
	}

	public static void RequestPaymentDirect(string ProductID, int quantity)
	{
		Debug.LogError("Direct payment requested, currently not supported!");
	}

	public static void DirectPurchaseRequestedResult(string productID, bool Approved)
	{
		if (Approved)
		{
			object[] parameters = new object[1] { productID };
			EventDispatch.GenerateEvent("PurchaseStarted", parameters);
			object[] parameters2 = new object[6]
			{
				productID,
				string.Empty,
				1,
				ProvideContentSource.ContentPurchase,
				0,
				StorePurchases.ShowDialog.Yes
			};
			EventDispatch.GenerateEvent("ProvideContent", parameters2);
		}
	}

	public static void ResetProductInfo()
	{
		m_SLStoreInterfaceClass.CallStatic("StoreReset");
	}

	public static void RestorePurchases()
	{
		m_SLStoreInterfaceClass.CallStatic("StoreRestorePurchases");
	}
}
