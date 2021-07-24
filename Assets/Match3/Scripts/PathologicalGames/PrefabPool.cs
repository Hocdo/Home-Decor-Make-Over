using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	[Serializable]
	public class PrefabPool
	{
		public string key;

		public Transform prefab;

		internal GameObject prefabGO;

		public int preloadAmount = 1;

		public bool preloadTime;

		public int preloadFrames = 2;

		public float preloadDelay;

		public bool limitInstances;

		public int limitAmount = 100;

		public bool limitFIFO;

		public bool cullDespawned;

		public int cullAbove = 50;

		public int cullDelay = 60;

		public int cullMaxPerPass = 5;

		public bool _logMessages;

		private bool forceLoggingSilent;

		public SpawnPool spawnPool;

		private bool cullingActive;

		internal List<Transform> _spawned = new List<Transform>();

		internal List<Transform> _despawned = new List<Transform>();

		private bool _preloaded;

		public bool logMessages
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

		public List<Transform> spawned => new List<Transform>(_spawned);

		public List<Transform> despawned => new List<Transform>(_despawned);

		public int totalCount
		{
			get
			{
				int num = 0;
				num += _spawned.Count;
				return num + _despawned.Count;
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
			_spawned = new List<Transform>();
			_despawned = new List<Transform>();
		}

		internal void SelfDestruct()
		{
			prefab = null;
			prefabGO = null;
			spawnPool = null;
			foreach (Transform item in _despawned)
			{
				if (item != null && spawnPool != null)
				{
					spawnPool.DestroyInstance(item.gameObject);
				}
			}
			foreach (Transform item2 in _spawned)
			{
				if (item2 != null && spawnPool != null)
				{
					spawnPool.DestroyInstance(item2.gameObject);
				}
			}
			_spawned.Clear();
			_despawned.Clear();
		}

		internal bool DespawnInstance(Transform xform)
		{
			return DespawnInstance(xform, sendEventMessage: true);
		}

		internal bool DespawnInstance(Transform xform, bool sendEventMessage)
		{
			if (logMessages)
			{
			}
			_spawned.Remove(xform);
			_despawned.Add(xform);
			if ((bool)xform)
			{
				if (sendEventMessage)
				{
					xform.gameObject.BroadcastMessage("OnDespawned", spawnPool, SendMessageOptions.DontRequireReceiver);
				}
				xform.SetParent(spawnPool.group);
				xform.gameObject.SetActive(value: false);
			}
			if (!cullingActive && cullDespawned && totalCount > cullAbove)
			{
				cullingActive = true;
				spawnPool.StartCoroutine(CullDespawned());
			}
			return true;
		}

		internal IEnumerator CullDespawned()
		{
			if (logMessages)
			{
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
					if (_despawned.Count > 0)
					{
						Transform transform = _despawned[0];
						_despawned.RemoveAt(0);
						spawnPool.DestroyInstance(transform.gameObject);
						if (!logMessages)
						{
						}
					}
					else if (logMessages)
					{
						break;
					}
				}
				yield return new WaitForSeconds(cullDelay);
			}
			if (logMessages)
			{
			}
			cullingActive = false;
			yield return null;
		}

		internal Transform SpawnInstance(Vector3 pos, Quaternion rot)
		{
			if (limitInstances && limitFIFO && _spawned.Count >= limitAmount)
			{
				Transform transform = _spawned[0];
				if (logMessages)
				{
				}
				DespawnInstance(transform);
				spawnPool._spawned.Remove(transform);
			}
			Transform transform2;
			if (_despawned.Count == 0)
			{
				transform2 = SpawnNew(pos, rot);
			}
			else
			{
				transform2 = _despawned[0];
				if (transform2 == null)
				{
					if (_despawned != null)
					{
						_despawned.RemoveAll((Transform x) => x == null);
					}
					if (_spawned != null)
					{
						_spawned.RemoveAll((Transform x) => x == null);
					}
					transform2 = SpawnNew(pos, rot);
				}
				else
				{
					_despawned.RemoveAt(0);
				}
				_spawned.Add(transform2);
				if (logMessages)
				{
				}
				transform2.position = pos;
				transform2.rotation = rot;
				transform2.gameObject.SetActive(value: true);
			}
			return transform2;
		}

		public Transform SpawnNew()
		{
			return SpawnNew(Vector3.zero, Quaternion.identity);
		}

		public Transform SpawnNew(Vector3 pos, Quaternion rot)
		{
			if (limitInstances && totalCount >= limitAmount)
			{
				if (logMessages)
				{
				}
				return null;
			}
			if (pos == Vector3.zero)
			{
				pos = spawnPool.group.position;
			}
			if (rot == Quaternion.identity)
			{
				rot = spawnPool.group.rotation;
			}
			GameObject gameObject = spawnPool.InstantiatePrefab(prefabGO, pos, rot);
			Transform transform = gameObject.transform;
			nameInstance(transform);
			if (!spawnPool.dontReparent)
			{
				bool worldPositionStays = !(transform is RectTransform);
				transform.SetParent(spawnPool.group, worldPositionStays);
			}
			if (spawnPool.matchPoolScale)
			{
				transform.localScale = Vector3.one;
			}
			if (spawnPool.matchPoolLayer)
			{
				SetRecursively(transform, spawnPool.gameObject.layer);
			}
			_spawned.Add(transform);
			if (logMessages)
			{
			}
			return transform;
		}

		private void SetRecursively(Transform xform, int layer)
		{
			xform.gameObject.layer = layer;
			IEnumerator enumerator = xform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform xform2 = (Transform)enumerator.Current;
					SetRecursively(xform2, layer);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		internal void AddUnpooled(Transform inst, bool despawn)
		{
			nameInstance(inst);
			if (despawn)
			{
				inst.gameObject.SetActive(value: false);
				_despawned.Add(inst);
			}
			else
			{
				_spawned.Add(inst);
			}
		}

		internal void PreloadInstances()
		{
			if (preloaded)
			{
				return;
			}
			preloaded = true;
			if (prefab == null)
			{
				return;
			}
			if (limitInstances && preloadAmount > limitAmount)
			{
				preloadAmount = limitAmount;
			}
			if (!cullDespawned || preloadAmount > cullAbove)
			{
			}
			if (preloadTime)
			{
				if (preloadFrames > preloadAmount)
				{
					preloadFrames = preloadAmount;
				}
				spawnPool.StartCoroutine(PreloadOverTime());
				return;
			}
			forceLoggingSilent = true;
			while (totalCount < preloadAmount)
			{
				Transform xform = SpawnNew();
				DespawnInstance(xform, sendEventMessage: false);
			}
			forceLoggingSilent = false;
		}

		private IEnumerator PreloadOverTime()
		{
			yield return new WaitForSeconds(preloadDelay);
			int amount = preloadAmount - totalCount;
			if (amount <= 0)
			{
				yield break;
			}
			int remainder = amount % preloadFrames;
			int numPerFrame = amount / preloadFrames;
			forceLoggingSilent = true;
			for (int j = 0; j < preloadFrames; j++)
			{
				int numThisFrame = numPerFrame;
				if (j == preloadFrames - 1)
				{
					numThisFrame += remainder;
				}
				for (int i = 0; i < numThisFrame; i++)
				{
					Transform inst = SpawnNew();
					if (inst != null)
					{
						DespawnInstance(inst, sendEventMessage: false);
					}
					yield return null;
				}
				if (totalCount > preloadAmount)
				{
					break;
				}
			}
			forceLoggingSilent = false;
		}

		public bool Contains(Transform transform)
		{
			if (prefabGO == null)
			{
			}
			if (spawned.Contains(transform))
			{
				return true;
			}
			if (despawned.Contains(transform))
			{
				return true;
			}
			return false;
		}

		private void nameInstance(Transform instance)
		{
			instance.name += (totalCount + 1).ToString("#000");
		}
	}
}
