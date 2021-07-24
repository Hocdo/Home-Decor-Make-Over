public class NetRequestLoadMapData : NetRequestBase
{
	private const string subUrl = "load_map";

	public static void Request(WebCommand<ResPacketLoadMapData>.NetReqCompleteHandler callback)
	{
		ReqPacketLoadMapData req = new ReqPacketLoadMapData();
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketLoadMapData>.MakeFormForMapTool("load_map", req, delegate(ResPacketLoadMapData res)
		{
			callback(res);
		}, tryRetry: true));
	}

	public static void Request(int reloadGid, WebCommand<ResPacketLoadMapData>.NetReqCompleteHandler callback)
	{
		ReqPacketReloadMapData reqPacketReloadMapData = new ReqPacketReloadMapData();
		reqPacketReloadMapData.gid = reloadGid;
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketLoadMapData>.MakeFormForMapTool("load_map", reqPacketReloadMapData, delegate(ResPacketLoadMapData res)
		{
			callback(res);
		}));
	}
}
