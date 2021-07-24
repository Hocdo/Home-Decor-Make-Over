public class NetRequestSaveGameData : NetRequestBase
{
	private const string subUrl = "save_game";

	public static void Request(ReqPacketSaveGameData req, WebCommand<ResPacketSaveGameData>.NetReqCompleteHandler callback)
	{
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketSaveGameData>.MakeFormForMapTool("save_game", req, delegate(ResPacketSaveGameData res)
		{
			callback(res);
		}));
	}
}
