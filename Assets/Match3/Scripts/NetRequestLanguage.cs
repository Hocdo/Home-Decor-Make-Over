public class NetRequestLanguage : NetRequestBase
{
	private const string subUrl = "m_json_lang.php";

	public static void Request(string langCode, WebCommand<ResPacketLanguage>.NetReqCompleteHandler callback)
	{
		ReqPacketLanguage reqPacketLanguage = new ReqPacketLanguage();
		reqPacketLanguage.lang = langCode;
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketLanguage>.MakeForm("m_json_lang.php", reqPacketLanguage, delegate(ResPacketLanguage res)
		{
			callback(res);
		}));
	}
}
