using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

public class NetworkManager : MonoSingleton<NetworkManager>
{
	public enum URLList
	{
		Dev,
		Release
	}

	[NonSerialized]
	[HideInInspector]
	public string[] strURLLists = new string[2]
	{
		"https://beagles.cookappsgames.com/jewel_blast/",
		"https://beagles.cookappsgames.com/jewel_blast/"
	};

	[NonSerialized]
	public URLList ConnectURL;

	[HideInInspector]
	public string URL;

	[HideInInspector]
	public readonly string MapToolURL = "https://beagles.cookappsgames.com/jewel_blast/";

	private string packetKey = string.Empty;

	public bool IsCustomizeTimeOut;

	public float CustomizeTimeOut = 30f;

	private WebCommandBase lastCmd;

	public bool EncryptOn
	{
		get;
		set;
	}

	public bool IsConnecting
	{
		get;
		set;
	}

	public NetworkManager()
	{
		IsConnecting = false;
		EncryptOn = false;
	}

	public override void Awake()
	{
		base.Awake();
		URL = strURLLists[(int)ConnectURL];
		ServicePointManager.Expect100Continue = false;
	}

	public void StartNetwork(WebCommandBase cmd)
	{
		IsConnecting = true;
		PushWebCommand(cmd);
	}

	public string GetPacketKey()
	{
		return (!EncryptOn) ? string.Empty : packetKey;
	}

	private void PushWebCommand(WebCommandBase cmd)
	{
		//if (MonoSingleton<IRVManager>.Instance.CurrentNetStatus == InternetReachabilityVerifier.Status.Offline)
		//{
		//	MonoSingleton<NetworkManager>.Instance.StopNetwork("Network Offline", cmd);
		//	return;
		//}
		//cmd.Pack();
		//POST(cmd);
	}
}