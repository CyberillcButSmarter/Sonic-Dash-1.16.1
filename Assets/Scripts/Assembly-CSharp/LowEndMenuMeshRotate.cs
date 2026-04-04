using UnityEngine;

public class LowEndMenuMeshRotate : MenuMeshRotate
{
	public override void Start()
	{
		base.Start();
		if (FeatureSupport.IsLowEndDevice())
		{
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		Spin();
	}
}
