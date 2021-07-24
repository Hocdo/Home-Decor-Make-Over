using System.IO;
using UnityEngine;

public class IronSourceMediationSettings : ScriptableObject
{
	public static readonly string IRONSOURCE_SETTINGS_ASSET_PATH = Path.Combine(IronSourceConstants.IRONSOURCE_RESOURCES_PATH, IronSourceConstants.IRONSOURCE_MEDIATION_SETTING_NAME + ".asset");

	[Header("Ironsource AppKey")]
	[Tooltip("Add your application AppKeys, as provided in Ironsource Platform")]
	public string AndroidAppKey = string.Empty;
	[Tooltip("Add your application AppKeys, as provided in Ironsource Platform")]
	public string IOSAppKey = string.Empty;

	[Header("Automatic Initialization")]
	[Tooltip("Use this flag when you wish to initialize all ad units (recommended)")]
	public bool EnableIronsourceSDKInitAPI;

    [Header("Ironsource SKAdNetwork ID")]
	[Tooltip("Add Ironsource SKAdNetworkIdentifier to your Info.plist for iOS 14+")]
    public bool AddIronsourceSkadnetworkID;

    [Header("Project Features")]
	public bool EnableAdapterDebug;

	public bool EnableIntegrationHelper;
}
