using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentAssistant : MonoBehaviour
{
	[Serializable]
	public struct ContentAssistantItem
	{
		public GameObject item;
	}

	public static ContentAssistant main;

	public List<ContentAssistantItem> cItems;

	private readonly Dictionary<string, GameObject> content = new Dictionary<string, GameObject>();

	private GameObject zObj;

	private void Awake()
	{
		main = this;
		content.Clear();
		foreach (ContentAssistantItem cItem in cItems)
		{
			ContentAssistantItem current = cItem;
			content.Add(current.item.name, current.item);
		}
	}

	public T GetItem<T>(string key) where T : Component
	{
		return UnityEngine.Object.Instantiate(content[key]).GetComponent<T>();
	}

	public GameObject GetItem(string key)
	{
		return UnityEngine.Object.Instantiate(content[key]);
	}

	public T GetItem<T>(string key, Vector3 position) where T : Component
	{
		zObj = GetItem(key);
		zObj.transform.position = position;
		return zObj.GetComponent<T>();
	}

	public GameObject GetItem(string key, Vector3 position)
	{
		zObj = GetItem(key);
		zObj.transform.position = position;
		return zObj;
	}

	public GameObject GetItem(string key, Vector3 position, Quaternion rotation)
	{
		zObj = GetItem(key, position);
		zObj.transform.rotation = rotation;
		return zObj;
	}
}
