public class NetRequestSaveMapData : NetRequestBase
{
	private const string subUrl = "save_map";

	public static void Request(ReqPacketSaveMapData req, WebCommand<ResPacketSaveMapData>.NetReqCompleteHandler callback)
	{
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketSaveMapData>.MakeFormForMapTool("save_map", req, delegate(ResPacketSaveMapData res)
		{
			callback(res);
		}));
	}

	public static void RequestClearAllMap(ReqPacketClearAllMapData req, WebCommand<ResPacketSaveMapData>.NetReqCompleteHandler callback)
	{
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketSaveMapData>.MakeFormForMapTool("save_map", req, delegate(ResPacketSaveMapData res)
		{
			callback(res);
		}));
	}
}
