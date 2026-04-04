using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

[AddComponentMenu("Path-o-logical/PoolManager/SpawnPoolDynamic")]
public class SpawnPoolDynamic : SpawnPool
{
	[Serializable]
	public class PrefabDesc
	{
		public string Name = string.Empty;

		public int PreloadAmount = 1;

		public bool LimitInstances;

		public int LimitAmount = 100;

		public bool LimitFIFO;

		public bool CullDespawned;

		public bool LogMessages;
	}

	[SerializeField]
	public string Path = string.Empty;

	[SerializeField]
	private List<PrefabDesc> PrefabDescriptions = new List<PrefabDesc>();

	private Stopwatch overallStopwatch;

	public List<PrefabDesc> Descs
	{
		get
		{
			return PrefabDescriptions;
		}
	}

	public SpawnPool Pool
	{
		get
		{
			return this;
		}
	}

	public IEnumerator Load()
	{
		if (Pool.Count > 0)
		{
			Debug.LogError("Spawn pool dynamic: " + poolName + " is already populated");
		}
		overallStopwatch = new Stopwatch();
		overallStopwatch.Start();
		for (int i = 0; i < PrefabDescriptions.Count; i++)
		{
			PrefabDesc desc = PrefabDescriptions[i];
			GameObject Object = Resources.Load(Path + desc.Name, typeof(GameObject)) as GameObject;
			if (PoolFrameTimeSentinal.IsFramerateImportant)
			{
				yield return null;
			}
			if (Object == null)
			{
				Debug.LogError("Spawn pool dynamic: " + poolName + " failed to load game object: " + desc.Name);
				continue;
			}
			PrefabPool prefabPool = new PrefabPool(Object.transform);
			prefabPool.inspectorInstanceConstructor();
			prefabPool.preloadAmount = desc.PreloadAmount;
			prefabPool.limitInstances = desc.LimitInstances;
			prefabPool.limitAmount = desc.LimitAmount;
			prefabPool.limitFIFO = desc.LimitFIFO;
			prefabPool.cullDespawned = desc.CullDespawned;
			prefabPool._logMessages = desc.LogMessages;
			CreatePrefabPool(prefabPool);
			if (PoolFrameTimeSentinal.IsFramerateImportant)
			{
				yield return null;
			}
		}
		overallStopwatch.Stop();
		Debug.Log(string.Format(arg1: 0.001f * (float)overallStopwatch.ElapsedMilliseconds, format: "SPAWN POOL DYANMIC: {0} took {1} seconds to load", arg0: poolName));
		overallStopwatch = null;
	}

	public IEnumerator Unload()
	{
		StopAllCoroutines();
		_spawned.Clear();
		foreach (PrefabPool pool in _prefabPools)
		{
			pool.SelfDestruct();
			if (PoolFrameTimeSentinal.IsFramerateImportant)
			{
				yield return null;
			}
		}
		_prefabPools.Clear();
		prefabs._Clear();
	}
}
