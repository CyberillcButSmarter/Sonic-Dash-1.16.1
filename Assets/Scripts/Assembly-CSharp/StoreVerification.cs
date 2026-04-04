using UnityEngine;

public class StoreVerification : MonoBehaviour
{
	private const string StateRoot = "store";

	private const string ValidatePurchaseProperty = "validateall";

	private const string ValidateSecurelyProperty = "secure";

	private void Start()
	{
		EventDispatch.RegisterInterest("FeatureStateReady", this);
	}

	private void SetRecieptVerificationState(LSON.Property validateProperty, LSON.Property secureProperty)
	{
		bool verify = true;
		bool secureConnection = true;
		if (validateProperty != null)
		{
			bool boolValue = false;
			if (LSONProperties.AsBool(validateProperty, out boolValue))
			{
				verify = boolValue;
			}
		}
		if (secureProperty != null)
		{
			bool boolValue2 = false;
			if (LSONProperties.AsBool(secureProperty, out boolValue2))
			{
				secureConnection = boolValue2;
			}
		}
		SLStorePlugin.SetReceiptVerification(verify, false, secureConnection);
	}

	private void Event_FeatureStateReady()
	{
		LSON.Property stateProperty = FeatureState.GetStateProperty("store", "validateall");
		LSON.Property stateProperty2 = FeatureState.GetStateProperty("store", "secure");
		SetRecieptVerificationState(stateProperty, stateProperty2);
	}
}
