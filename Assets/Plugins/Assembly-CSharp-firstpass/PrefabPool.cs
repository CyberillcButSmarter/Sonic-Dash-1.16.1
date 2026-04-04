using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PrefabPool
{
	public Transform prefab;

	internal GameObject prefabGO;

	public int preloadAmount = 1;

	public bool limitInstances;

	public int limitAmount = 100;

	public bool limitFIFO;

	public bool cullDespawned;

	public int cullAbove = 50;

	public int cullDelay = 60;

	public int cullMaxPerPass = 5;

	public bool _logMessages;

	private bool forceLoggingSilent;

	internal SpawnPool spawnPool;

	private bool cullingActive;

	internal List<Transform> spawned = new List<Transform>();

	internal List<Transform> despawned = new List<Transform>();

	private bool _preloaded;

	private bool logMessages
	{
		get
		{
			if (forceLoggingSilent)
			{
				return false;
			}
			if (spawnPool.logMessages)
			{
				return spawnPool.logMessages;
			}
			return _logMessages;
		}
	}

	internal int totalCount
	{
		get
		{
			int num = 0;
			num += spawned.Count;
			return num + despawned.Count;
		}
	}

	internal bool preloaded
	{
		get
		{
			return _preloaded;
		}
		private set
		{
			_preloaded = value;
		}
	}

	public PrefabPool(Transform prefab)
	{
		this.prefab = prefab;
		prefabGO = prefab.gameObject;
	}

	public PrefabPool()
	{
	}

	internal void inspectorInstanceConstructor()
	{
		prefabGO = prefab.gameObject;
		spawned = new List<Transform>();
		despawned = new List<Transform>();
	}

	public void SelfDestruct()
	{
		prefab = null;
		prefabGO = null;
		spawnPool = null;
		foreach (Transform item in despawned)
		{
			if (item.gameObject != null)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			else
			{
				Debug.LogWarning("A null game object has been encountered");
			}
		}
		foreach (Transform item2 in spawned)
		{
			if (item2.gameObject != null)
			{
				UnityEngine.Object.Destroy(item2.gameObject);
			}
			else
			{
				Debug.LogWarning("A null game object has been encountered");
			}
		}
		spawned.Clear();
		despawned.Clear();
	}

	internal bool DespawnInstance(Transform xform, bool forceDestroyOnDespawn)
	{
		if (logMessages)
		{
			Debug.Log(string.Format("SpawnPool {0} ({1}): Despawning '{2}'", spawnPool.poolName, prefab.name, xform.name));
		}
		spawned.Remove(xform);
		xform.gameObject.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
		xform.gameObject.SetActive(false);
		if (forceDestroyOnDespawn)
		{
			UnityEngine.Object.Destroy(xform.gameObject);
		}
		else
		{
			despawned.Add(xform);
		}
		if (!cullingActive && cullDespawned && totalCount > cullAbove)
		{
			cullingActive = true;
			spawnPool.StartCoroutine(CullDespawned());
		}
		return true;
	}

	internal void CullAll()
	{
		for (int i = 0; i < despawned.Count; i++)
		{
			UnityEngine.Object.Destroy(despawned[i].gameObject);
		}
		despawned.Clear();
	}

	internal IEnumerator CullDespawned()
	{
		if (logMessages)
		{
			Debug.Log(string.Format("SpawnPool {0} ({1}): CULLING TRIGGERED! Waiting {2}sec to begin checking for despawns...", spawnPool.poolName, prefab.name, cullDelay));
		}
		yield return new WaitForSeconds(cullDelay);
		while (totalCount > cullAbove)
		{
			for (int i = 0; i < cullMaxPerPass; i++)
			{
				if (totalCount <= cullAbove)
				{
					break;
				}
				if (despawned.Count > 0)
				{
					Transform inst = despawned[0];
					despawned.RemoveAt(0);
					UnityEngine.Object.Destroy(inst.gameObject);
					if (logMessages)
					{
						Debug.Log(string.Format("SpawnPool {0} ({1}): CULLING to {2} instances. Now at {3}.", spawnPool.poolName, prefab.name, cullAbove, totalCount));
					}
				}
				else if (logMessages)
				{
					Debug.Log(string.Format("SpawnPool {0} ({1}): CULLING waiting for despawn. Checking again in {2}sec", spawnPool.poolName, prefab.name, cullDelay));
					break;
				}
			}
			yield return new WaitForSeconds(cullDelay);
		}
		if (logMessages)
		{
			Debug.Log(string.Format("SpawnPool {0} ({1}): CULLING FINISHED! Stopping", spawnPool.poolName, prefab.name));
		}
		cullingActive = false;
		yield return null;
	}

	internal Transform SpawnInstance(Vector3 pos, Quaternion rot, bool forceDestroyOnDespawn)
	{
		if (limitInstances && limitFIFO && spawned.Count >= limitAmount)
		{
			Transform transform = spawned[0];
			if (logMessages)
			{
				Debug.Log(string.Format("SpawnPool {0} ({1}): LIMIT REACHED! FIFO=True. Calling despawning for {2}...", spawnPool.poolName, prefab.name, transform));
			}
			DespawnInstance(transform, forceDestroyOnDespawn);
			spawnPool._spawned.Remove(transform);
		}
		Transform transform2;
		if (despawned.Count == 0 || forceDestroyOnDespawn)
		{
			transform2 = SpawnNew(pos, rot);
		}
		else
		{
			transform2 = despawned[0];
			despawned.RemoveAt(0);
			spawned.Add(transform2);
			if (transform2 == null)
			{
				string message = "Make sure you didn't delete a despawned instance directly.";
				throw new MissingReferenceException(message);
			}
			if (logMessages)
			{
				Debug.Log(string.Format("SpawnPool {0} ({1}): respawning '{2}'.", spawnPool.poolName, prefab.name, transform2.name));
			}
			transform2.position = pos;
			transform2.rotation = rot;
			transform2.gameObject.SetActive(true);
		}
		if (transform2 != null)
		{
			transform2.gameObject.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
		}
		return transform2;
	}

	internal Transform SpawnNew(Vector3 pos, Quaternion rot)
	{
		if (limitInstances && totalCount >= limitAmount)
		{
			if (logMessages)
			{
				Debug.Log(string.Format("SpawnPool {0} ({1}): LIMIT REACHED! Not creating new instances! (Returning null)", spawnPool.poolName, prefab.name));
			}
			return null;
		}
		if (spawnPool == null || spawnPool.group == null)
		{
			Debug.LogError("no group or pool for obj " + prefabGO.name);
		}
		if (pos == Vector3.zero)
		{
			pos = spawnPool.group.position;
		}
		if (rot == Quaternion.identity)
		{
			rot = spawnPool.group.rotation;
		}
		Transform transform = (Transform)UnityEngine.Object.Instantiate(prefab, pos, rot);
		nameInstance(transform);
		transform.parent = spawnPool.group;
		if (spawnPool.matchPoolScale)
		{
			transform.localScale = Vector3.one;
		}
		if (spawnPool.matchPoolLayer)
		{
			SetRecursively(transform, spawnPool.gameObject.layer);
		}
		spawned.Add(transform);
		if (logMessages)
		{
			Debug.Log(string.Format("SpawnPool {0} ({1}): Spawned new instance '{2}'.", spawnPool.poolName, prefab.name, transform.name));
		}
		return transform;
	}

	private void SetRecursively(Transform xform, int layer)
	{
		xform.gameObject.layer = layer;
		foreach (Transform item in xform)
		{
			SetRecursively(item, layer);
		}
	}

	internal void AddUnpooled(Transform inst, bool despawn)
	{
		nameInstance(inst);
		if (despawn)
		{
			inst.gameObject.SetActive(false);
			despawned.Add(inst);
		}
		else
		{
			spawned.Add(inst);
		}
	}

	internal List<Transform> PreloadInstances()
	{
		List<Transform> list = new List<Transform>();
		if (preloaded)
		{
			Debug.Log(string.Format("SpawnPool {0} ({1}): Already preloaded! You cannot preload twice. If you are running this through code, make sure it isn't also defined in the Inspector.", spawnPool.poolName, prefab.name));
			return list;
		}
		if (prefab == null)
		{
			Debug.LogError(string.Format("SpawnPool {0} ({1}): Prefab cannot be null.", spawnPool.poolName, prefab.name));
			return list;
		}
		forceLoggingSilent = true;
		if (limitInstances && preloadAmount > limitAmount)
		{
			Debug.LogWarning(string.Format("SpawnPool {0} ({1}): You turned ON 'Limit Instances' and entered a 'Limit Amount' greater than the 'Preload Amount'! Setting preload amount to limit amount.", spawnPool.poolName, prefab.name));
			preloadAmount = limitAmount;
		}
		while (totalCount < preloadAmount)
		{
			Transform transform = SpawnNew(Vector3.zero, Quaternion.identity);
			DespawnInstance(transform, false);
			list.Add(transform);
		}
		forceLoggingSilent = false;
		if (cullDespawned && totalCount > cullAbove)
		{
			Debug.LogWarning(string.Format("SpawnPool {0} ({1}): You turned ON Culling and entered a 'Cull Above' threshold greater than the 'Preload Amount'! This will cause the culling feature to trigger immediatly, which is wrong conceptually. Only use culling for extreme situations. See the docs.", spawnPool.poolName, prefab.name));
		}
		return list;
	}

	private void nameInstance(Transform instance)
	{
		instance.name += (totalCount + 1).ToString("#000");
	}
}
