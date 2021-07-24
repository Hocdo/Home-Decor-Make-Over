using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	//[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
	public sealed class SpawnPool : MonoBehaviour, IList<Transform>, IEnumerable, ICollection<Transform>, IEnumerable<Transform>
	{
		[ContextMenu("Generate Keys")]
		public void GenerateKeys()
        {
			for (int i = 0; i < _perPrefabPoolOptions.Count; i++)
            {
				if (_perPrefabPoolOptions[i].prefab != null)
                {
					_perPrefabPoolOptions[i].key = _perPrefabPoolOptions[i].prefab.name;
				}
            }
        }

		public string poolName = string.Empty;

		public bool matchPoolScale;

		public bool matchPoolLayer;

		public bool dontReparent;

		public bool _dontDestroyOnLoad;

		public bool logMessages;

		public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

		public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();

		public PrefabsDict prefabs = new PrefabsDict();

		public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

		private List<PrefabPool> _prefabPools = new List<PrefabPool>();

		internal List<Transform> _spawned = new List<Transform>();

		public bool dontDestroyOnLoad
		{
			get
			{
				return _dontDestroyOnLoad;
			}
			set
			{
				_dontDestroyOnLoad = value;
				if (group != null)
				{
					UnityEngine.Object.DontDestroyOnLoad(group.gameObject);
				}
			}
		}

		public Transform group
		{
			get;
			private set;
		}

		public Dictionary<string, PrefabPool> prefabPools
		{
			get
			{
				Dictionary<string, PrefabPool> dictionary = new Dictionary<string, PrefabPool>();
				for (int i = 0; i < _prefabPools.Count; i++)
				{
					dictionary[_prefabPools[i].prefabGO.name] = _prefabPools[i];
				}
				return dictionary;
			}
		}

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

		public int Count => _spawned.Count;

		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void Initialize()
		{
			if (_dontDestroyOnLoad)
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
			}
			for (int i = 0; i < _perPrefabPoolOptions.Count; i++)
			{
				if (!(_perPrefabPoolOptions[i].prefab == null))
				{
					_perPrefabPoolOptions[i].inspectorInstanceConstructor();
					CreatePrefabPool(_perPrefabPoolOptions[i]);
				}
			}
			PoolManager.Pools.Add(this);
		}

		internal GameObject InstantiatePrefab(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			return InstanceHandler.InstantiatePrefab(prefab, pos, rot);
		}

		internal void DestroyInstance(GameObject instance)
		{
            InstanceHandler.DestroyInstance(instance);
        }

		private void OnDestroy()
		{
			if (logMessages)
			{
			}
			PoolManager.Pools.Remove(this);
			StopAllCoroutines();
			_spawned.Clear();
			foreach (PrefabPool prefabPool in _prefabPools)
			{
				prefabPool.SelfDestruct();
			}
			_prefabPools.Clear();
			prefabs._Clear();
		}

		public void CreatePrefabPool(PrefabPool prefabPool)
		{
			if ((GetPrefabPool(prefabPool.prefab) != null) ? true : false)
			{
				throw new Exception($"Prefab '{prefabPool.prefab}' is already in  SpawnPool '{poolName}'. Prefabs can be in more than 1 SpawnPool but cannot be in the same SpawnPool twice.");
			}

			prefabPool.spawnPool = this;
			_prefabPools.Add(prefabPool);
			prefabs._Add(prefabPool.key, prefabPool.prefab);
			if (!prefabPool.preloaded)
			{
				if (logMessages)
				{
				}
				prefabPool.PreloadInstances();
			}
		}

		public void Add(Transform instance, string prefabName, bool despawn, bool parent)
		{
			int num = 0;
			while (true)
			{
				if (num >= _prefabPools.Count || _prefabPools[num].prefabGO == null)
				{
					return;
				}
				if (_prefabPools[num].prefabGO.name == prefabName)
				{
					break;
				}
				num++;
			}
			_prefabPools[num].AddUnpooled(instance, despawn);
			if (logMessages)
			{
			}
			if (parent)
			{
				bool worldPositionStays = !(instance is RectTransform);
				instance.SetParent(group, worldPositionStays);
			}
			if (!despawn)
			{
				_spawned.Add(instance);
			}
		}

		public void Add(Transform item)
		{
			string message = "Use SpawnPool.Spawn() to properly add items to the pool.";
			throw new NotImplementedException(message);
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform;
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == prefab.gameObject)
				{
					transform = _prefabPools[i].SpawnInstance(pos, rot);
					if (transform == null)
					{
						return null;
					}
					if (parent != null)
					{
						transform.parent = parent;
					}
					else if (!dontReparent && transform.parent != group)
					{
						transform.parent = group;
					}
					_spawned.Add(transform);
					transform.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
					return transform;
				}
			}

			Debug.Log("Missing : " + prefab.name);

			PrefabPool prefabPool = new PrefabPool(prefab);
			CreatePrefabPool(prefabPool);
			transform = prefabPool.SpawnInstance(pos, rot);
			if (parent != null)
			{
				transform.parent = parent;
			}
			else
			{
				transform.parent = group;
			}
			_spawned.Add(transform);
			transform.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
			return transform;
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
		{
			Transform transform = Spawn(prefab, pos, rot, null);
			if (transform == null)
			{
				return null;
			}
			return transform;
		}

		public Transform Spawn(Transform prefab)
		{
			return Spawn(prefab, Vector3.zero, Quaternion.identity);
		}

		public Transform Spawn(string prefabName)
		{
			Transform prefab = prefabs[prefabName];
			return Spawn(prefab);
		}

		public void Despawn(Transform instance)
		{
			bool flag = false;
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i]._spawned.Contains(instance))
				{
					flag = _prefabPools[i].DespawnInstance(instance);
					break;
				}
				if (_prefabPools[i]._despawned.Contains(instance))
				{
					return;
				}
			}
			if (flag && (bool)instance)
			{
				instance.SetParent(group);
				_spawned.Remove(instance);
			}
		}

		public void Despawn(Transform instance, Transform parent)
		{
			instance.parent = parent;
			Despawn(instance);
		}

		public void Despawn(Transform instance, float seconds)
		{
			StartCoroutine(DoDespawnAfterSeconds(instance, seconds, useParent: false, null));
		}

		private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds, bool useParent, Transform parent)
		{
			GameObject go = instance.gameObject;
			while (seconds > 0f)
			{
				yield return null;
				if (go == null || !go.activeInHierarchy)
				{
					yield break;
				}
				seconds -= Time.deltaTime;
			}
			if (useParent)
			{
				Despawn(instance, parent);
			}
			else
			{
				Despawn(instance);
			}
		}

		public void DespawnAll()
		{
			List<Transform> list = new List<Transform>(_spawned);
			for (int i = 0; i < list.Count; i++)
			{
				if ((bool)list[i])
				{
					Despawn(list[i]);
				}
			}
		}

		public bool IsSpawned(Transform instance)
		{
			return _spawned.Contains(instance);
		}

		public PrefabPool GetPrefabPool(Transform prefab)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == null)
				{
				}
				if (_prefabPools[i].prefabGO == prefab.gameObject)
				{
					return _prefabPools[i];
				}
			}
			return null;
		}

		public PrefabPool GetPrefabPool(GameObject prefab)
		{
			for (int i = 0; i < _prefabPools.Count; i++)
			{
				if (_prefabPools[i].prefabGO == null)
				{
				}
				if (_prefabPools[i].prefabGO == prefab)
				{
					return _prefabPools[i];
				}
			}
			return null;
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
			for (int i = 0; i < _spawned.Count; i++)
			{
				yield return _spawned[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < _spawned.Count; i++)
			{
				yield return _spawned[i];
			}
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

		public void Clear()
		{
			throw new NotImplementedException();
		}

		bool ICollection<Transform>.Remove(Transform item)
		{
			throw new NotImplementedException();
		}

		[ContextMenu("Log")]
		public void Log()
        {
			for (int i = 0; i < _perPrefabPoolOptions.Count; i++)
            {
				Debug.Log(i + "-" + _perPrefabPoolOptions[i].prefab.gameObject.name);
            }
        }
	}
}
