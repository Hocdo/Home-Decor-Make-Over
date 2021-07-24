public class NetRequestResume : NetRequestBase
{
	private const string subUrl = "resume";

	public static void Request()
	{
		ReqPacketBase req = new ReqPacketBase();
		MonoSingleton<NetworkManager>.Instance.StartNetwork(WebCommand<ResPacketBase>.MakeForm("resume", req, null));
	}
}
