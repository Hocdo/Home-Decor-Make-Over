using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class InternetReachabilityVerifier : MonoBehaviour
{
	public enum CaptivePortalDetectionMethod
	{
		DefaultByPlatform,
		Google204,
		GoogleBlank,
		MicrosoftNCSI,
		Apple,
		Ubuntu,
		Custom,
		Apple2
	}

	public delegate bool CustomMethodVerifierDelegate(WWW www, string customMethodExpectedData);

	public enum Status
	{
		Offline,
		PendingVerification,
		Error,
		Mismatch,
		NetVerified
	}

	public delegate void StatusChangedDelegate(Status newStatus);

	public CaptivePortalDetectionMethod captivePortalDetectionMethod;

	public string customMethodURL = string.Empty;

	public string customMethodExpectedData = "OK";

	public bool customMethodWithCacheBuster = true;

	public CustomMethodVerifierDelegate customMethodVerifierDelegate;

	public bool dontDestroyOnLoad = true;

	private float noInternetStartTime;

	private Status _status;

	private string _lastError = string.Empty;

	private static InternetReachabilityVerifier _instance = null;

	private static RuntimePlatform[] methodGoogle204Supported = new RuntimePlatform[6]
	{
		RuntimePlatform.WindowsPlayer,
		RuntimePlatform.WindowsEditor,
		RuntimePlatform.Android,
		RuntimePlatform.LinuxPlayer,
		RuntimePlatform.OSXPlayer,
		RuntimePlatform.OSXEditor
	};

	private const CaptivePortalDetectionMethod fallbackMethodIfNoDefaultByPlatform = CaptivePortalDetectionMethod.MicrosoftNCSI;

	private WaitForSeconds defaultReachabilityCheckPeriod = new WaitForSeconds(5f);

	private WaitForSeconds netVerificationErrorRetryTime = new WaitForSeconds(30f);

	private WaitForSeconds netVerificationMismatchRetryTime = new WaitForSeconds(7f);

	private bool netActivityRunning;

	private string apple2MethodURL = string.Empty;

	public Status status
	{
		get
		{
			return _status;
		}
		set
		{
			Status status = _status;
			_status = value;
			if (status == Status.NetVerified && _status != Status.NetVerified)
			{
				noInternetStartTime = Time.time;
			}
			if (this.statusChangedDelegate != null)
			{
				this.statusChangedDelegate(value);
			}
		}
	}

	public string lastError
	{
		get
		{
			return _lastError;
		}
		set
		{
			_lastError = value;
		}
	}

	public static InternetReachabilityVerifier Instance => _instance;

	public event StatusChangedDelegate statusChangedDelegate;

	public float getTimeWithoutInternetConnection()
	{
		if (status == Status.NetVerified)
		{
			return 0f;
		}
		return Time.time - noInternetStartTime;
	}

	public void setNetActivityTimes(float defaultReachabilityCheckPeriodSeconds, float netVerificationErrorRetryTimeSeconds, float netVerificationMismatchRetryTimeSeconds)
	{
		defaultReachabilityCheckPeriod = new WaitForSeconds(defaultReachabilityCheckPeriodSeconds);
		netVerificationErrorRetryTime = new WaitForSeconds(netVerificationErrorRetryTimeSeconds);
		netVerificationMismatchRetryTime = new WaitForSeconds(netVerificationMismatchRetryTimeSeconds);
	}

	private string getCaptivePortalDetectionURL(CaptivePortalDetectionMethod cpdm)
	{
		switch (cpdm)
		{
		case CaptivePortalDetectionMethod.Custom:
		{
			string text = customMethodURL;
			if (customMethodWithCacheBuster)
			{
				text = text + "?z=" + (UnityEngine.Random.Range(0, int.MaxValue) ^ 0x13377AA7);
			}
			return text;
		}
		case CaptivePortalDetectionMethod.Google204:
			return "https://clients3.google.com/generate_204";
		case CaptivePortalDetectionMethod.MicrosoftNCSI:
			return "https://www.msftncsi.com/ncsi.txt";
		case CaptivePortalDetectionMethod.GoogleBlank:
			return "https://www.google.com/blank.html";
		case CaptivePortalDetectionMethod.Apple:
			return "https://www.apple.com/library/test/success.html";
		case CaptivePortalDetectionMethod.Ubuntu:
			return "https://start.ubuntu.com/connectivity-check";
		case CaptivePortalDetectionMethod.Apple2:
			if (apple2MethodURL.Length == 0)
			{
				apple2MethodURL = "https://captive.apple.com/";
				char[] array = new char[17];
				for (int i = 0; i < 17; i++)
				{
					array[i] = (char)(97 + UnityEngine.Random.Range(0, 26));
				}
				array[8] = '/';
				apple2MethodURL += new string(array);
			}
			return apple2MethodURL;
		default:
			return string.Empty;
		}
	}

	private bool checkCaptivePortalDetectionResult(CaptivePortalDetectionMethod cpdm, WWW www)
	{
		if (www == null)
		{
			return false;
		}
		if (www.error != null && www.error.Length > 0)
		{
			return false;
		}
		switch (cpdm)
		{
		case CaptivePortalDetectionMethod.Custom:
			if (customMethodVerifierDelegate != null)
			{
				return customMethodVerifierDelegate(www, customMethodExpectedData);
			}
			if ((customMethodExpectedData.Length > 0 && www.text != null && www.text.StartsWith(customMethodExpectedData)) || (customMethodExpectedData.Length == 0 && (www.bytes == null || www.bytes.Length == 0)))
			{
				return true;
			}
			break;
		case CaptivePortalDetectionMethod.Google204:
		{
			Dictionary<string, string> responseHeaders = www.responseHeaders;
			string text = string.Empty;
			if (responseHeaders.ContainsKey("STATUS"))
			{
				text = responseHeaders["STATUS"];
			}
			else if (responseHeaders.ContainsKey("NULL"))
			{
				text = responseHeaders["NULL"];
			}
			if (text.Length > 0)
			{
				if (text.IndexOf("204 No Content") >= 0)
				{
					return true;
				}
			}
			else if (www.size == 0)
			{
				return true;
			}
			break;
		}
		case CaptivePortalDetectionMethod.GoogleBlank:
			if (www.size == 0)
			{
				return true;
			}
			break;
		case CaptivePortalDetectionMethod.MicrosoftNCSI:
			if (www.text.StartsWith("Microsoft NCSI"))
			{
				return true;
			}
			break;
		case CaptivePortalDetectionMethod.Apple:
		case CaptivePortalDetectionMethod.Apple2:
			if (www.text.IndexOf("<BODY>Success</BODY>") < 50)
			{
				return true;
			}
			break;
		case CaptivePortalDetectionMethod.Ubuntu:
			if (www.text.IndexOf("Lorem ipsum dolor sit amet") == 109)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private IEnumerator netActivity()
	{
		netActivityRunning = true;
		NetworkReachability prevUnityReachability = Application.internetReachability;
		if (Application.internetReachability != 0)
		{
			status = Status.PendingVerification;
		}
		else
		{
			status = Status.Offline;
		}
		noInternetStartTime = Time.time;
		while (netActivityRunning)
		{
			if (status == Status.Error)
			{
				yield return netVerificationErrorRetryTime;
				status = Status.PendingVerification;
			}
			else if (status == Status.Mismatch)
			{
				yield return netVerificationMismatchRetryTime;
				status = Status.PendingVerification;
			}
			NetworkReachability unityReachability = Application.internetReachability;
			if (prevUnityReachability != unityReachability)
			{
				if (unityReachability != 0)
				{
					status = Status.PendingVerification;
				}
				else if (unityReachability == NetworkReachability.NotReachable)
				{
					status = Status.Offline;
				}
				prevUnityReachability = Application.internetReachability;
			}
			if (status == Status.PendingVerification)
			{
				verifyCaptivePortalDetectionMethod();
				CaptivePortalDetectionMethod cpdm = captivePortalDetectionMethod;
				string url = getCaptivePortalDetectionURL(cpdm);
				WWW www = new WWW(url);
				yield return www;
				if (www.error != null && www.error.Length > 0)
				{
					lastError = www.error;
					status = Status.Error;
					continue;
				}
				if (!checkCaptivePortalDetectionResult(cpdm, www))
				{
					status = Status.Mismatch;
					continue;
				}
				status = Status.NetVerified;
			}
			yield return defaultReachabilityCheckPeriod;
		}
		netActivityRunning = false;
		status = Status.PendingVerification;
	}

	private void Awake()
	{
		if ((bool)_instance)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		_instance = this;
		if (dontDestroyOnLoad)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		customMethodURL = MonoSingleton<NetworkManager>.Instance.URL + customMethodURL;
	}

	private void verifyCaptivePortalDetectionMethod()
	{
		if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.DefaultByPlatform)
		{
			captivePortalDetectionMethod = CaptivePortalDetectionMethod.Google204;
			if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.DefaultByPlatform)
			{
				captivePortalDetectionMethod = CaptivePortalDetectionMethod.MicrosoftNCSI;
			}
		}
		if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.Google204 && Array.IndexOf(methodGoogle204Supported, Application.platform) < 0)
		{
			captivePortalDetectionMethod = CaptivePortalDetectionMethod.GoogleBlank;
		}
		if (captivePortalDetectionMethod == CaptivePortalDetectionMethod.Custom && customMethodURL.Length == 0)
		{
			base.enabled = false;
			if (netActivityRunning)
			{
				Stop();
			}
		}
	}

	private void Start()
	{
		verifyCaptivePortalDetectionMethod();
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			status = Status.NetVerified;
		}
		else
		{
			StartCoroutine("netActivity");
		}
	}

	private void OnDisable()
	{
		Stop();
	}

	private void OnEnable()
	{
		Start();
	}

	public void Stop()
	{
		StopCoroutine("netActivity");
	}
}
