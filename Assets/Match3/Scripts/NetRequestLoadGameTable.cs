public class NetRequestLoadGameTable : NetRequestBase
{
	private const string subUrl = "game_data";

	public static void RequestCheckTableVersion(WebCommand<ResPacketCheckTableVersion>.NetReqCompleteHandler callback)
	{
		ReqPacketLoadGameTable req = new ReqPacketLoadGameTable(1, string.Empty);
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketCheckTableVersion>.MakeForm("game_data", req, delegate(ResPacketCheckTableVersion res)
		{
			callback(res);
		}));
	}

	public static void Request(WebCommand<ResPacketLoadGameTable>.NetReqCompleteHandler callback, string _tableInfoListValue)
	{
		ReqPacketLoadGameTable req = new ReqPacketLoadGameTable(0, _tableInfoListValue);
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketLoadGameTable>.MakeForm("game_data", req, delegate(ResPacketLoadGameTable res)
		{
			callback(res);
		}));
	}
}
