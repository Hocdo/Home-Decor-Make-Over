public class NetRequestLoadGameData : NetRequestBase
{
	private const string subUrl = "load_game";

	public static void Request(WebCommand<ResPacketLoadGameData>.NetReqCompleteHandler callback)
	{
		ReqPacketLoadGameData req = new ReqPacketLoadGameData();
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketLoadGameData>.MakeFormForMapTool("load_game", req, delegate(ResPacketLoadGameData res)
		{
			callback(res);
		}, tryRetry: true));
	}

	public static void Request(int reloadGid, WebCommand<ResPacketLoadGameData>.NetReqCompleteHandler callback)
	{
		ReqPacketReloadGameData reqPacketReloadGameData = new ReqPacketReloadGameData();
		reqPacketReloadGameData.gid = reloadGid;
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketLoadGameData>.MakeFormForMapTool("load_game", reqPacketReloadGameData, delegate(ResPacketLoadGameData res)
		{
			callback(res);
		}));
	}
}
