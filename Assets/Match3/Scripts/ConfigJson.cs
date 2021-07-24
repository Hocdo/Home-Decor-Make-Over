public class ConfigJson
{
	public string AppContentVersion;

	public string AppVersion;

	public int AppVersionCode;

	public int AppVersionNumber;

	public ConfigJson()
	{
		AppVersion = (AppContentVersion = "1.0.0");
		AppVersionNumber = 100000;
	}
}
