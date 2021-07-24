using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.RemoteConfig;
using Firebase.Extensions;

public class RemoteConfig : MonoBehaviour
{
	Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
	public static int levelStartAds = 3;
	public static int enablelocalizationTutorial;
	protected bool isFirebaseInitialized = false;
	// Use this for initialization
	void Start()
	{
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
			dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				InitializeFirebase();
			}
			else
			{
				Debug.LogError(
				  "Could not resolve all Firebase dependencies: " + dependencyStatus);
			}
		});
	}

	void InitializeFirebase()
	{
		System.Collections.Generic.Dictionary<string, object> defaults =
		  new System.Collections.Generic.Dictionary<string, object>();

		// These are the values that are used if we haven't fetched data from the
		// server
		// yet, or if we ask for values that the server doesn't have:
		// 		
		defaults.Add("config_level_start_ads", levelStartAds);
	

		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
		  .ContinueWithOnMainThread(task => {
			// [END set_defaults]
			Debug.Log("RemoteConfig configured and ready!");
			  isFirebaseInitialized = true;
			  FetchDataAsync();
		  });
	}
	
	public void GetData()
	{
		//Debug.Log("config_test_string: " +
		//		 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
		//		 .GetValue("config_test_string").StringValue);

		levelStartAds = (int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
				 .GetValue("config_level_start_ads").LongValue;

		Debug.Log("config_level_start_ads: " +
				 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
				 .GetValue("config_level_start_ads").LongValue);

		//Debug.Log("config_test_float: " +
		//		 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
		//		 .GetValue("config_test_float").DoubleValue);
		//Debug.Log("config_test_bool: " +
		//		 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
		//		 .GetValue("config_test_bool").BooleanValue);
	}
	public void ShowKeys()
	{
		Debug.Log("Current Keys:");
		System.Collections.Generic.IEnumerable<string> keys =
			Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Keys;
		foreach (string key in keys)
		{
			Debug.Log(" " + key);
		}
		Debug.Log("GetKeysByPrefix(\"config_level_s\"):");
		keys = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetKeysByPrefix("config_level_s");
		foreach (string key in keys)
		{
			Debug.Log(" " + key);
		}
	}
	// Start a fetch request.
	public Task FetchDataAsync()
	{
		
		System.Threading.Tasks.Task fetchTask =
		Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
			TimeSpan.Zero);
		return fetchTask.ContinueWithOnMainThread(FetchComplete);
	}

	void FetchComplete(Task fetchTask)
	{
		if (fetchTask.IsCanceled)
		{
			Debug.Log("Fetch canceled.");
		}
		else if (fetchTask.IsFaulted)
		{
			Debug.Log("Fetch encountered an error.");
		}
		else if (fetchTask.IsCompleted)
		{
			Debug.Log("Fetch completed successfully!");
		}

		var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
		switch (info.LastFetchStatus)
		{
			case Firebase.RemoteConfig.LastFetchStatus.Success:
				Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
				.ContinueWithOnMainThread(task => {
					Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
								   info.FetchTime));

					GetData();					
				});

				break;
			case Firebase.RemoteConfig.LastFetchStatus.Failure:
				switch (info.LastFetchFailureReason)
				{
					case Firebase.RemoteConfig.FetchFailureReason.Error:
						Debug.Log("Fetch failed for unknown reason");
						break;
					case Firebase.RemoteConfig.FetchFailureReason.Throttled:
						Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
						break;
				}
				break;
			case Firebase.RemoteConfig.LastFetchStatus.Pending:
				Debug.Log("Latest Fetch call still pending.");
				break;
		}
	}
}
