//@TODO ENABLE_ZLIB
//#define ENABLE_ZLIB

#if ENABLE_ZLIB
using Ionic.Zlib;
#endif

using System;
using System.IO;
using System.Text;
using UnityEngine;

public class WebCommand<RES_PACKET> : WebCommandBase where RES_PACKET : ResPacketBase
{
	public delegate void NetReqCompleteHandler(RES_PACKET res);

	public NetReqCompleteHandler OnComplete;

	public ReqPacketBase req;

	public ResPacketBase res;

	public string SubUrl;

	public static WebCommand<RES_PACKET> MakeCommand(string subUrl, ReqPacketBase req, NetReqCompleteHandler callback, bool tryRetry = false)
	{
		WebCommand<RES_PACKET> webCommand = new WebCommand<RES_PACKET>();
		webCommand.SubUrl = subUrl;
		webCommand.req = req;
		webCommand.OnComplete = callback;
		webCommand.IsRetry = tryRetry;
		webCommand.requestBodyJson = true;
		return webCommand.MakeParam(MonoSingleton<NetworkManager>.Instance.URL + subUrl).AES128(MonoSingleton<NetworkManager>.Instance.GetPacketKey());
	}

	public static WebCommand<RES_PACKET> MakeForm(string subUrl, ReqPacketBase req, NetReqCompleteHandler callback, bool testMapTool = false, bool tryRetry = false, bool ignoreNetworkOrder = false)
	{
		WebCommand<RES_PACKET> webCommand = new WebCommand<RES_PACKET>();
		webCommand.SubUrl = subUrl;
		webCommand.req = req;
		webCommand.OnComplete = callback;
		webCommand.IsRetry = tryRetry;
		webCommand.requestBodyJson = false;
		webCommand.IsRetComressed = ((req.zip == 1) ? true : false);
		webCommand.IgnoreNetworkOrder = ignoreNetworkOrder;
		return webCommand.MakeParam(((!testMapTool) ? MonoSingleton<NetworkManager>.Instance.URL : MonoSingleton<NetworkManager>.Instance.MapToolURL) + subUrl).AES128(MonoSingleton<NetworkManager>.Instance.GetPacketKey());
	}

	public static WebCommand<RES_PACKET> MakeFormForMapTool(string subUrl, ReqPacketBase req, NetReqCompleteHandler callback, bool tryRetry = false)
	{
		WebCommand<RES_PACKET> webCommand = new WebCommand<RES_PACKET>();
		webCommand.SubUrl = subUrl;
		webCommand.req = req;
		webCommand.OnComplete = callback;
		webCommand.IsRetry = tryRetry;
		webCommand.requestBodyJson = false;
		webCommand.IsRetComressed = false;
		return webCommand.MakeParam(MonoSingleton<NetworkManager>.Instance.MapToolURL + subUrl + "?is_mobile=m").AES128(MonoSingleton<NetworkManager>.Instance.GetPacketKey());
	}

	public WebCommand<RES_PACKET> AES128(string key)
	{
		AES128Key = key;
		return this;
	}

	public WebCommand<RES_PACKET> MakeParam(string url)
	{
		TimeOut = WebCommandBase.DefaultTimeout;
		URL = url;
		MakeKey();
		return this;
	}

	public override void Pack()
	{
		reqBase = req;
		try
		{
			RequestString = JsonUtility.ToJson(req);
		}
		catch (Exception)
		{
		}
	}

	private static sbyte[] GetBytes(string str)
	{
		sbyte[] array = new sbyte[str.Length * 2];
		Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
		return array;
	}

	public override void UnPack(bool compressed)
	{
#if ENABLE_ZLIB
		try
		{
			object obj = null;
			if (compressed)
			{
				if (ResponseByteData != null && ResponseByteData.Length > 0)
				{
					using (Stream stream = new MemoryStream(ResponseByteData))
					{
						using (Stream stream2 = new GZipStream(stream, CompressionMode.Decompress))
						{
							using (StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8))
							{
								string text = streamReader.ReadToEnd();
								obj = JsonUtility.FromJson<RES_PACKET>(text);
								ResponseStringData = text;
							}
						}
					}
				}
			}
			else if (ResponseStringData != null && ResponseStringData.Length > 0)
			{
				obj = JsonUtility.FromJson<RES_PACKET>(ResponseStringData.Trim());
			}
			if (obj != null)
			{
				res = (RES_PACKET)obj;
				if (ResponseStringData != null)
				{
					res.originalJson = ResponseStringData;
				}
			}
		}
		catch (Exception)
		{
			res = null;
		}
#endif
	}

	public override void Complete()
	{
		if (OnComplete == null || (OnComplete.Target != null && OnComplete.Target.Equals(null)))
		{
			return;
		}
		if (res != null)
		{
			bool flag = false;
			if (res is IPacketBaseError)
			{
				flag = ((IPacketBaseError)res).IsError();
			}
			if (res.crypt_fail <= 0 && flag)
			{
				if (string.IsNullOrEmpty(RequestString))
				{
				}
				return;
			}
		}
		OnComplete((RES_PACKET)res);
	}
}
