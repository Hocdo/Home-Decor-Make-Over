using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	public class PrefabsDict : IDictionary<string, Transform>, IEnumerable, ICollection<KeyValuePair<string, Transform>>, IEnumerable<KeyValuePair<string, Transform>>
	{
		private Dictionary<string, Transform> _prefabs = new Dictionary<string, Transform>();

		bool ICollection<KeyValuePair<string, Transform>>.IsReadOnly => true;

		public int Count => _prefabs.Count;

		public Transform this[string key]
		{
			get
			{
				try
				{
					return _prefabs[key];
				}
				catch (KeyNotFoundException)
				{
					string message = $"A Prefab with the name '{key}' not found. \nPrefabs={ToString()}";
					throw new KeyNotFoundException(message);
				}
			}
			set
			{
				throw new NotImplementedException("Read-only.");
			}
		}

		public ICollection<string> Keys => _prefabs.Keys;

		public ICollection<Transform> Values => _prefabs.Values;

		private bool IsReadOnly => true;

		public override string ToString()
		{
			string[] array = new string[_prefabs.Count];
			_prefabs.Keys.CopyTo(array, 0);
			return string.Format("[{0}]", string.Join(", ", array));
		}

		internal void _Add(string prefabName, Transform prefab)
		{
			_prefabs.Add(prefabName, prefab);
		}

		internal void _Update(string prefabName, Transform prefab)
		{
			_prefabs[prefabName] = prefab;
		}

		internal bool _Remove(string prefabName)
		{
			return _prefabs.Remove(prefabName);
		}

		internal void _Clear()
		{
			_prefabs.Clear();
		}

		public bool ContainsKey(string prefabName)
		{
			return _prefabs.ContainsKey(prefabName);
		}

		public bool TryGetValue(string prefabName, out Transform prefab)
		{
			return _prefabs.TryGetValue(prefabName, out prefab);
		}

		public void Add(string key, Transform value)
		{
			throw new NotImplementedException("Read-Only");
		}

		public bool Remove(string prefabName)
		{
			throw new NotImplementedException("Read-Only");
		}

		public bool Contains(KeyValuePair<string, Transform> item)
		{
			string message = "Use Contains(string prefabName) instead.";
			throw new NotImplementedException(message);
		}

		public void Add(KeyValuePair<string, Transform> item)
		{
			throw new NotImplementedException("Read-only");
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		private void CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
		{
			string message = "Cannot be copied";
			throw new NotImplementedException(message);
		}

		void ICollection<KeyValuePair<string, Transform>>.CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
		{
			string message = "Cannot be copied";
			throw new NotImplementedException(message);
		}

		public bool Remove(KeyValuePair<string, Transform> item)
		{
			throw new NotImplementedException("Read-only");
		}

		public IEnumerator<KeyValuePair<string, Transform>> GetEnumerator()
		{
			return _prefabs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _prefabs.GetEnumerator();
		}
	}
}
