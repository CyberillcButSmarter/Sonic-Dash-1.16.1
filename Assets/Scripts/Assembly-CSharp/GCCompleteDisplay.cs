using UnityEngine;

public class GCCompleteDisplay : MonoBehaviour
{
	[SerializeField]
	private MeshFilter m_prizeMesh;

	private void OnEnable()
	{
		StoreContent.StoreEntry storeEntry = StoreContent.GetStoreEntry(GC6Progress.TierRewards[GC6Progress.TierRewards.Length - 1], StoreContent.Identifiers.Name);
		m_prizeMesh.mesh = storeEntry.m_mesh;
	}
}
