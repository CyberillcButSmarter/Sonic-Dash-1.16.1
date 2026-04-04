using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
public class SpawnPool : MonoBehaviour, IList<Transform>, ICollection<Transform>, IEnumerable<Transform>, IEnumerable
{
	private sealed class GetEnumerator_003Ec__Iterator2 : IEnumerator, IDisposable, IEnumerator<object>
	{
		internal List<Transform>.Enumerator _003C_0024s_10_003E__0;

		internal Transform _003Cinstance_003E__1;

		internal int _0024PC;

		internal object _0024current;

		internal SpawnPool _003C_003Ef__this;

		object IEnumerator<object>.Current
		{
			[System.Diagnostics.DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		object IEnumerator.Current
		{
			[System.Diagnostics.DebuggerHidden]
			get
			{
				return _0024current;
			}
		}

		public bool MoveNext()
		{
			uint num = (uint)_0024PC;
			_0024PC = -1;
			bool flag = false;
			switch (num)
			{
			case 0u:
				_003C_0024s_10_003E__0 = _003C_003Ef__this._spawned.GetEnumerator();
				num = 4294967293u;
				goto case 1u;
			case 1u:
				try
				{
					switch (num)
					{
					default:
						if (_003C_0024s_10_003E__0.MoveNext())
						{
							_003Cinstance_003E__1 = _003C_0024s_10_003E__0.Current;
							_0024current = _003Cinstance_003E__1;
							_0024PC = 1;
							flag = true;
							goto IL_00ab;
						}
						break;
					}
				}
				finally
				{
					if (!flag)
					{
						((IDisposable)_003C_0024s_10_003E__0).Dispose();
					}
				}
				_0024PC = -1;
				goto default;
			default:
				{
					return false;
				}
				IL_00ab:
				return true;
			}
		}

		[System.Diagnostics.DebuggerHidden]
		public void Dispose()
		{
			uint num = (uint)_0024PC;
			_0024PC = -1;
			switch (num)
			{
			case 1u:
				try
				{
					break;
				}
				finally
				{
					((IDisposable)_003C_0024s_10_003E__0).Dispose();
				}
			case 0u:
				break;
			}
		}

		[System.Diagnostics.DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}
	}

	public string poolName = string.Empty;

	public bool matchPoolScale;

	public bool matchPoolLayer;

	public bool dontDestroyOnLoad;

	public bool forceDestroyOnDespawn;

	public bool logMessages;

	public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

	public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();

	[HideInInspector]
	public float maxParticleDespawnTime = 60f;

	public PrefabsDict prefabs = new PrefabsDict();

	public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

	protected List<PrefabPool> _prefabPools = new List<PrefabPool>();

	internal List<Transform> _spawned = new List<Transform>();

	public Transform group { get; private set; }

	public Transform this[int index]
	{
		get
		{
			return _spawned[index];
		}
		set
		{
			throw new NotImplementedException("Read-only.");
		}
	}

	public int Count
	{
		get
		{
			return _spawned.Count;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	[System.Diagnostics.DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
	{
		GetEnumerator_003Ec__Iterator2 getEnumerator_003Ec__Iterator = new GetEnumerator_003Ec__Iterator2();
		getEnumerator_003Ec__Iterator._003C_003Ef__this = this;
		return getEnumerator_003Ec__Iterator;
	}

	bool ICollection<Transform>.Remove(Transform item)
	{
		throw new NotImplementedException();
	}

	private void Awake()
	{
		if (dontDestroyOnLoad)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		group = base.transform;
		if (poolName == string.Empty)
		{
			poolName = group.name.Replace("Pool", string.Empty);
			poolName = poolName.Replace("(Clone)", string.Empty);
		}
		if (logMessages)
		{
			Debug.Log(string.Format("SpawnPool {0}: Initializing..", poolName));
		}
		foreach (PrefabPool perPrefabPoolOption in _perPrefabPoolOptions)
		{
			if (!(perPrefabPoolOption.prefab == null))
			{
				perPrefabPoolOption.inspectorInstanceConstructor();
				CreatePrefabPool(perPrefabPoolOption);
			}
		}
		PoolManager.Pools.Add(this);
	}

	private void OnDestroy()
	{
		if (logMessages)
		{
			Debug.Log(string.Format("SpawnPool {0}: Destroying...", poolName));
		}
		PoolManager.Pools.Remove(this);
		Clear();
	}

	public List<Transform> CreatePrefabPool(PrefabPool prefabPool)
	{
		if (GetPrefab(prefabPool.prefab) == null && 0 == 0)
		{
			prefabPool.spawnPool = this;
			_prefabPools.Add(prefabPool);
			prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
		}
		List<Transform> list = new List<Transform>();
		if (!prefabPool.preloaded && !forceDestroyOnDespawn)
		{
			if (logMessages)
			{
				Debug.Log(string.Format("SpawnPool {0}: Preloading {1} {2}", poolName, prefabPool.preloadAmount, prefabPool.prefab.name));
			}
			list.AddRange(prefabPool.PreloadInstances());
		}
		return list;
	}

	public void Add(Transform instance, string prefabName, bool despawn, bool parent)
	{
		foreach (PrefabPool prefabPool in _prefabPools)
		{
			if (prefabPool.prefabGO == null)
			{
				Debug.LogError("Unexpected Error: PrefabPool.prefabGO is null");
				return;
			}
			if (prefabPool.prefabGO.name == prefabName)
			{
				prefabPool.AddUnpooled(instance, despawn);
				if (logMessages)
				{
					Debug.Log(string.Format("SpawnPool {0}: Adding previously unpooled instance {1}", poolName, instance.name));
				}
				if (parent)
				{
					instance.parent = group;
				}
				if (!despawn)
				{
					_spawned.Add(instance);
				}
				return;
			}
		}
		Debug.LogError(string.Format("SpawnPool {0}: PrefabPool {1} not found.", poolName, prefabName));
	}

	public void Add(Transform item)
	{
		string message = "Use SpawnPool.Spawn() to properly add items to the pool.";
		throw new NotImplementedException(message);
	}

	public void Remove(Transform item)
	{
		string message = "Use Despawn() to properly manage items that should remain in the pool but be deactivated.";
		throw new NotImplementedException(message);
	}

	public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
	{
		Transform transform;
		foreach (PrefabPool prefabPool2 in _prefabPools)
		{
			if (prefabPool2.prefabGO == prefab.gameObject)
			{
				transform = prefabPool2.SpawnInstance(pos, rot, forceDestroyOnDespawn);
				if (transform == null)
				{
					return null;
				}
				if (transform.parent != group)
				{
					transform.parent = group;
				}
				_spawned.Add(transform);
				return transform;
			}
		}
		PrefabPool prefabPool = new PrefabPool(prefab);
		CreatePrefabPool(prefabPool);
		transform = prefabPool.SpawnInstance(pos, rot, forceDestroyOnDespawn);
		transform.parent = group;
		_spawned.Add(transform);
		return transform;
	}

	public Transform Spawn(Transform prefab)
	{
		return Spawn(prefab, Vector3.zero, Quaternion.identity);
	}

	public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion quat)
	{
		Transform transform = Spawn(prefab.transform, pos, quat);
		if (transform == null)
		{
			return null;
		}
		ParticleSystem component = transform.GetComponent<ParticleSystem>();
		StartCoroutine(ListenForEmitDespawn(component));
		return component;
	}

	public void CullAll()
	{
		for (int i = 0; i < _prefabPools.Count; i++)
		{
			_prefabPools[i].CullAll();
		}
	}

	public void Despawn(Transform xform)
	{
		bool flag = false;
		foreach (PrefabPool prefabPool in _prefabPools)
		{
			if (prefabPool.spawned.Contains(xform))
			{
				flag = prefabPool.DespawnInstance(xform, forceDestroyOnDespawn);
				break;
			}
			if (prefabPool.despawned.Contains(xform))
			{
				return;
			}
		}
		if (!flag)
		{
			Debug.LogError(string.Format("SpawnPool {0}: {1} not found in SpawnPool", poolName, xform.name));
		}
		else
		{
			_spawned.Remove(xform);
		}
	}

	public void Despawn(Transform instance, float seconds)
	{
		StartCoroutine(DoDespawnAfterSeconds(instance, seconds));
	}

	private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Despawn(instance);
	}

	public void DespawnAll()
	{
		List<Transform> list = new List<Transform>(_spawned);
		foreach (Transform item in list)
		{
			Despawn(item);
		}
	}

	public bool IsSpawned(Transform instance)
	{
		return _spawned.Contains(instance);
	}

	public GameObject GetPrefab(string name)
	{
		foreach (PrefabPool prefabPool in _prefabPools)
		{
			if (prefabPool.prefabGO == null)
			{
				Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", poolName));
			}
			if (prefabPool.prefabGO.name == name)
			{
				return prefabPool.prefabGO;
			}
		}
		return null;
	}

	public Transform GetPrefab(Transform prefab)
	{
		foreach (PrefabPool prefabPool in _prefabPools)
		{
			if (prefabPool.prefabGO == null)
			{
				Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", poolName));
			}
			else if (prefabPool.prefabGO == prefab.gameObject)
			{
				return prefabPool.prefab;
			}
		}
		return null;
	}

	public GameObject GetPrefab(GameObject prefab)
	{
		foreach (PrefabPool prefabPool in _prefabPools)
		{
			if (prefabPool.prefabGO == null)
			{
				Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", poolName));
			}
			if (prefabPool.prefabGO == prefab)
			{
				return prefabPool.prefabGO;
			}
		}
		return null;
	}

	private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
	{
		yield return new WaitForSeconds(emitter.startDelay + 0.25f);
		float safetimer = 0f;
		while (emitter.IsAlive(true))
		{
			if (!emitter.gameObject.activeInHierarchy)
			{
				emitter.Clear(true);
				yield break;
			}
			safetimer += Time.deltaTime;
			if (safetimer > maxParticleDespawnTime)
			{
				Debug.LogWarning(string.Format("SpawnPool {0}: Timed out while listening for all particles to die. Waited for {1}sec.", poolName, maxParticleDespawnTime));
			}
			yield return null;
		}
		Despawn(emitter.transform);
	}

	public override string ToString()
	{
		List<string> list = new List<string>();
		foreach (Transform item in _spawned)
		{
			list.Add(item.name);
		}
		return string.Join(", ", list.ToArray());
	}

	public bool Contains(Transform item)
	{
		string message = "Use IsSpawned(Transform instance) instead.";
		throw new NotImplementedException(message);
	}

	public void CopyTo(Transform[] array, int arrayIndex)
	{
		_spawned.CopyTo(array, arrayIndex);
	}

	public IEnumerator<Transform> GetEnumerator()
	{
		foreach (Transform item in _spawned)
		{
			yield return item;
		}
	}

	public void Clear()
	{
		StopAllCoroutines();
		_spawned.Clear();
		foreach (PrefabPool prefabPool in _prefabPools)
		{
			prefabPool.SelfDestruct();
		}
		_prefabPools.Clear();
		prefabs._Clear();
	}

	public int IndexOf(Transform item)
	{
		throw new NotImplementedException();
	}

	public void Insert(int index, Transform item)
	{
		throw new NotImplementedException();
	}

	public void RemoveAt(int index)
	{
		throw new NotImplementedException();
	}
}
