using antilunchbox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("AntiLunchBox/SoundManager")]
public class SoundManager : antilunchbox.Singleton<SoundManager>
{
	public enum PlayMethod
	{
		ContinuousPlayThrough,
		ContinuousPlayThroughWithDelay,
		ContinuousPlayThroughWithRandomDelayInRange,
		OncePlayThrough,
		OncePlayThroughWithDelay,
		OncePlayThroughWithRandomDelayInRange,
		ShufflePlayThrough,
		ShufflePlayThroughWithDelay,
		ShufflePlayThroughWithRandomDelayInRange
	}

	public delegate void SongCallBack();

	public const string VIEW = "view";

	public const string EDIT = "edit";

	public const string HIDE = "hide";

	private bool _viewAll;

	[SerializeField]
	public Hashtable songStatus = new Hashtable();

	public bool helpOn;

	public bool showInfo = true;

	public bool showDev = true;

	public bool showList = true;

	public bool showAdd = true;

	public bool showSFX = true;

	public List<bool> showSFXDetails = new List<bool>();

	public int groupAddIndex;

	public int autoPrepoolAmount;

	public float autoBaseVolume = 1f;

	public float autoVolumeVariation;

	public float autoPitchVariation;

	public bool showAsGrouped;

	public List<SoundConnection> soundConnections = new List<SoundConnection>();

	[SerializeField]
	public AudioSource[] audios;

	private bool[] audiosPaused;

	public string currentLevel;

	public SoundConnection currentSoundConnection;

	private AudioSource currentSource;

	public float crossDuration = 5f;

	private float modifiedCrossDuration;

	public bool showDebug = true;

	public bool offTheBGM;

	private bool ignoreCrossDuration;

	private int currentPlaying;

	private bool silentLevel;

	public bool isPaused;

	private bool skipSongs;

	private int skipAmount;

	private bool[] inCrossing = new bool[2];

	private bool[] outCrossing = new bool[2];

	public bool movingOnFromSong;

	private float lastLevelLoad;

	public const int SOUNDMANAGER_FALSE = -1;

	public SongCallBack OnSongEnd;

	public SongCallBack OnSongBegin;

	public SongCallBack OnCrossOutBegin;

	public SongCallBack OnCrossInBegin;

	private SongCallBack InternalCallback;

	private int currentSongIndex = -1;

	private bool ignoreFromLosingFocus;

	public bool ignoreLevelLoad;

	private float _maxMusicVolume = 1f;

	private float _maxVolume = 1f;

	private bool _mutedMusic;

	public string resourcesPath = "Sounds/SFX";

	public List<AudioClip> storedSFXs = new List<AudioClip>();

	public List<GameObject> unOwnedSFXObjects = new List<GameObject>();

	public Dictionary<int, string> cappedSFXObjects = new Dictionary<int, string>();

	public Dictionary<AudioSource, float> delayedAudioSources = new Dictionary<AudioSource, float>();

	public Dictionary<AudioSource, SongCallBack> runOnEndFunctions = new Dictionary<AudioSource, SongCallBack>();

	private AudioSource duckSource;

	private SongCallBack duckFunction;

	private bool isDucking;

	private int duckNumber;

	private float preDuckVolume = 1f;

	private float preDuckVolumeMusic = 1f;

	private float preDuckVolumeSFX = 1f;

	private float preDuckPitch = 1f;

	private float preDuckPitchMusic = 1f;

	private float preDuckPitchSFX = 1f;

	public static float duckStartSpeed = 0.1f;

	public static float duckEndSpeed = 0.5f;

	public List<SFXGroup> sfxGroups = new List<SFXGroup>();

	public List<string> clipToGroupKeys = new List<string>();

	public List<string> clipToGroupValues = new List<string>();

	private Dictionary<string, SFXGroup> groups = new Dictionary<string, SFXGroup>();

	private Dictionary<string, string> clipsInGroups = new Dictionary<string, string>();

	private Dictionary<string, AudioClip> allClips = new Dictionary<string, AudioClip>();

	private Dictionary<string, int> prepools = new Dictionary<string, int>();

	private Dictionary<string, float> baseVolumes = new Dictionary<string, float>();

	private Dictionary<string, float> volumeVariations = new Dictionary<string, float>();

	private Dictionary<string, float> pitchVariations = new Dictionary<string, float>();

	public bool offTheSFX;

	public int capAmount = 3;

	private float _volumeSFX = 1f;

	private float _pitchSFX = 1f;

	private float _maxSFXVolume = 1f;

	private bool _mutedSFX;

	private Dictionary<AudioClip, SFXPoolInfo> ownedPools = new Dictionary<AudioClip, SFXPoolInfo>();

	public List<int> sfxPrePoolAmounts = new List<int>();

	public List<float> sfxBaseVolumes = new List<float>();

	public List<float> sfxVolumeVariations = new List<float>();

	public List<float> sfxPitchVariations = new List<float>();

	public float SFXObjectLifetime = 10f;

	public List<string> currentPockets = new List<string>
	{
		"Default"
	};

	public float defaultSFXSpatialBlend;

	public bool viewAll
	{
		get
		{
			return _viewAll;
		}
		set
		{
			_viewAll = value;
			List<string> list = new List<string>();
			IDictionaryEnumerator enumerator = songStatus.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					list.Add(((DictionaryEntry)enumerator.Current).Key.ToString());
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			foreach (string item in list)
			{
				if (_viewAll)
				{
					songStatus[item] = "view";
				}
				else
				{
					songStatus[item] = "hide";
				}
			}
		}
	}

	public static SoundManager Instance
	{
		get
		{
			return (SoundManager)antilunchbox.Singleton<SoundManager>.mInstance;
		}
		set
		{
			antilunchbox.Singleton<SoundManager>.mInstance = value;
		}
	}

	public float volume1
	{
		get
		{
			return audios[0].volume;
		}
		set
		{
			audios[0].volume = value;
		}
	}

	public float volume2
	{
		get
		{
			return audios[1].volume;
		}
		set
		{
			audios[1].volume = value;
		}
	}

	public float maxMusicVolume
	{
		get
		{
			return _maxMusicVolume;
		}
		set
		{
			_maxMusicVolume = value;
		}
	}

	public float maxVolume
	{
		get
		{
			return _maxVolume;
		}
		set
		{
			_maxVolume = value;
		}
	}

	public bool mutedMusic
	{
		get
		{
			return _mutedMusic;
		}
		set
		{
			AudioSource obj = audios[0];
			bool mute = value;
			audios[1].mute = mute;
			obj.mute = mute;
			_mutedMusic = value;
		}
	}

	public bool muted
	{
		get
		{
			return mutedMusic || mutedSFX;
		}
		set
		{
			bool mutedMusic = mutedSFX = value;
			this.mutedMusic = mutedMusic;
		}
	}

	private bool crossingIn => inCrossing[0] || inCrossing[1];

	private bool crossingOut => outCrossing[0] || outCrossing[1];

	public float volumeSFX
	{
		get
		{
			return _volumeSFX;
		}
		set
		{
			foreach (KeyValuePair<AudioClip, SFXPoolInfo> ownedPool in Instance.ownedPools)
			{
				foreach (GameObject item in ownedPool.Value.ownedAudioClipPool)
				{
					if (item != null && item.GetComponent<AudioSource>() != null && (!isDucking || item.GetComponent<AudioSource>() != duckSource))
					{
						item.GetComponent<AudioSource>().volume = value;
					}
				}
			}
			foreach (GameObject unOwnedSFXObject in Instance.unOwnedSFXObjects)
			{
				if (unOwnedSFXObject != null && unOwnedSFXObject.GetComponent<AudioSource>() != null && (!isDucking || unOwnedSFXObject.GetComponent<AudioSource>() != duckSource))
				{
					unOwnedSFXObject.GetComponent<AudioSource>().volume = value;
				}
			}
			_volumeSFX = value;
		}
	}

	public float pitchSFX
	{
		get
		{
			return _pitchSFX;
		}
		set
		{
			foreach (KeyValuePair<AudioClip, SFXPoolInfo> ownedPool in Instance.ownedPools)
			{
				foreach (GameObject item in ownedPool.Value.ownedAudioClipPool)
				{
					if (item != null && item.GetComponent<AudioSource>() != null && (!isDucking || item.GetComponent<AudioSource>() != duckSource))
					{
						item.GetComponent<AudioSource>().pitch = value;
					}
				}
			}
			foreach (GameObject unOwnedSFXObject in Instance.unOwnedSFXObjects)
			{
				if (unOwnedSFXObject != null && unOwnedSFXObject.GetComponent<AudioSource>() != null && (!isDucking || unOwnedSFXObject.GetComponent<AudioSource>() != duckSource))
				{
					unOwnedSFXObject.GetComponent<AudioSource>().pitch = value;
				}
			}
			_pitchSFX = value;
		}
	}

	public float maxSFXVolume
	{
		get
		{
			return _maxSFXVolume;
		}
		set
		{
			_maxSFXVolume = value;
		}
	}

	public bool mutedSFX
	{
		get
		{
			return _mutedSFX;
		}
		set
		{
			foreach (KeyValuePair<AudioClip, SFXPoolInfo> ownedPool in Instance.ownedPools)
			{
				foreach (GameObject item in ownedPool.Value.ownedAudioClipPool)
				{
					if (item != null && item.GetComponent<AudioSource>() != null)
					{
						if (value)
						{
							item.GetComponent<AudioSource>().mute = value;
						}
						else if (!Instance.offTheSFX)
						{
							item.GetComponent<AudioSource>().mute = value;
						}
					}
				}
			}
			foreach (GameObject unOwnedSFXObject in Instance.unOwnedSFXObjects)
			{
				if (unOwnedSFXObject != null && unOwnedSFXObject.GetComponent<AudioSource>() != null)
				{
					if (value)
					{
						unOwnedSFXObject.GetComponent<AudioSource>().mute = value;
					}
					else if (!Instance.offTheSFX)
					{
						unOwnedSFXObject.GetComponent<AudioSource>().mute = value;
					}
				}
			}
			_mutedSFX = value;
		}
	}

	private void Update()
	{
		HandleSFX();
	}

	private void OnLevelFinishedLoad(Scene scene, LoadSceneMode mode)
	{
		OnLevelFinishedLoaded(scene.buildIndex);
	}

	private void OnLevelFinishedLoaded(int level)
	{
		if (Instance == this && !ignoreLevelLoad)
		{
			HandleLevel(level);
		}
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoad;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoad;
	}

	private void OnApplicationPause(bool pause)
	{
		CommonHandleApplicationFocus(pause);
	}

	private void CommonHandleApplicationFocus(bool applicationFocus)
	{
		if (applicationFocus)
		{
			ignoreFromLosingFocus = true;
		}
		else
		{
			ignoreFromLosingFocus = false;
		}
	}

	public static void AddSoundConnection(SoundConnection sc)
	{
		int num = SoundConnectionsContainsThisLevel(sc.level);
		if (num == -1)
		{
			if (!Application.CanStreamedLevelBeLoaded(sc.level))
			{
				sc.isCustomLevel = true;
			}
			Instance.soundConnections.Add(sc);
		}
	}

	public static void RemoveSoundConnectionForLevel(string lvl)
	{
		int num = SoundConnectionsContainsThisLevel(lvl);
		if (num != -1)
		{
			Instance.soundConnections.RemoveAt(num);
		}
	}

	public static void ReplaceSoundConnection(SoundConnection sc)
	{
		int num = SoundConnectionsContainsThisLevel(sc.level);
		if (num != -1)
		{
			RemoveSoundConnectionForLevel(sc.level);
		}
		AddSoundConnection(sc);
	}

	public static int SoundConnectionsContainsThisLevel(string lvl)
	{
		return Instance.soundConnections.FindIndex((SoundConnection sc) => sc.level == lvl);
	}

	public static SoundConnection GetSoundConnectionForThisLevel(string lvl)
	{
		int num = SoundConnectionsContainsThisLevel(lvl);
		if (num == -1)
		{
			return null;
		}
		return Instance.soundConnections[num];
	}

	public static bool MuteMusic()
	{
		return MuteMusic(!Instance.mutedMusic);
	}

	public static bool MuteMusic(bool toggle)
	{
		Instance.mutedMusic = toggle;
		return Instance.mutedMusic;
	}

	public static bool IsMusicMuted()
	{
		return Instance.mutedMusic;
	}

	public static bool Mute()
	{
		bool flag = MuteMusic();
		MuteSFX(flag);
		return flag;
	}

	public static bool Mute(bool toggle)
	{
		return MuteMusic(MuteSFX(toggle));
	}

	public static bool IsMuted()
	{
		return Instance.muted;
	}

	public static void SetVolumeMusic(float setVolume)
	{
		setVolume = Mathf.Clamp01(setVolume);
		float num = Instance.volume1 / Instance.maxMusicVolume;
		float num2 = Instance.volume2 / Instance.maxMusicVolume;
		Instance.maxMusicVolume = setVolume * Instance.maxVolume;
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			num = ((!Instance.audios[0].isPlaying) ? 0f : 1f);
		}
		if (float.IsNaN(num2) || float.IsInfinity(num2))
		{
			num2 = ((!Instance.audios[1].isPlaying) ? 0f : 1f);
		}
		Instance.volume1 = Instance.maxMusicVolume * num;
		Instance.volume2 = Instance.maxMusicVolume * num2;
	}

	public static float GetVolumeMusic()
	{
		return Instance.maxMusicVolume;
	}

	public static void SetPitchMusic(float setPitch)
	{
		AudioSource obj = Instance.audios[0];
		Instance.audios[1].pitch = setPitch;
		obj.pitch = setPitch;
	}

	public static float GetPitchMusic()
	{
		return Instance.audios[0].pitch;
	}

	public static void SetVolume(float setVolume)
	{
		setVolume = Mathf.Clamp01(setVolume);
		float num = Instance.maxMusicVolume / Instance.maxVolume;
		float num2 = Instance.maxSFXVolume / Instance.maxVolume;
		Instance.maxVolume = setVolume;
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			num = 1f;
		}
		if (float.IsNaN(num2) || float.IsInfinity(num2))
		{
			num2 = 1f;
		}
		SetVolumeMusic(num);
		SetVolumeSFX(num2);
	}

	public static float GetVolume()
	{
		return Instance.maxVolume;
	}

	public static void SetPitch(float setPitch)
	{
		SetPitchMusic(setPitch);
		SetPitchSFX(setPitch);
	}

	public static float GetPitch()
	{
		return GetPitchMusic();
	}

	public static float GetCrossDuration()
	{
		return Instance.crossDuration;
	}

	public static void SetCrossDuration(float duration)
	{
		Instance.crossDuration = duration;
	}

	public static string GetDefaultResourcesPath()
	{
		return Instance.resourcesPath;
	}

	public static void SetDefaultResourcesPath(string path)
	{
		Instance.resourcesPath = path;
	}

	public static SoundConnection GetCurrentSoundConnection()
	{
		return Instance.currentSoundConnection;
	}

	public static void SetCurrentSoundConnection(SoundConnection connection)
	{
		Instance.currentSoundConnection = connection;
	}

	public static void SetDisableBGM(bool disabled)
	{
		Instance.offTheBGM = disabled;
	}

	public static void SetDisableSFX(bool disabled)
	{
		Instance.offTheSFX = disabled;
	}

	public static void PlayConnection(SoundConnection sc, bool syncPlaybackTime = false, int trackNumber = 0)
	{
		int num = 0;
		AudioSource audioSource = null;
		if (syncPlaybackTime)
		{
			audioSource = GetCurrentAudioSource();
			if (audioSource != null)
			{
				num = audioSource.timeSamples;
			}
		}
		for (int i = 0; i < trackNumber; i++)
		{
			Next();
		}
		Instance._PlayConnection(sc);
		if (syncPlaybackTime)
		{
			audioSource = GetCurrentAudioSource();
			if (num > audioSource.clip.samples)
			{
				num = 0;
			}
			audioSource.timeSamples = num;
		}
	}

	public static void PlayConnection(string levelName, bool syncPlaybackTime = false, int trackNumber = 0)
	{
		int num = 0;
		AudioSource audioSource = null;
		if (syncPlaybackTime)
		{
			audioSource = GetCurrentAudioSource();
			if (audioSource != null)
			{
				num = audioSource.timeSamples;
			}
		}
		for (int i = 0; i < trackNumber; i++)
		{
			Next();
		}
		Instance._PlayConnection(levelName);
		if (syncPlaybackTime)
		{
			audioSource = GetCurrentAudioSource();
			if (num > audioSource.clip.samples)
			{
				num = 0;
			}
			audioSource.timeSamples = num;
		}
	}

	public static SoundConnection CreateSoundConnection(string lvl, params AudioClip[] audioList)
	{
		SoundConnection soundConnection = new SoundConnection(lvl, PlayMethod.ContinuousPlayThrough, audioList);
		if (!Application.CanStreamedLevelBeLoaded(lvl))
		{
			soundConnection.SetToCustom();
		}
		return soundConnection;
	}

	public static SoundConnection CreateSoundConnection(string lvl, PlayMethod method, params AudioClip[] audioList)
	{
		SoundConnection soundConnection = new SoundConnection(lvl, method, audioList);
		if (!Application.CanStreamedLevelBeLoaded(lvl))
		{
			soundConnection.SetToCustom();
		}
		return soundConnection;
	}

	public static SoundConnection CreateSoundConnection(string lvl, PlayMethod method, float delayPlay, params AudioClip[] audioList)
	{
		SoundConnection soundConnection = new SoundConnection(lvl, method, delayPlay, audioList);
		if (!Application.CanStreamedLevelBeLoaded(lvl))
		{
			soundConnection.SetToCustom();
		}
		return soundConnection;
	}

	public static SoundConnection CreateSoundConnection(string lvl, PlayMethod method, float minDelayPlay, float maxDelayPlay, params AudioClip[] audioList)
	{
		SoundConnection soundConnection = new SoundConnection(lvl, method, minDelayPlay, maxDelayPlay, audioList);
		if (!Application.CanStreamedLevelBeLoaded(lvl))
		{
			soundConnection.SetToCustom();
		}
		return soundConnection;
	}

	public static void PlayImmediately(AudioClip clip2play, bool loop = false, SongCallBack runOnEndFunction = null)
	{
		Instance._PlayImmediately(clip2play, loop, runOnEndFunction);
	}

	public static void Play(AudioClip clip2play, bool loop = false, SongCallBack runOnEndFunction = null)
	{
		Instance._Play(clip2play, loop, runOnEndFunction);
	}

	public static void StopMusicImmediately()
	{
		Instance._StopMusicImmediately();
	}

	public static void StopMusic()
	{
		Instance._StopMusic();
	}

	public static void Stop()
	{
		StopMusicImmediately();
		StopSFX();
	}

	public static void Pause()
	{
		Instance._Pause();
	}

	public static void UnPause()
	{
		Instance._UnPause();
	}

	public static void PauseToggle()
	{
		if (Instance.isPaused)
		{
			UnPause();
		}
		else
		{
			Pause();
		}
	}

	public static bool IsPaused()
	{
		return Instance.isPaused;
	}

	public static void SetIgnoreLevelLoad(bool ignore)
	{
		Instance.ignoreLevelLoad = ignore;
	}

	public static List<AudioClip> GetCurrentSongList()
	{
		if (Instance.currentSoundConnection == null)
		{
			return null;
		}
		return Instance.currentSoundConnection.soundsToPlay;
	}

	public static AudioClip GetCurrentSong()
	{
		if (!Instance.IsPlaying() || Instance.currentSource.clip == null)
		{
			return null;
		}
		return Instance.currentSource.clip;
	}

	public static void Next()
	{
		Instance.skipAmount++;
		Instance.skipSongs = true;
	}

	public static void Prev()
	{
		Instance.skipAmount--;
		Instance.skipSongs = true;
	}

	public static bool ClipNameIsValid(string clipName)
	{
		if (string.IsNullOrEmpty(clipName))
		{
			return false;
		}
		return Instance.allClips.ContainsKey(clipName) && Instance.allClips[clipName] != null;
	}

	public static bool GroupNameIsValid(string groupName)
	{
		if (string.IsNullOrEmpty(groupName))
		{
			return false;
		}
		return Instance.groups.ContainsKey(groupName) && Instance.groups[groupName] != null;
	}

	public static AudioSource GetCurrentAudioSource()
	{
		return Instance.IsPlaying(GetCurrentSong());
	}

	public static int GetTrackNumber(AudioClip clip)
	{
		return GetCurrentSongList().IndexOf(clip);
	}

	private void Awake()
	{
		Setup();
	}

	private void Setup()
	{
		if ((bool)Instance && Instance.gameObject != base.gameObject)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		base.gameObject.name = base.name;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		ClearAudioSources();
		Init();
		SetupSoundFX();
		OnLevelFinishedLoaded(SceneManager.GetActiveScene().buildIndex);
	}

	private void Init()
	{
		if ((bool)Instance)
		{
			audios = new AudioSource[2];
			audios[0] = base.gameObject.AddComponent<AudioSource>();
			audios[1] = base.gameObject.AddComponent<AudioSource>();
			audiosPaused = new bool[2];
			audiosPaused[0] = (audiosPaused[1] = false);
			audios[0].hideFlags = HideFlags.HideInInspector;
			audios[1].hideFlags = HideFlags.HideInInspector;
			SoundManagerTools.make2D(ref audios[0]);
			SoundManagerTools.make2D(ref audios[1]);
			audios[0].volume = 0f;
			audios[1].volume = 0f;
			audios[0].ignoreListenerVolume = true;
			audios[1].ignoreListenerVolume = true;
			maxVolume = AudioListener.volume;
			currentPlaying = CheckWhosPlaying();
		}
	}

	private void ClearAudioSources()
	{
		AudioSource[] components = base.gameObject.GetComponents<AudioSource>();
		AudioSource[] array = components;
		foreach (AudioSource obj in array)
		{
			UnityEngine.Object.Destroy(obj);
		}
	}

	private void _PlayConnection(SoundConnection sc)
	{
		if (offTheBGM || isPaused)
		{
			return;
		}
		if (string.IsNullOrEmpty(sc.level))
		{
			int num = 1;
			while (SoundConnectionsContainsThisLevel("CustomConnection" + num.ToString()) != -1)
			{
				num++;
			}
			sc.level = "CustomConnection" + num.ToString();
		}
		StopPreviousPlaySoundConnection();
		StartCoroutine("PlaySoundConnection", sc);
	}

	private void _PlayConnection(string levelName)
	{
		if (!offTheBGM && !isPaused)
		{
			int num = SoundConnectionsContainsThisLevel(levelName);
			if (num != -1)
			{
				StopPreviousPlaySoundConnection();
				StartCoroutine("PlaySoundConnection", soundConnections[num]);
			}
		}
	}

	private void StopPreviousPlaySoundConnection()
	{
		StopCoroutine("PlaySoundConnection");
	}

	private int CheckWhosNotPlaying()
	{
		if (!audios[0].isPlaying)
		{
			return 0;
		}
		if (!audios[1].isPlaying)
		{
			return 1;
		}
		return -1;
	}

	private int CheckWhosPlaying()
	{
		if (audios[0].isPlaying)
		{
			return 0;
		}
		if (audios[1].isPlaying)
		{
			return 1;
		}
		return -1;
	}

	private void HandleLevel(int level)
	{
		if (base.gameObject != base.gameObject || isPaused || (Time.realtimeSinceStartup != 0f && lastLevelLoad == Time.realtimeSinceStartup))
		{
			return;
		}
		lastLevelLoad = Time.realtimeSinceStartup;
		if (showDebug)
		{
		}
		int num = SoundConnectionsContainsThisLevel(SceneManager.GetActiveScene().name);
		if (num == -1 || soundConnections[num].isCustomLevel)
		{
			silentLevel = true;
		}
		else
		{
			silentLevel = false;
			currentLevel = SceneManager.GetActiveScene().name;
			currentSoundConnection = soundConnections[num];
		}
		if (!silentLevel && !offTheBGM)
		{
			if (showDebug)
			{
			}
			StopPreviousPlaySoundConnection();
			StartCoroutine("PlaySoundConnection", currentSoundConnection);
			return;
		}
		if (showDebug)
		{
		}
		currentSoundConnection = null;
		audios[0].loop = false;
		audios[1].loop = false;
		if (showDebug)
		{
		}
		currentPlaying = CheckWhosPlaying();
		StopAllCoroutines();
		if (currentPlaying == -1)
		{
			if (!showDebug)
			{
			}
		}
		else if (CheckWhosNotPlaying() == -1)
		{
			if (showDebug)
			{
			}
			StartCoroutine("CrossoutAll", crossDuration);
		}
		else if (audios[currentPlaying].isPlaying)
		{
			if (showDebug)
			{
			}
			StartCoroutine("Crossout", new object[2]
			{
				audios[currentPlaying],
				crossDuration
			});
		}
	}

	private void _PlayImmediately(AudioClip clip2play, bool loop, SongCallBack runOnEndFunction)
	{
		if (InternalCallback != null)
		{
			OnSongEnd = InternalCallback;
		}
		StopMusicImmediately();
		InternalCallback = runOnEndFunction;
		if (!offTheBGM && !isPaused)
		{
			SoundConnection sc = (!loop) ? new SoundConnection(string.Empty, PlayMethod.OncePlayThrough, clip2play) : new SoundConnection(string.Empty, PlayMethod.ContinuousPlayThrough, clip2play);
			ignoreCrossDuration = true;
			PlayConnection(sc);
		}
	}

	private void _PlayImmediately(AudioClip clip2play, bool loop)
	{
		_PlayImmediately(clip2play, loop, null);
	}

	private void _PlayImmediately(AudioClip clip2play)
	{
		_PlayImmediately(clip2play, loop: false);
	}

	private void _Play(AudioClip clip2play, bool loop, SongCallBack runOnEndFunction)
	{
		if (!offTheBGM && !isPaused)
		{
			if (InternalCallback != null)
			{
				OnSongEnd = InternalCallback;
			}
			InternalCallback = runOnEndFunction;
			SoundConnection sc = (!loop) ? new SoundConnection(SceneManager.GetActiveScene().name, PlayMethod.OncePlayThrough, clip2play) : new SoundConnection(SceneManager.GetActiveScene().name, PlayMethod.ContinuousPlayThrough, clip2play);
			PlayConnection(sc);
		}
	}

	private void _Play(AudioClip clip2play, bool loop)
	{
		_Play(clip2play, loop, null);
	}

	private void _Play(AudioClip clip2play)
	{
		_Play(clip2play, loop: false);
	}

	private void _StopMusicImmediately()
	{
		StopAllCoroutines();
		StartCoroutine("CrossoutAll", 0f);
	}

	private void _StopMusic()
	{
		StopAllCoroutines();
		StartCoroutine("CrossoutAll", crossDuration);
	}

	private void _Pause()
	{
		if (!isPaused)
		{
			isPaused = !isPaused;
			if (audios[0].isPlaying)
			{
				audiosPaused[0] = true;
			}
			audios[0].Pause();
			if (audios[1].isPlaying)
			{
				audiosPaused[1] = true;
			}
			audios[1].Pause();
			PSFX(pause: true);
		}
	}

	private void _UnPause()
	{
		if (isPaused)
		{
			if (audiosPaused[0])
			{
				audios[0].Play();
			}
			if (audiosPaused[1])
			{
				audios[1].Play();
			}
			audiosPaused[0] = (audiosPaused[1] = false);
			PSFX(pause: false);
			isPaused = !isPaused;
		}
	}

	private int PlayClip(AudioClip clip2play, float clipVolume = 1f)
	{
		if (showDebug)
		{
			
		}
		
		currentPlaying = CheckWhosPlaying();
		int num = CheckWhosNotPlaying();
		if (currentPlaying != -1)
		{
			if (num != -1)
			{
				if (audios[currentPlaying].clip.Equals(clip2play) && audios[currentPlaying].isPlaying)
				{
					if (showDebug)
					{
					}
					if (outCrossing[currentPlaying])
					{
						StopAllNonSoundConnectionCoroutines();
						if (showDebug)
						{
						}
						outCrossing[currentPlaying] = false;
						StartCoroutine("Crossin", new object[3]
						{
							audios[currentPlaying],
							crossDuration,
							clipVolume
						});
						return currentPlaying;
					}
					if (movingOnFromSong)
					{
						if (showDebug)
						{
						}
						if (audios[num] == null || audios[num].clip == null || !audios[num].clip.Equals(clip2play))
						{
							audios[num].clip = clip2play;
						}
						StartCoroutine("Crossfade", new object[4]
						{
							audios[currentPlaying],
							audios[num],
							crossDuration,
							clipVolume
						});
						return num;
					}
					return currentPlaying;
				}
				StopAllNonSoundConnectionCoroutines();
				if (showDebug)
				{
				}
				audios[num].clip = clip2play;
				StartCoroutine("Crossfade", new object[4]
				{
					audios[currentPlaying],
					audios[num],
					crossDuration,
					clipVolume
				});
				return num;
			}
			int actualCurrentPlayingIndex = GetActualCurrentPlayingIndex();
			if (showDebug)
			{
			}
			if (clip2play.Equals(audios[0].clip) && clip2play.Equals(audios[1].clip))
			{
				if (showDebug)
				{
				}
				int result = (actualCurrentPlayingIndex != 0) ? 1 : 0;
				if (!audios[0].isPlaying)
				{
					audios[0].Play();
				}
				if (!audios[1].isPlaying)
				{
					audios[1].Play();
				}
				return result;
			}
			if (clip2play.Equals(audios[0].clip))
			{
				bool flag = false;
				if (outCrossing[0] && (float)(audios[0].clip.samples - audios[0].timeSamples) * 1f / ((float)audios[0].clip.frequency * 1f) <= crossDuration)
				{
					if (showDebug)
					{
					}
					audios[1].clip = clip2play;
					audios[1].timeSamples = 0;
					flag = true;
				}
				else if (!showDebug)
				{
				}
				int num2 = (actualCurrentPlayingIndex != 0) ? 1 : 0;
				int num3 = (num2 == 0) ? 1 : 0;
				StopAllNonSoundConnectionCoroutines();
				if (flag)
				{
					int num4 = num2;
					num2 = num3;
					num3 = num4;
				}
				if (num2 != 0 || (flag && num2 != 1))
				{
					StartCoroutine("Crossfade", new object[4]
					{
						audios[num2],
						audios[num3],
						crossDuration,
						clipVolume
					});
				}
				else
				{
					StartCoroutine("Crossfade", new object[4]
					{
						audios[num3],
						audios[num2],
						crossDuration,
						clipVolume
					});
				}
				if (!audios[0].isPlaying)
				{
					audios[0].Play();
				}
				if (!audios[1].isPlaying)
				{
					audios[1].Play();
				}
				if (num2 != 0)
				{
					return num3;
				}
				return num2;
			}
			if (clip2play.Equals(audios[1].clip))
			{
				bool flag2 = false;
				if (outCrossing[1] && (float)(audios[1].clip.samples - audios[1].timeSamples) * 1f / ((float)audios[1].clip.frequency * 1f) <= crossDuration)
				{
					if (showDebug)
					{
					}
					audios[0].clip = clip2play;
					audios[0].timeSamples = 0;
					flag2 = true;
				}
				else if (!showDebug)
				{
				}
				int num5 = (actualCurrentPlayingIndex != 0) ? 1 : 0;
				int num6 = (num5 == 0) ? 1 : 0;
				StopAllNonSoundConnectionCoroutines();
				if (flag2)
				{
					int num7 = num5;
					num5 = num6;
					num6 = num7;
				}
				if (num5 != 1 || (flag2 && num5 != 0))
				{
					StartCoroutine("Crossfade", new object[4]
					{
						audios[num5],
						audios[num6],
						crossDuration,
						clipVolume
					});
				}
				else
				{
					StartCoroutine("Crossfade", new object[4]
					{
						audios[num6],
						audios[num5],
						crossDuration,
						clipVolume
					});
				}
				if (!audios[0].isPlaying)
				{
					audios[0].Play();
				}
				if (!audios[1].isPlaying)
				{
					audios[1].Play();
				}
				if (num5 != 1)
				{
					return num6;
				}
				return num5;
			}
			StopAllNonSoundConnectionCoroutines();
			if (showDebug)
			{
			}
			if (audios[0].volume > audios[1].volume)
			{
				audios[1].clip = clip2play;
				StartCoroutine("Crossfade", new object[4]
				{
					audios[0],
					audios[1],
					crossDuration,
					clipVolume
				});
				return 1;
			}
			audios[0].clip = clip2play;
			StartCoroutine("Crossfade", new object[4]
			{
				audios[1],
				audios[0],
				crossDuration,
				clipVolume
			});
			return 0;
		}
		if (audiosPaused[0] && audiosPaused[1])
		{
			if (showDebug)
			{
			}
			int num8 = (audios[0].volume > audios[1].volume) ? 1 : 0;
			int num9 = (num8 == 0) ? 1 : 0;
			audios[num8].clip = clip2play;
			StartCoroutine("Crossfade", new object[4]
			{
				audios[num9],
				audios[num8],
				crossDuration,
				clipVolume
			});
		}
		else if (audiosPaused[0])
		{
			if (showDebug)
			{
			}
			audios[1].clip = clip2play;
			audiosPaused[1] = true;
			StartCoroutine("Crossfade", new object[4]
			{
				audios[0],
				audios[1],
				crossDuration,
				clipVolume
			});
		}
		else if (audiosPaused[1])
		{
			if (showDebug)
			{
			}
			audios[0].clip = clip2play;
			audiosPaused[0] = true;
			StartCoroutine("Crossfade", new object[4]
			{
				audios[1],
				audios[0],
				crossDuration,
				clipVolume
			});
		}
		else
		{
			if (showDebug)
			{
			}
			audios[num].clip = clip2play;
			StartCoroutine("Crossin", new object[3]
			{
				audios[num],
				crossDuration,
				clipVolume
			});
		}
		return -1;
	}

	private IEnumerator Crossfade(object[] param)
	{
		if (OnCrossInBegin != null)
		{
			OnCrossInBegin();
		}
		OnCrossInBegin = null;
		if (OnCrossOutBegin != null)
		{
			OnCrossOutBegin();
		}
		OnCrossOutBegin = null;
		AudioSource a3 = param[0] as AudioSource;
		AudioSource a2 = param[1] as AudioSource;
		int index3 = GetAudioSourceIndex(a3);
		int index2 = GetAudioSourceIndex(a2);
		float duration = (float)param[2];
		if (ignoreCrossDuration)
		{
			ignoreCrossDuration = false;
			duration = 0f;
		}
		float realSongLength = 0f;
		if (a2.clip != null)
		{
			realSongLength = (float)(a2.clip.samples - a2.timeSamples) * 1f / ((float)a2.clip.frequency * 1f);
		}
		if (duration - realSongLength / 2f > 0.1f)
		{
			duration = Mathf.Floor(realSongLength / 2f * 100f) / 100f;
		}
		modifiedCrossDuration = duration;
		float clipVolume = (float)param[3];
		if (OnSongBegin != null)
		{
			OnSongBegin();
		}
		OnSongBegin = null;
		if (index3 == -1 || index2 == -1)
		{
		}
		outCrossing[index3] = true;
		inCrossing[index2] = true;
		outCrossing[index2] = false;
		inCrossing[index3] = false;
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + duration;
		if (!a2.isPlaying)
		{
			a2.Play();
		}
		float a1StartVolume = a3.volume;
		float a2StartVolume = a2.volume;
		float startMaxMusicVolume = maxMusicVolume;
		bool passedFirstPause = false;
		bool passedFirstUnpause = true;
		float pauseTimeRemaining = 0f;
		while (isPaused || passedFirstPause || Time.realtimeSinceStartup < endTime)
		{
			if (isPaused)
			{
				if (!passedFirstPause)
				{
					pauseTimeRemaining = endTime - Time.realtimeSinceStartup;
					passedFirstPause = true;
					passedFirstUnpause = false;
				}
				yield return new WaitForFixedUpdate();
				continue;
			}
			if (!passedFirstUnpause)
			{
				float num = endTime;
				endTime = Time.realtimeSinceStartup + pauseTimeRemaining;
				startTime += endTime - num;
				passedFirstPause = false;
				passedFirstUnpause = true;
			}
			float volumePercent = (startMaxMusicVolume != 0f) ? (maxMusicVolume / startMaxMusicVolume) : 1f;
			if (endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
				endTime = startTime + duration;
			}
			float deltaPercent = (Time.realtimeSinceStartup - startTime) / duration;
			float a1DeltaVolume = deltaPercent * a1StartVolume;
			float a2DeltaVolume = deltaPercent * (startMaxMusicVolume - a2StartVolume);
			a3.volume = Mathf.Clamp01((a1StartVolume - a1DeltaVolume) * volumePercent);
			a2.volume = Mathf.Clamp01((a2DeltaVolume + a2StartVolume) * volumePercent * clipVolume);
			yield return null;
		}
		a3.volume = 0f;
		a2.volume = maxMusicVolume * clipVolume;
		a3.Stop();
		a3.timeSamples = 0;
		modifiedCrossDuration = crossDuration;
		currentPlaying = CheckWhosPlaying();
		outCrossing[index3] = false;
		inCrossing[index2] = false;
		if (OnSongEnd != null)
		{
			OnSongEnd();
			if (InternalCallback != null)
			{
				OnSongEnd = InternalCallback;
			}
			else
			{
				OnSongEnd = null;
			}
			InternalCallback = null;
		}
		if (InternalCallback != null)
		{
			OnSongEnd = InternalCallback;
		}
		InternalCallback = null;
		if (OnSongBegin != null)
		{
			OnSongBegin();
		}
		OnSongBegin = null;
		SetNextSongInQueue();
	}

	private IEnumerator Crossout(object[] param)
	{
		if (OnCrossOutBegin != null)
		{
			OnCrossOutBegin();
		}
		OnCrossOutBegin = null;
		AudioSource a = param[0] as AudioSource;
		float duration = (float)param[1];
		if (ignoreCrossDuration)
		{
			ignoreCrossDuration = false;
			duration = 0f;
		}
		float realSongLength = 0f;
		if (a.clip != null)
		{
			realSongLength = (float)(a.clip.samples - a.timeSamples) * 1f / ((float)a.clip.frequency * 1f);
		}
		if (duration - realSongLength / 2f > 0.1f)
		{
			duration = Mathf.Floor(realSongLength / 2f * 100f) / 100f;
		}
		modifiedCrossDuration = duration;
		int index = GetAudioSourceIndex(a);
		if (index == -1)
		{
		}
		outCrossing[index] = true;
		inCrossing[index] = false;
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + duration;
		float maxVolume = a.volume;
		float startMaxMusicVolume = maxMusicVolume;
		bool passedFirstPause = false;
		bool passedFirstUnpause = true;
		float pauseTimeRemaining = 0f;
		while (isPaused || passedFirstPause || Time.realtimeSinceStartup < endTime)
		{
			if (isPaused)
			{
				if (!passedFirstPause)
				{
					pauseTimeRemaining = endTime - Time.realtimeSinceStartup;
					passedFirstPause = true;
					passedFirstUnpause = false;
				}
				yield return new WaitForFixedUpdate();
				continue;
			}
			if (!passedFirstUnpause)
			{
				float num = endTime;
				endTime = Time.realtimeSinceStartup + pauseTimeRemaining;
				startTime += endTime - num;
				passedFirstPause = false;
				passedFirstUnpause = true;
			}
			float volumePercent = (startMaxMusicVolume != 0f) ? (maxMusicVolume / startMaxMusicVolume) : 1f;
			if (endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
				endTime = startTime + duration;
			}
			float deltaVolume = (Time.realtimeSinceStartup - startTime) / duration * maxVolume;
			a.volume = Mathf.Clamp01((maxVolume - deltaVolume) * volumePercent);
			yield return null;
		}
		a.volume = 0f;
		a.Stop();
		a.timeSamples = 0;
		modifiedCrossDuration = crossDuration;
		currentPlaying = CheckWhosPlaying();
		outCrossing[index] = true;
		if (OnSongEnd != null)
		{
			OnSongEnd();
		}
		OnSongEnd = null;
		if (InternalCallback != null)
		{
			InternalCallback();
		}
		InternalCallback = null;
	}

	private IEnumerator CrossoutAll(float duration)
	{
		if (CheckWhosPlaying() == -1)
		{
			yield break;
		}
		if (OnCrossOutBegin != null)
		{
			OnCrossOutBegin();
		}
		OnCrossOutBegin = null;
		outCrossing[0] = true;
		outCrossing[1] = true;
		inCrossing[0] = false;
		inCrossing[1] = false;
		float realSongLength = 0f;
		if (audios[0].clip != null && audios[1].clip != null)
		{
			realSongLength = Mathf.Max((float)(audios[0].clip.samples - audios[0].timeSamples) * 1f / ((float)audios[0].clip.frequency * 1f), (float)(audios[1].clip.samples - audios[1].timeSamples) * 1f / ((float)audios[1].clip.frequency * 1f));
		}
		else if (audios[0].clip != null)
		{
			realSongLength = (float)(audios[0].clip.samples - audios[0].timeSamples) * 1f / ((float)audios[0].clip.frequency * 1f);
		}
		else if (audios[1].clip != null)
		{
			realSongLength = (float)(audios[1].clip.samples - audios[1].timeSamples) * 1f / ((float)audios[1].clip.frequency * 1f);
		}
		if (ignoreCrossDuration)
		{
			ignoreCrossDuration = false;
			duration = 0f;
		}
		if (duration - realSongLength / 2f > 0.1f)
		{
			duration = Mathf.Floor(realSongLength / 2f * 100f) / 100f;
		}
		modifiedCrossDuration = duration;
		float startTime = Time.realtimeSinceStartup;
		float endTime = Time.realtimeSinceStartup + duration;
		float a1MaxVolume = volume1;
		float a2MaxVolume = volume2;
		float startMaxMusicVolume = maxMusicVolume;
		bool passedFirstPause = false;
		bool passedFirstUnpause = true;
		float pauseTimeRemaining = 0f;
		while (isPaused || passedFirstPause || Time.realtimeSinceStartup < endTime)
		{
			if (isPaused)
			{
				if (!passedFirstPause)
				{
					pauseTimeRemaining = endTime - Time.realtimeSinceStartup;
					passedFirstPause = true;
					passedFirstUnpause = false;
				}
				yield return new WaitForFixedUpdate();
				continue;
			}
			if (!passedFirstUnpause)
			{
				float num = endTime;
				endTime = Time.realtimeSinceStartup + pauseTimeRemaining;
				startTime += endTime - num;
				passedFirstPause = false;
				passedFirstUnpause = true;
			}
			float volumePercent = (startMaxMusicVolume != 0f) ? (maxMusicVolume / startMaxMusicVolume) : 1f;
			if (endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
				endTime = startTime + duration;
			}
			float deltaPercent = (Time.realtimeSinceStartup - startTime) / duration;
			float a1DeltaVolume = deltaPercent * a1MaxVolume;
			float a2DeltaVolume = deltaPercent * a2MaxVolume;
			volume1 = Mathf.Clamp01((a1MaxVolume - a1DeltaVolume) * volumePercent);
			volume2 = Mathf.Clamp01((a2MaxVolume - a2DeltaVolume) * volumePercent);
			yield return null;
		}
		float num4 = volume1 = (volume2 = 0f);
		audios[0].Stop();
		audios[1].Stop();
		audios[0].timeSamples = 0;
		audios[1].timeSamples = 0;
		modifiedCrossDuration = crossDuration;
		currentPlaying = CheckWhosPlaying();
		outCrossing[0] = false;
		outCrossing[1] = false;
		if (OnSongEnd != null)
		{
			OnSongEnd();
		}
		OnSongEnd = null;
		if (InternalCallback != null)
		{
			InternalCallback();
		}
		InternalCallback = null;
	}

	private IEnumerator Crossin(object[] param)
	{
		if (OnCrossInBegin != null)
		{
			OnCrossInBegin();
		}
		OnCrossInBegin = null;
		AudioSource a = param[0] as AudioSource;
		float duration = (float)param[1];
		if (ignoreCrossDuration)
		{
			ignoreCrossDuration = false;
			duration = 0f;
		}
		float realSongLength = 0f;
		if (a.clip != null)
		{
			realSongLength = (float)a.clip.samples * 1f / ((float)a.clip.frequency * 1f);
		}
		if (duration - realSongLength / 2f > 0.1f)
		{
			duration = Mathf.Floor(realSongLength / 2f * 100f) / 100f;
		}
		modifiedCrossDuration = duration;
		float clipVolume = (float)param[2];
		int index = GetAudioSourceIndex(a);
		if (index == -1)
		{
		}
		inCrossing[index] = true;
		outCrossing[index] = false;
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + duration;
		float a1StartVolume = a.volume;
		float startMaxMusicVolume = maxMusicVolume;
		if (!a.isPlaying)
		{
			a1StartVolume = 0f;
			a.Play();
		}
		bool passedFirstPause = false;
		bool passedFirstUnpause = true;
		float pauseTimeRemaining = 0f;
		while (isPaused || passedFirstPause || Time.realtimeSinceStartup < endTime)
		{
			if (isPaused)
			{
				if (!passedFirstPause)
				{
					pauseTimeRemaining = endTime - Time.realtimeSinceStartup;
					passedFirstPause = true;
					passedFirstUnpause = false;
				}
				yield return new WaitForFixedUpdate();
				continue;
			}
			if (!passedFirstUnpause)
			{
				float num = endTime;
				endTime = Time.realtimeSinceStartup + pauseTimeRemaining;
				startTime += endTime - num;
				passedFirstPause = false;
				passedFirstUnpause = true;
			}
			float volumePercent = (startMaxMusicVolume != 0f) ? (maxMusicVolume / startMaxMusicVolume) : 1f;
			if (endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
				endTime = startTime + duration;
			}
			float deltaVolume = (Time.realtimeSinceStartup - startTime) / duration * (startMaxMusicVolume - a1StartVolume);
			a.volume = Mathf.Clamp01((deltaVolume + a1StartVolume) * volumePercent * clipVolume);
			yield return null;
		}
		a.volume = maxMusicVolume * clipVolume;
		modifiedCrossDuration = crossDuration;
		currentPlaying = CheckWhosPlaying();
		inCrossing[index] = false;
		if (OnSongBegin != null)
		{
			OnSongBegin();
		}
		OnSongBegin = null;
		if (InternalCallback != null)
		{
			OnSongEnd = InternalCallback;
		}
		InternalCallback = null;
		SetNextSongInQueue();
	}

	private AudioSource IsPlaying(AudioClip clip)
	{
		return IsPlaying(clip, regardlessOfCrossOut: true);
	}

	private AudioSource IsPlaying(AudioClip clip, bool regardlessOfCrossOut)
	{
		for (int i = 0; i < audios.Length; i++)
		{
			if (audios[i].isPlaying && audios[i].clip == clip && (regardlessOfCrossOut || !outCrossing[i]))
			{
				return audios[i];
			}
		}
		return null;
	}

	private bool IsPlaying()
	{
		for (int i = 0; i < audios.Length; i++)
		{
			if (audios[i].isPlaying)
			{
				return true;
			}
		}
		return false;
	}

	private int GetAudioSourceIndex(AudioSource source)
	{
		for (int i = 0; i < audios.Length; i++)
		{
			if (source == audios[i])
			{
				return i;
			}
		}
		return -1;
	}

	private void StopAllNonSoundConnectionCoroutines()
	{
		StopCoroutine("Crossfade");
		StopCoroutine("Crossout");
		StopCoroutine("Crossin");
		StopCoroutine("CrossoutAll");
	}

	private int GetActualCurrentPlayingIndex()
	{
		if (audios[0].isPlaying && audios[1].isPlaying)
		{
			if (inCrossing[0] && inCrossing[1])
			{
				return -1;
			}
			if (inCrossing[0])
			{
				return 0;
			}
			if (inCrossing[1])
			{
				return 1;
			}
			return -1;
		}
		for (int i = 0; i < audios.Length; i++)
		{
			if (audios[i].isPlaying)
			{
				return i;
			}
		}
		return -1;
	}

	private IEnumerator PlaySoundConnection(SoundConnection sc)
	{
		if (isPaused)
		{
			yield break;
		}
		currentSoundConnection = sc;
		if (sc.soundsToPlay.Count == 0)
		{
			StartCoroutine("CrossoutAll", crossDuration);
			yield break;
		}
		int songPlaying = 0;
		if (skipSongs)
		{
			songPlaying = (songPlaying + skipAmount) % sc.soundsToPlay.Count;
			if (songPlaying < 0)
			{
				songPlaying += sc.soundsToPlay.Count;
			}
			skipSongs = false;
			skipAmount = 0;
		}
		switch (sc.playMethod)
		{
		case PlayMethod.ContinuousPlayThrough:
			while (Application.isPlaying)
			{
				modifiedCrossDuration = crossDuration;
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying], sc.baseVolumes[songPlaying]);
				movingOnFromSong = false;
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], regardlessOfCrossOut: false);
				if (currentSource != null)
				{
					while ((ignoreFromLosingFocus || ((currentSource.isPlaying || isPaused || currentSource.mute) && (float)(currentSource.clip.samples - currentSource.timeSamples) * 1f / ((float)currentSource.clip.frequency * 1f) > modifiedCrossDuration)) && !skipSongs)
					{
						yield return null;
					}
				}
				movingOnFromSong = true;
				if (!skipSongs)
				{
					songPlaying = (songPlaying + 1) % sc.soundsToPlay.Count;
					continue;
				}
				songPlaying = (songPlaying + skipAmount) % sc.soundsToPlay.Count;
				if (songPlaying < 0)
				{
					songPlaying += sc.soundsToPlay.Count;
				}
				skipSongs = false;
				skipAmount = 0;
			}
			break;
		case PlayMethod.ContinuousPlayThroughWithDelay:
			while (Application.isPlaying)
			{
				modifiedCrossDuration = crossDuration;
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying], sc.baseVolumes[songPlaying]);
				movingOnFromSong = false;
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], regardlessOfCrossOut: false);
				if (currentSource != null)
				{
					while ((ignoreFromLosingFocus || currentSource.isPlaying || isPaused || currentSource.mute) && !skipSongs)
					{
						yield return null;
					}
				}
				movingOnFromSong = true;
				yield return new WaitForSeconds(sc.delay);
				if (!skipSongs)
				{
					songPlaying = (songPlaying + 1) % sc.soundsToPlay.Count;
					continue;
				}
				songPlaying = (songPlaying + skipAmount) % sc.soundsToPlay.Count;
				if (songPlaying < 0)
				{
					songPlaying += sc.soundsToPlay.Count;
				}
				skipSongs = false;
				skipAmount = 0;
			}
			break;
		case PlayMethod.ContinuousPlayThroughWithRandomDelayInRange:
			while (Application.isPlaying)
			{
				modifiedCrossDuration = crossDuration;
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying], sc.baseVolumes[songPlaying]);
				movingOnFromSong = false;
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], regardlessOfCrossOut: false);
				if (currentSource != null)
				{
					while ((ignoreFromLosingFocus || currentSource.isPlaying || isPaused || currentSource.mute) && !skipSongs)
					{
						yield return null;
					}
				}
				movingOnFromSong = true;
				float randomDelay = UnityEngine.Random.Range(sc.minDelay, sc.maxDelay);
				yield return new WaitForSeconds(randomDelay);
				if (!skipSongs)
				{
					songPlaying = (songPlaying + 1) % sc.soundsToPlay.Count;
					continue;
				}
				songPlaying = (songPlaying + skipAmount) % sc.soundsToPlay.Count;
				if (songPlaying < 0)
				{
					songPlaying += sc.soundsToPlay.Count;
				}
				skipSongs = false;
				skipAmount = 0;
			}
			break;
		case PlayMethod.OncePlayThrough:
			while (songPlaying < sc.soundsToPlay.Count)
			{
				modifiedCrossDuration = crossDuration;
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying], sc.baseVolumes[songPlaying]);
				movingOnFromSong = false;
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], regardlessOfCrossOut: false);
				if (currentSource != null)
				{
					while ((ignoreFromLosingFocus || ((currentSource.isPlaying || isPaused || currentSource.mute) && (float)(currentSource.clip.samples - currentSource.timeSamples) * 1f / ((float)currentSource.clip.frequency * 1f) > modifiedCrossDuration)) && !skipSongs)
					{
						yield return null;
					}
				}
				movingOnFromSong = true;
				if (!skipSongs)
				{
					songPlaying++;
				}
				else
				{
					songPlaying += skipAmount;
					if (songPlaying < 0)
					{
						songPlaying = 0;
					}
					skipSongs = false;
					skipAmount = 0;
				}
				if (sc.soundsToPlay.Count <= songPlaying)
				{
					StartCoroutine("CrossoutAll", crossDuration);
				}
			}
			break;
		case PlayMethod.OncePlayThroughWithDelay:
			while (songPlaying < sc.soundsToPlay.Count)
			{
				modifiedCrossDuration = crossDuration;
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying], sc.baseVolumes[songPlaying]);
				movingOnFromSong = false;
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], regardlessOfCrossOut: false);
				if (currentSource != null)
				{
					while ((ignoreFromLosingFocus || currentSource.isPlaying || isPaused || currentSource.mute) && !skipSongs)
					{
						yield return null;
					}
				}
				movingOnFromSong = true;
				yield return new WaitForSeconds(sc.delay);
				if (!skipSongs)
				{
					songPlaying++;
				}
				else
				{
					songPlaying += skipAmount;
					if (songPlaying < 0)
					{
						songPlaying = 0;
					}
					skipSongs = false;
					skipAmount = 0;
				}
				if (sc.soundsToPlay.Count <= songPlaying)
				{
					StartCoroutine("CrossoutAll", crossDuration);
				}
			}
			break;
		case PlayMethod.OncePlayThroughWithRandomDelayInRange:
			while (songPlaying < sc.soundsToPlay.Count)
			{
				modifiedCrossDuration = crossDuration;
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying], sc.baseVolumes[songPlaying]);
				movingOnFromSong = false;
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], regardlessOfCrossOut: false);
				if (currentSource != null)
				{
					while ((ignoreFromLosingFocus || currentSource.isPlaying || isPaused || currentSource.mute) && !skipSongs)
					{
						yield return null;
					}
				}
				movingOnFromSong = true;
				float randomDelay2 = UnityEngine.Random.Range(sc.minDelay, sc.maxDelay);
				yield return new WaitForSeconds(randomDelay2);
				if (!skipSongs)
				{
					songPlaying++;
				}
				else
				{
					songPlaying += skipAmount;
					if (songPlaying < 0)
					{
						songPlaying = 0;
					}
					skipSongs = false;
					skipAmount = 0;
				}
				if (sc.soundsToPlay.Count <= songPlaying)
				{
					StartCoroutine("CrossoutAll", crossDuration);
				}
			}
			break;
		case PlayMethod.ShufflePlayThrough:
			SoundManagerTools.ShuffleTwo(ref sc.soundsToPlay, ref sc.baseVolumes);
			while (Application.isPlaying)
			{
				modifiedCrossDuration = crossDuration;
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying], sc.baseVolumes[songPlaying]);
				movingOnFromSong = false;
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], regardlessOfCrossOut: false);
				if (currentSource != null)
				{
					while ((ignoreFromLosingFocus || ((currentSource.isPlaying || isPaused || currentSource.mute) && (float)(currentSource.clip.samples - currentSource.timeSamples) * 1f / ((float)currentSource.clip.frequency * 1f) > modifiedCrossDuration)) && !skipSongs)
					{
						yield return null;
					}
				}
				movingOnFromSong = true;
				if (!skipSongs)
				{
					songPlaying = (songPlaying + 1) % sc.soundsToPlay.Count;
				}
				else
				{
					songPlaying = (songPlaying + skipAmount) % sc.soundsToPlay.Count;
					if (songPlaying < 0)
					{
						songPlaying += sc.soundsToPlay.Count;
					}
					skipSongs = false;
					skipAmount = 0;
				}
				if (songPlaying == 0)
				{
					SoundManagerTools.ShuffleTwo(ref sc.soundsToPlay, ref sc.baseVolumes);
				}
			}
			break;
		case PlayMethod.ShufflePlayThroughWithDelay:
			SoundManagerTools.ShuffleTwo(ref sc.soundsToPlay, ref sc.baseVolumes);
			while (Application.isPlaying)
			{
				modifiedCrossDuration = crossDuration;
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying], sc.baseVolumes[songPlaying]);
				movingOnFromSong = false;
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], regardlessOfCrossOut: false);
				if (currentSource != null)
				{
					while ((ignoreFromLosingFocus || currentSource.isPlaying || isPaused || currentSource.mute) && !skipSongs)
					{
						yield return null;
					}
				}
				movingOnFromSong = true;
				yield return new WaitForSeconds(sc.delay);
				if (!skipSongs)
				{
					songPlaying = (songPlaying + 1) % sc.soundsToPlay.Count;
				}
				else
				{
					songPlaying = (songPlaying + skipAmount) % sc.soundsToPlay.Count;
					if (songPlaying < 0)
					{
						songPlaying += sc.soundsToPlay.Count;
					}
					skipSongs = false;
					skipAmount = 0;
				}
				if (songPlaying == 0)
				{
					SoundManagerTools.ShuffleTwo(ref sc.soundsToPlay, ref sc.baseVolumes);
				}
			}
			break;
		case PlayMethod.ShufflePlayThroughWithRandomDelayInRange:
			SoundManagerTools.ShuffleTwo(ref sc.soundsToPlay, ref sc.baseVolumes);
			while (Application.isPlaying)
			{
				modifiedCrossDuration = crossDuration;
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying], sc.baseVolumes[songPlaying]);
				movingOnFromSong = false;
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], regardlessOfCrossOut: false);
				if (currentSource != null)
				{
					while ((ignoreFromLosingFocus || currentSource.isPlaying || isPaused || currentSource.mute) && !skipSongs)
					{
						yield return null;
					}
				}
				movingOnFromSong = true;
				float randomDelay3 = UnityEngine.Random.Range(sc.minDelay, sc.maxDelay);
				yield return new WaitForSeconds(randomDelay3);
				if (!skipSongs)
				{
					songPlaying = (songPlaying + 1) % sc.soundsToPlay.Count;
				}
				else
				{
					songPlaying = (songPlaying + skipAmount) % sc.soundsToPlay.Count;
					if (songPlaying < 0)
					{
						songPlaying += sc.soundsToPlay.Count;
					}
					skipSongs = false;
					skipAmount = 0;
				}
				if (songPlaying == 0)
				{
					SoundManagerTools.ShuffleTwo(ref sc.soundsToPlay, ref sc.baseVolumes);
				}
			}
			break;
		}
	}

	private void SetNextSongInQueue()
	{
		if (currentSongIndex != -1 && currentSoundConnection != null && currentSoundConnection.soundsToPlay != null && currentSoundConnection.soundsToPlay.Count > 0)
		{
			int index = (currentSongIndex + 1) % currentSoundConnection.soundsToPlay.Count;
			AudioClip audioClip = currentSoundConnection.soundsToPlay[index];
			int num = CheckWhosNotPlaying();
			if (audios[num] == null || (audios[num].clip != null && !audios[num].clip.Equals(audioClip)))
			{
				audios[num].clip = audioClip;
			}
		}
	}

	public static void SetSFXCap(int cap)
	{
		Instance.capAmount = cap;
	}

	public static AudioSource PlaySFX(AudioClip clip, bool looping = false, float delay = 0f, float volume = float.MaxValue, float pitch = float.MaxValue, Vector3 location = default(Vector3), SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (clip == null)
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		return Instance.PlaySFXAt(clip, volume, pitch, location, capped: false, string.Empty, looping, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static AudioSource PlaySFX(string clipName, bool looping = false, float delay = 0f, float volume = float.MaxValue, float pitch = float.MaxValue, Vector3 location = default(Vector3), SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{		
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (!ClipNameIsValid(clipName))
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		
		return Instance.PlaySFXAt(Load(clipName), volume, pitch, location, capped: false, string.Empty, looping, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static AudioSource PlayCappedSFX(AudioClip clip, string cappedID, float volume = float.MaxValue, float pitch = float.MaxValue, Vector3 location = default(Vector3))
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (clip == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(cappedID))
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		if (!Instance.IsAtCapacity(cappedID, clip.name))
		{
			return Instance.PlaySFXAt(clip, volume, pitch, location, capped: true, cappedID);
		}
		return null;
	}

	public static AudioSource PlayCappedSFX(string clipName, string cappedID, float volume = float.MaxValue, float pitch = float.MaxValue, Vector3 location = default(Vector3))
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (!ClipNameIsValid(clipName))
		{
			return null;
		}
		if (string.IsNullOrEmpty(cappedID))
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		if (!Instance.IsAtCapacity(cappedID, clipName))
		{
			return Instance.PlaySFXAt(Load(clipName), volume, pitch, location, capped: true, cappedID);
		}
		return null;
	}

	public static AudioSource PlayCappedSFX(AudioSource aS, AudioClip clip, string cappedID, float volume = float.MaxValue, float pitch = float.MaxValue)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (clip == null || aS == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(cappedID))
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		if (!Instance.IsAtCapacity(cappedID, clip.name))
		{
			Instance.CheckInsertionIntoUnownedSFXObjects(aS);
			return Instance.PlaySFXOn(aS, clip, volume, pitch, capped: true, cappedID);
		}
		return null;
	}

	public static AudioSource PlayCappedSFX(AudioSource aS, string clipName, string cappedID, float volume = float.MaxValue, float pitch = float.MaxValue)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (!ClipNameIsValid(clipName) || aS == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(cappedID))
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		if (!Instance.IsAtCapacity(cappedID, clipName))
		{
			Instance.CheckInsertionIntoUnownedSFXObjects(aS);
			return Instance.PlaySFXOn(aS, Load(clipName), volume, pitch, capped: true, cappedID);
		}
		return null;
	}

	public static AudioSource PlaySFX(AudioSource aS, AudioClip clip, bool looping = false, float delay = 0f, float volume = float.MaxValue, float pitch = float.MaxValue, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (clip == null || aS == null)
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		Instance.CheckInsertionIntoUnownedSFXObjects(aS);
		return Instance.PlaySFXOn(aS, clip, volume, pitch, capped: false, string.Empty, looping, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static AudioSource PlaySFX(AudioSource aS, string clipName, bool looping = false, float delay = 0f, float volume = float.MaxValue, float pitch = float.MaxValue, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (!ClipNameIsValid(clipName) || aS == null)
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		Instance.CheckInsertionIntoUnownedSFXObjects(aS);
		return Instance.PlaySFXOn(aS, Load(clipName), volume, pitch, capped: false, string.Empty, looping, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static void StopSFXObject(AudioSource aS)
	{
		if (!(aS == null))
		{
			if (aS.isPlaying)
			{
				aS.Stop();
			}
			if (Instance.delayedAudioSources.ContainsKey(aS))
			{
				Instance.delayedAudioSources.Remove(aS);
			}
		}
	}

	public static AudioSource PlaySFX(GameObject gO, AudioClip clip, bool looping = false, float delay = 0f, float volume = float.MaxValue, float pitch = float.MaxValue, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (clip == null || gO == null)
		{
			return null;
		}
		if (gO.GetComponent<AudioSource>() == null)
		{
			gO.AddComponent<AudioSource>();
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		return PlaySFX(gO.GetComponent<AudioSource>(), clip, looping, delay, volume, pitch, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static AudioSource PlaySFX(GameObject gO, string clipName, bool looping = false, float delay = 0f, float volume = float.MaxValue, float pitch = float.MaxValue, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (!ClipNameIsValid(clipName) || gO == null)
		{
			return null;
		}
		if (gO.GetComponent<AudioSource>() == null)
		{
			gO.AddComponent<AudioSource>();
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		return PlaySFX(gO.GetComponent<AudioSource>(), Load(clipName), looping, delay, volume, pitch, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static void StopSFXObject(GameObject gO)
	{
		if (!(gO == null))
		{
			StopSFXObject(gO.GetComponent<AudioSource>());
		}
	}

	public static void StopSFX()
	{
		Instance._StopSFX();
	}

	public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip, bool tillDestroy = true, float volume = float.MaxValue, float pitch = float.MaxValue, float maxDuration = 0f, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (clip == null || aS == null)
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		Instance.CheckInsertionIntoUnownedSFXObjects(aS);
		return Instance.PlaySFXLoopOn(aS, clip, tillDestroy, volume, pitch, maxDuration, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static AudioSource PlaySFXLoop(AudioSource aS, string clipName, bool tillDestroy = true, float volume = float.MaxValue, float pitch = float.MaxValue, float maxDuration = 0f, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (!ClipNameIsValid(clipName) || aS == null)
		{
			return null;
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		Instance.CheckInsertionIntoUnownedSFXObjects(aS);
		return Instance.PlaySFXLoopOn(aS, Load(clipName), tillDestroy, volume, pitch, maxDuration, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip, bool tillDestroy = true, float volume = float.MaxValue, float pitch = float.MaxValue, float maxDuration = 0f, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (clip == null || gO == null)
		{
			return null;
		}
		if (gO.GetComponent<AudioSource>() == null)
		{
			gO.AddComponent<AudioSource>();
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		Instance.CheckInsertionIntoUnownedSFXObjects(gO.GetComponent<AudioSource>());
		return Instance.PlaySFXLoopOn(gO.GetComponent<AudioSource>(), clip, tillDestroy, volume, pitch, maxDuration, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static AudioSource PlaySFXLoop(GameObject gO, string clipName, bool tillDestroy = true, float volume = float.MaxValue, float pitch = float.MaxValue, float maxDuration = 0f, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		if (Instance.offTheSFX || Instance.isPaused)
		{
			return null;
		}
		if (!ClipNameIsValid(clipName) || gO == null)
		{
			return null;
		}
		if (gO.GetComponent<AudioSource>() == null)
		{
			gO.AddComponent<AudioSource>();
		}
		if (volume == float.MaxValue)
		{
			volume = Instance.volumeSFX;
		}
		if (pitch == float.MaxValue)
		{
			pitch = Instance.pitchSFX;
		}
		Instance.CheckInsertionIntoUnownedSFXObjects(gO.GetComponent<AudioSource>());
		return Instance.PlaySFXLoopOn(gO.GetComponent<AudioSource>(), Load(clipName), tillDestroy, volume, pitch, maxDuration, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	public static bool MuteSFX(bool toggle)
	{
		Instance.mutedSFX = toggle;
		return Instance.mutedSFX;
	}

	public static bool MuteSFX()
	{
		return MuteSFX(!Instance.mutedSFX);
	}

	public static bool IsSFXMuted()
	{
		return Instance.mutedSFX;
	}

	public static void SetVolumeSFX(float setVolume)
	{
		setVolume = Mathf.Clamp01(setVolume);
		float num = Instance.volumeSFX / Instance.maxSFXVolume;
		Instance.maxSFXVolume = setVolume * Instance.maxVolume;
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			num = 1f;
		}
		Instance.volumeSFX = Instance.maxSFXVolume * num;
	}

	public static void SetVolumeSFX(float setVolume, bool ignoreMaxSFXVolume, params AudioSource[] audioSources)
	{
		setVolume = Mathf.Clamp01(setVolume);
		float volume = (!ignoreMaxSFXVolume) ? (setVolume * Instance.maxSFXVolume) : setVolume;
		foreach (AudioSource audioSource in audioSources)
		{
			audioSource.volume = volume;
		}
	}

	public static void SetVolumeSFX(float setVolume, bool ignoreMaxSFXVolume, params GameObject[] sfxObjects)
	{
		setVolume = Mathf.Clamp01(setVolume);
		float volume = (!ignoreMaxSFXVolume) ? (setVolume * Instance.maxSFXVolume) : setVolume;
		foreach (GameObject gameObject in sfxObjects)
		{
			gameObject.GetComponent<AudioSource>().volume = volume;
		}
	}

	public static float GetVolumeSFX()
	{
		return Instance.maxSFXVolume;
	}

	public static void SetPitchSFX(float setPitch)
	{
		Instance.pitchSFX = setPitch;
	}

	public static void SetPitchSFX(float setPitch, params AudioSource[] audioSources)
	{
		foreach (AudioSource audioSource in audioSources)
		{
			audioSource.pitch = setPitch;
		}
	}

	public static void SetPitchSFX(float setPitch, params GameObject[] sfxObjects)
	{
		foreach (GameObject gameObject in sfxObjects)
		{
			gameObject.GetComponent<AudioSource>().pitch = setPitch;
		}
	}

	public static float GetPitchSFX()
	{
		return Instance.pitchSFX;
	}

	public static void SaveSFX(AudioClip clip, string grpName)
	{
		if (!(clip == null))
		{
			SFXGroup groupByGroupName = Instance.GetGroupByGroupName(grpName);
			if (groupByGroupName == null)
			{
			}
			SaveSFX(clip);
			Instance.AddClipToGroup(clip.name, grpName);
		}
	}

	public static void SaveSFX(AudioClip clip, SFXGroup grp)
	{
		if (clip == null)
		{
			return;
		}
		if (grp != null)
		{
			if (!Instance.groups.ContainsKey(grp.groupName))
			{
				Instance.groups.Add(grp.groupName, grp);
			}
			else if (Instance.groups[grp.groupName] == grp)
			{
			}
		}
		SaveSFX(clip);
		Instance.AddClipToGroup(clip.name, grp.groupName);
	}

	public static void SaveSFX(params AudioClip[] clips)
	{
		foreach (AudioClip audioClip in clips)
		{
			if (!(audioClip == null) && !Instance.allClips.ContainsKey(audioClip.name))
			{
				string name = audioClip.name;
				Instance.allClips.Add(name, audioClip);
				Instance.prepools.Add(name, 0);
				Instance.baseVolumes.Add(name, 1f);
				Instance.volumeVariations.Add(name, 0f);
				Instance.pitchVariations.Add(name, 0f);
			}
		}
	}

	public static void DeleteSFX()
	{
		Instance.allClips.Clear();
		Instance.prepools.Clear();
		Instance.baseVolumes.Clear();
		Instance.volumeVariations.Clear();
		Instance.pitchVariations.Clear();
		Instance.clipsInGroups.Clear();
		Instance.clipToGroupKeys.Clear();
		Instance.clipToGroupValues.Clear();
		foreach (SFXGroup sfxGroup in Instance.sfxGroups)
		{
			sfxGroup.clips.Clear();
		}
	}

	public static void DeleteSFX(params AudioClip[] clips)
	{
		foreach (AudioClip audioClip in clips)
		{
			if (!(audioClip == null) && !Instance.allClips.ContainsKey(audioClip.name))
			{
				string name = audioClip.name;
				Instance.allClips.Remove(name);
				Instance.prepools.Remove(name);
				Instance.baseVolumes.Remove(name);
				Instance.volumeVariations.Remove(name);
				Instance.pitchVariations.Remove(name);
				Instance.RemoveClipFromGroup(name);
			}
		}
	}

	public static void DeleteSFX(params string[] clipNames)
	{
		foreach (string text in clipNames)
		{
			if (!string.IsNullOrEmpty(text) && !Instance.allClips.ContainsKey(text))
			{
				AudioClip audioClip = Instance.allClips[text];
				Instance.allClips.Remove(text);
				Instance.prepools.Remove(text);
				Instance.baseVolumes.Remove(text);
				Instance.volumeVariations.Remove(text);
				Instance.pitchVariations.Remove(text);
				Instance.RemoveClipFromGroup(text);
			}
		}
	}

	public static void ApplySFXAttributes(AudioClip clip, int prepool, float baseVolume, float volumeVariation, float pitchVariation)
	{
		if (clip == null || !Instance.allClips.ContainsKey(clip.name) || Instance.allClips[clip.name] != clip)
		{
			return;
		}
		string name = clip.name;
		int num = Instance.prepools[name];
		Instance.prepools[name] = prepool;
		Instance.baseVolumes[name] = baseVolume;
		Instance.volumeVariations[name] = volumeVariation;
		Instance.pitchVariations[name] = pitchVariation;
		SFXPoolInfo sFXPoolInfo = null;
		if (Instance.ownedPools.ContainsKey(clip))
		{
			sFXPoolInfo = Instance.ownedPools[clip];
			if (sFXPoolInfo != null)
			{
				sFXPoolInfo.prepoolAmount = prepool;
				sFXPoolInfo.baseVolume = baseVolume;
				sFXPoolInfo.volumeVariation = volumeVariation;
				sFXPoolInfo.pitchVariation = pitchVariation;
			}
		}
		if (num < prepool)
		{
			Instance.PrePoolClip(clip, prepool - num);
		}
	}

	public static void ApplySFXAttributes(string clipName, int prepool, float baseVolume, float volumeVariation, float pitchVariation)
	{
		if (!string.IsNullOrEmpty(clipName) && ClipNameIsValid(clipName))
		{
			ApplySFXAttributes(Instance.allClips[clipName], prepool, baseVolume, volumeVariation, pitchVariation);
		}
	}

	public static SFXGroup CreateSFXGroup(string grpName, int capAmount)
	{
		if (!Instance.groups.ContainsKey(grpName))
		{
			SFXGroup sFXGroup = new SFXGroup(grpName, capAmount);
			Instance.groups.Add(grpName, sFXGroup);
			return sFXGroup;
		}
		return null;
	}

	public static SFXGroup CreateSFXGroup(string grpName)
	{
		if (!Instance.groups.ContainsKey(grpName))
		{
			SFXGroup sFXGroup = new SFXGroup(grpName);
			Instance.groups.Add(grpName, sFXGroup);
			return sFXGroup;
		}
		return null;
	}

	public static void MoveToSFXGroup(string clipName, string newGroupName)
	{
		Instance.SetClipToGroup(clipName, newGroupName);
	}

	public static void RemoveFromSFXGroup(string clipName)
	{
		Instance.RemoveClipFromGroup(clipName);
	}

	public static AudioClip LoadFromGroup(string grpName)
	{
		SFXGroup groupByGroupName = Instance.GetGroupByGroupName(grpName);
		if (groupByGroupName == null)
		{
			return null;
		}
		AudioClip audioClip = null;
		if (groupByGroupName.clips.Count == 0)
		{
			return null;
		}
		return groupByGroupName.clips[UnityEngine.Random.Range(0, groupByGroupName.clips.Count)];
	}

	public static AudioClip[] LoadAllFromGroup(string grpName)
	{
		SFXGroup groupByGroupName = Instance.GetGroupByGroupName(grpName);
		if (groupByGroupName == null)
		{
			return null;
		}
		if (groupByGroupName.clips.Count == 0)
		{
			return null;
		}
		return groupByGroupName.clips.ToArray();
	}

	public static AudioClip Load(string clipname, string customPath)
	{
		AudioClip audioClip = null;
		if (!string.IsNullOrEmpty(customPath))
		{
			audioClip = ((customPath[customPath.Length - 1] != '/') ? ((AudioClip)Resources.Load(customPath + "/" + clipname)) : ((AudioClip)Resources.Load(customPath.Substring(0, customPath.Length) + "/" + clipname)));
		}
		if ((bool)audioClip)
		{
			return audioClip;
		}
		if (Instance.allClips.ContainsKey(clipname))
		{
			audioClip = Instance.allClips[clipname];
		}
		if ((bool)audioClip)
		{
			return audioClip;
		}
		return (AudioClip)Resources.Load(Instance.resourcesPath + "/" + clipname);
	}

	public static AudioClip Load(string clipname)
	{
		return Load(clipname, string.Empty);
	}

	public static void ResetSFXObject(GameObject sfxObj)
	{
		AudioSource component = sfxObj.GetComponent<AudioSource>();
		if (!(component == null))
		{
			component.outputAudioMixerGroup = null;
			component.mute = false;
			component.bypassEffects = false;
			component.bypassListenerEffects = false;
			component.bypassReverbZones = false;
			component.playOnAwake = false;
			component.loop = false;
			component.priority = 128;
			component.volume = 1f;
			component.pitch = 1f;
			component.panStereo = 0f;
			component.reverbZoneMix = 1f;
			component.dopplerLevel = 1f;
			component.rolloffMode = AudioRolloffMode.Logarithmic;
			component.minDistance = 1f;
			component.spatialBlend = Instance.defaultSFXSpatialBlend;
			component.spread = 0f;
			component.maxDistance = 500f;
		}
	}

	public static void Crossfade(float duration, AudioSource fromSource, AudioSource toSource, SongCallBack runOnEndFunction = null)
	{
		Instance.StartCoroutine(Instance.XFade(duration, fromSource, toSource, runOnEndFunction));
	}

	public static void CrossIn(float duration, AudioSource source, SongCallBack runOnEndFunction = null)
	{
		Instance.StartCoroutine(Instance.XIn(duration, source, runOnEndFunction));
	}

	public static void CrossIn(float duration, GameObject sfxObject, SongCallBack runOnEndFunction = null)
	{
		CrossIn(duration, sfxObject.GetComponent<AudioSource>(), runOnEndFunction);
	}

	public static void CrossOut(float duration, AudioSource source, SongCallBack runOnEndFunction = null)
	{
		Instance.StartCoroutine(Instance.XOut(duration, source, runOnEndFunction));
	}

	public static void CrossOut(float duration, GameObject sfxObject, SongCallBack runOnEndFunction = null)
	{
		CrossOut(duration, sfxObject.GetComponent<AudioSource>(), runOnEndFunction);
	}

	private void SetupSoundFX()
	{
		SetupDictionary();
		foreach (KeyValuePair<AudioClip, SFXPoolInfo> ownedPool in ownedPools)
		{
			ownedPool.Value.ownedAudioClipPool.Clear();
		}
		ownedPools.Clear();
		unOwnedSFXObjects.Clear();
		cappedSFXObjects.Clear();
		foreach (KeyValuePair<string, AudioClip> allClip in allClips)
		{
			PrePoolClip(allClip.Value, prepools[allClip.Key]);
		}
	}

	private void PrePoolClip(AudioClip clip, int prepoolAmount)
	{
		for (int i = 0; i < prepoolAmount; i++)
		{
			AddOwnedSFXObject(clip);
		}
	}

	private void RemoveSFXObject(SFXPoolInfo info, int index)
	{
		GameObject obj = info.ownedAudioClipPool[index];
		info.ownedAudioClipPool.RemoveAt(index);
		info.timesOfDeath.RemoveAt(index);
		if (info.currentIndexInPool >= index)
		{
			info.currentIndexInPool = 0;
		}
		UnityEngine.Object.Destroy(obj);
	}

	private void SetupDictionary()
	{
		allClips.Clear();
		prepools.Clear();
		baseVolumes.Clear();
		volumeVariations.Clear();
		pitchVariations.Clear();
		for (int i = 0; i < storedSFXs.Count; i++)
		{
			if (!(storedSFXs[i] == null))
			{
				string name = storedSFXs[i].name;
				allClips.Add(name, storedSFXs[i]);
				prepools.Add(name, sfxPrePoolAmounts[i]);
				baseVolumes.Add(name, sfxBaseVolumes[i]);
				volumeVariations.Add(name, sfxVolumeVariations[i]);
				pitchVariations.Add(name, sfxPitchVariations[i]);
			}
		}
		storedSFXs.Clear();
		if (clipToGroupKeys.Count != clipToGroupValues.Count)
		{
			if (clipToGroupKeys.Count > clipToGroupValues.Count)
			{
				clipToGroupKeys.RemoveRange(clipToGroupValues.Count, clipToGroupKeys.Count - clipToGroupValues.Count);
			}
			else if (clipToGroupValues.Count > clipToGroupKeys.Count)
			{
				clipToGroupValues.RemoveRange(clipToGroupKeys.Count, clipToGroupValues.Count - clipToGroupKeys.Count);
			}
		}
		clipsInGroups.Clear();
		groups.Clear();
		for (int j = 0; j < clipToGroupValues.Count; j++)
		{
			if (ClipNameIsValid(clipToGroupKeys[j]))
			{
				clipsInGroups.Add(clipToGroupKeys[j], clipToGroupValues[j]);
				if (!groups.ContainsKey(clipToGroupValues[j]))
				{
					groups.Add(clipToGroupValues[j], new SFXGroup(clipToGroupValues[j], Load(clipToGroupKeys[j])));
				}
				else if (groups[clipToGroupValues[j]] == null)
				{
					groups[clipToGroupValues[j]] = new SFXGroup(clipToGroupValues[j], Load(clipToGroupKeys[j]));
				}
				else
				{
					groups[clipToGroupValues[j]].clips.Add(Load(clipToGroupKeys[j]));
				}
			}
		}
		foreach (SFXGroup sfxGroup in sfxGroups)
		{
			if (sfxGroup != null && groups.ContainsKey(sfxGroup.groupName))
			{
				groups[sfxGroup.groupName].specificCapAmount = sfxGroup.specificCapAmount;
			}
		}
		sfxGroups.Clear();
	}

	private void AddClipToGroup(string clipName, string groupName)
	{
		if (clipsInGroups.ContainsKey(clipName))
		{
			SetClipToGroup(clipName, groupName);
			return;
		}
		SFXGroup groupByGroupName = GetGroupByGroupName(groupName);
		if (groupByGroupName == null)
		{
			groups.Add(groupName, new SFXGroup(groupName, Load(clipName)));
		}
		else
		{
			groupByGroupName.clips.Add(Load(clipName));
		}
		clipsInGroups.Add(clipName, groupName);
	}

	private void SetClipToGroup(string clipName, string groupName)
	{
		SFXGroup groupForClipName = GetGroupForClipName(clipName);
		if (groupForClipName != null)
		{
			RemoveClipFromGroup(clipName);
		}
		AddClipToGroup(clipName, groupName);
	}

	private void RemoveClipFromGroup(string clipName)
	{
		SFXGroup groupForClipName = GetGroupForClipName(clipName);
		if (groupForClipName != null)
		{
			groupForClipName.clips.Remove(Load(clipName));
			clipsInGroups.Remove(clipName);
		}
	}

	private string GetClipToGroup(string clipName)
	{
		return clipsInGroups[clipName];
	}

	private void PSFX(bool pause)
	{
		foreach (KeyValuePair<AudioClip, SFXPoolInfo> ownedPool in ownedPools)
		{
			foreach (GameObject item in ownedPool.Value.ownedAudioClipPool)
			{
				if (item != null && item.activeSelf && item.GetComponent<AudioSource>() != null)
				{
					if (pause)
					{
						item.GetComponent<AudioSource>().Pause();
					}
					else
					{
						item.GetComponent<AudioSource>().Play();
					}
				}
			}
		}
		foreach (GameObject unOwnedSFXObject in unOwnedSFXObjects)
		{
			if (unOwnedSFXObject != null && unOwnedSFXObject.activeSelf && unOwnedSFXObject.GetComponent<AudioSource>() != null)
			{
				if (pause)
				{
					unOwnedSFXObject.GetComponent<AudioSource>().Pause();
				}
				else
				{
					unOwnedSFXObject.GetComponent<AudioSource>().Play();
				}
			}
		}
	}

	private void HandleSFX()
	{
		if (isPaused)
		{
			return;
		}
		foreach (KeyValuePair<AudioClip, SFXPoolInfo> ownedPool in ownedPools)
		{
			for (int i = 0; i < ownedPool.Value.ownedAudioClipPool.Count; i++)
			{
				if (ownedPool.Value.ownedAudioClipPool[i].activeSelf)
				{
					AudioSource component = ownedPool.Value.ownedAudioClipPool[i].GetComponent<AudioSource>();
					if (!component.isPlaying)
					{
						if (!delayedAudioSources.ContainsKey(component))
						{
							if (runOnEndFunctions.ContainsKey(component) && runOnEndFunctions[component] != null)
							{
								runOnEndFunctions[component]();
								runOnEndFunctions.Remove(component);
							}
							int instanceID = ownedPool.Value.ownedAudioClipPool[i].GetInstanceID();
							if (cappedSFXObjects.ContainsKey(instanceID))
							{
								cappedSFXObjects.Remove(instanceID);
							}
							ownedPool.Value.ownedAudioClipPool[i].SetActive(value: false);
							if (ownedPool.Value.prepoolAmount <= i)
							{
								ownedPool.Value.timesOfDeath[i] = Time.time + SFXObjectLifetime;
							}
						}
					}
					else if (delayedAudioSources.ContainsKey(component))
					{
						delayedAudioSources.Remove(component);
					}
				}
				else if (ownedPool.Value.prepoolAmount <= i && Time.time > ownedPool.Value.timesOfDeath[i])
				{
					RemoveSFXObject(ownedPool.Value, i);
				}
			}
		}
		for (int num = unOwnedSFXObjects.Count - 1; num >= 0; num--)
		{
			if (unOwnedSFXObjects[num] != null && unOwnedSFXObjects[num].GetComponent<AudioSource>() != null)
			{
				AudioSource component2 = unOwnedSFXObjects[num].GetComponent<AudioSource>();
				if (component2.isPlaying)
				{
					if (delayedAudioSources.ContainsKey(component2))
					{
						delayedAudioSources.Remove(component2);
					}
					continue;
				}
				if (delayedAudioSources.ContainsKey(component2))
				{
					continue;
				}
				if (runOnEndFunctions.ContainsKey(component2) && runOnEndFunctions[component2] != null)
				{
					runOnEndFunctions[component2]();
					runOnEndFunctions.Remove(component2);
				}
			}
			unOwnedSFXObjects.RemoveAt(num);
		}
	}

	private GameObject GetNextInactiveSFXObject(AudioClip clip)
	{
		if (!ownedPools.ContainsKey(clip) || ownedPools[clip].ownedAudioClipPool.Count == 0)
		{
			return AddOwnedSFXObject(clip);
		}
		SFXPoolInfo sFXPoolInfo = ownedPools[clip];
		for (int num = (sFXPoolInfo.currentIndexInPool + 1) % sFXPoolInfo.ownedAudioClipPool.Count; num != sFXPoolInfo.currentIndexInPool; num = (num + 1) % sFXPoolInfo.ownedAudioClipPool.Count)
		{
			if (!sFXPoolInfo.ownedAudioClipPool[num].activeSelf)
			{
				ownedPools[clip].currentIndexInPool = num;
				ResetSFXObject(sFXPoolInfo.ownedAudioClipPool[num]);
				return sFXPoolInfo.ownedAudioClipPool[num];
			}
		}
		return AddOwnedSFXObject(clip);
	}

	private GameObject AddOwnedSFXObject(AudioClip clip)
	{
		GameObject gameObject = new GameObject("SFX-[" + clip.name + "]", typeof(AudioSource));
		gameObject.transform.parent = base.transform;
		GameObject gameObject2 = gameObject;
		string name = gameObject2.name;
		gameObject2.name = name + "(" + gameObject.GetInstanceID() + ")";
		gameObject.GetComponent<AudioSource>().playOnAwake = false;
		if (ownedPools.ContainsKey(clip))
		{
			ownedPools[clip].ownedAudioClipPool.Add(gameObject);
			ownedPools[clip].timesOfDeath.Add(0f);
		}
		else
		{
			int minAmount = 0;
			float baseVol = 1f;
			float volVar = 0f;
			float pitchVar = 0f;
			string name2 = clip.name;
			if (allClips.ContainsKey(name2))
			{
				minAmount = prepools[name2];
				baseVol = baseVolumes[name2];
				volVar = volumeVariations[name2];
				pitchVar = pitchVariations[name2];
			}
			ownedPools.Add(clip, new SFXPoolInfo(0, minAmount, new List<float>
			{
				0f
			}, new List<GameObject>
			{
				gameObject
			}, baseVol, volVar, pitchVar));
		}
		ResetSFXObject(gameObject);
		gameObject.GetComponent<AudioSource>().clip = clip;
		return gameObject;
	}

	private AudioSource PlaySFXAt(AudioClip clip, float volume, float pitch, Vector3 location = default(Vector3), bool capped = false, string cappedID = "", bool looping = false, float delay = 0f, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		GameObject nextInactiveSFXObject = GetNextInactiveSFXObject(clip);
		
		if (nextInactiveSFXObject == null)
		{
			return null;
		}

		AudioSource component = nextInactiveSFXObject.GetComponent<AudioSource>();
		component.transform.position = location;
		component.gameObject.SetActive(value: true);
		return PlaySFXBase(component, clip, volume, pitch, capped, cappedID, looping, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	private AudioSource PlaySFXOn(AudioSource aSource, AudioClip clip, float volume, float pitch, bool capped = false, string cappedID = "", bool looping = false, float delay = 0f, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		aSource.clip = clip;
		return PlaySFXBase(aSource, clip, volume, pitch, capped, cappedID, looping, delay, runOnEndFunction, duckingSetting, duckVolume, duckPitch);
	}

	private AudioSource PlaySFXBase(AudioSource aSource, AudioClip clip, float volume, float pitch, bool capped = false, string cappedID = "", bool looping = false, float delay = 0f, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		aSource.Stop();
		string name = clip.name;
		if (pitchVariations.ContainsKey(name))
		{
			aSource.pitch = pitch.Vary(pitchVariations[name]);
		}
		else
		{
			aSource.pitch = pitch;
		}
		if (baseVolumes.ContainsKey(name))
		{
			volume *= baseVolumes[name];
		}
		if (volumeVariations.ContainsKey(name))
		{
			aSource.volume = volume.VaryWithRestrictions(volumeVariations[name]);
		}
		else
		{
			aSource.volume = volume;
		}
		if (!capped)
		{
			aSource.loop = looping;
		}
		aSource.mute = mutedSFX;
		if (delay <= 0f)
		{
			aSource.Play();
		}
		else
		{
			if (!delayedAudioSources.ContainsKey(aSource))
			{
				delayedAudioSources.Add(aSource, delay);
			}
			else
			{
				delayedAudioSources[aSource] = delay;
			}
			aSource.PlayDelayed(delay);
		}
		if (runOnEndFunction != null)
		{
			if (runOnEndFunctions.ContainsKey(aSource))
			{
				runOnEndFunctions[aSource] = runOnEndFunction;
			}
			else
			{
				runOnEndFunctions.Add(aSource, runOnEndFunction);
			}
		}
		Duck(duckingSetting, duckVolume, duckPitch, volume, pitch, aSource);
		if (capped && !string.IsNullOrEmpty(cappedID))
		{
			cappedSFXObjects.Add(aSource.gameObject.GetInstanceID(), cappedID);
		}
		return aSource;
	}

	private AudioSource PlaySFXLoopOn(AudioSource source, AudioClip clip, bool tillDestroy, float volume, float pitch, float maxDuration, SongCallBack runOnEndFunction = null, SoundDuckingSetting duckingSetting = SoundDuckingSetting.DoNotDuck, float duckVolume = 0f, float duckPitch = 1f)
	{
		source.Stop();
		source.clip = clip;
		string name = clip.name;
		if (pitchVariations.ContainsKey(name))
		{
			source.pitch = pitch.Vary(pitchVariations[name]);
		}
		else
		{
			source.pitch = pitch;
		}
		if (baseVolumes.ContainsKey(name))
		{
			volume *= baseVolumes[name];
		}
		if (volumeVariations.ContainsKey(name))
		{
			source.volume = volume.VaryWithRestrictions(volumeVariations[name]);
		}
		else
		{
			source.volume = volume;
		}
		source.mute = Instance.mutedSFX;
		source.loop = true;
		source.Play();
		if (runOnEndFunction != null)
		{
			if (runOnEndFunctions.ContainsKey(source))
			{
				runOnEndFunctions[source] = runOnEndFunction;
			}
			else
			{
				runOnEndFunctions.Add(source, runOnEndFunction);
			}
		}
		Duck(duckingSetting, duckVolume, duckPitch, volume, pitch, source);
		Instance.StartCoroutine(Instance._PlaySFXLoopTillDestroy(source.gameObject, source, tillDestroy, maxDuration));
		return source;
	}

	private IEnumerator _PlaySFXLoopTillDestroy(GameObject gO, AudioSource source, bool tillDestroy, float maxDuration)
	{
		bool trackEndTime = false;
		float endTime = Time.time + maxDuration;
		if (!tillDestroy || maxDuration > 0f)
		{
			trackEndTime = true;
		}
		while (ShouldContinueLoop(gO, trackEndTime, endTime))
		{
			yield return null;
		}
		source.Stop();
	}

	private void CheckInsertionIntoUnownedSFXObjects(AudioSource aSource)
	{
		if (!IsOwnedSFXObject(aSource) && !unOwnedSFXObjects.Contains(aSource.gameObject))
		{
			Instance.unOwnedSFXObjects.Add(aSource.gameObject);
		}
	}

	private void _StopSFX()
	{
		foreach (KeyValuePair<AudioClip, SFXPoolInfo> ownedPool in ownedPools)
		{
			foreach (GameObject item in ownedPool.Value.ownedAudioClipPool)
			{
				if (item != null && item.activeSelf && item.GetComponent<AudioSource>() != null)
				{
					item.GetComponent<AudioSource>().Stop();
				}
			}
		}
		foreach (GameObject unOwnedSFXObject in unOwnedSFXObjects)
		{
			if (unOwnedSFXObject != null && unOwnedSFXObject.activeSelf && unOwnedSFXObject.GetComponent<AudioSource>() != null)
			{
				unOwnedSFXObject.GetComponent<AudioSource>().Stop();
			}
		}
		delayedAudioSources.Clear();
	}

	private bool ShouldContinueLoop(GameObject gO, bool trackEndTime, float endTime)
	{
		bool flag = gO != null && gO.activeSelf;
		if (trackEndTime)
		{
			flag = (flag && Time.time < endTime);
		}
		return flag;
	}

	private bool IsAtCapacity(string cappedID, string clipName)
	{
		int specificCapAmount = capAmount;
		SFXGroup groupForClipName = GetGroupForClipName(clipName);
		if (groupForClipName != null)
		{
			if (groupForClipName.specificCapAmount == 0)
			{
				return false;
			}
			if (groupForClipName.specificCapAmount != -1)
			{
				specificCapAmount = groupForClipName.specificCapAmount;
			}
		}
		if (!cappedSFXObjects.ContainsValue(cappedID))
		{
			return false;
		}
		int num = 0;
		foreach (string value in cappedSFXObjects.Values)
		{
			if (value == cappedID)
			{
				num++;
				if (num >= specificCapAmount)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsAtCapacity(string clipName)
	{
		int specificCapAmount = capAmount;
		SFXGroup groupForClipName = GetGroupForClipName(clipName);
		if (groupForClipName != null)
		{
			if (groupForClipName.specificCapAmount == 0)
			{
				return false;
			}
			if (groupForClipName.specificCapAmount != -1)
			{
				specificCapAmount = groupForClipName.specificCapAmount;
			}
		}
		if (!cappedSFXObjects.ContainsValue(groupForClipName.groupName))
		{
			return false;
		}
		int num = 0;
		foreach (string value in cappedSFXObjects.Values)
		{
			if (value == groupForClipName.groupName)
			{
				num++;
				if (num >= specificCapAmount)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsOwnedSFXObject(AudioSource aSource)
	{
		if (aSource.clip == null)
		{
			if (aSource.gameObject.name.StartsWith("SFX-["))
			{
				foreach (KeyValuePair<AudioClip, SFXPoolInfo> ownedPool in ownedPools)
				{
					foreach (GameObject item in ownedPool.Value.ownedAudioClipPool)
					{
						if (item == aSource.gameObject)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		if (ownedPools.ContainsKey(aSource.clip))
		{
			return ownedPools[aSource.clip].ownedAudioClipPool.Contains(aSource.gameObject);
		}
		return false;
	}

	private bool IsOwnedSFXObject(GameObject obj)
	{
		return IsOwnedSFXObject(obj.GetComponent<AudioSource>());
	}

	private SFXGroup GetGroupForClipName(string clipName)
	{
		if (!clipsInGroups.ContainsKey(clipName))
		{
			return null;
		}
		return groups[clipsInGroups[clipName]];
	}

	private SFXGroup GetGroupByGroupName(string grpName)
	{
		if (!groups.ContainsKey(grpName))
		{
			return null;
		}
		return groups[grpName];
	}

	public string GetGroupName(string clipName)
	{
		if (!clipsInGroups.ContainsKey(clipName))
		{
			return null;
		}
		return groups[clipsInGroups[clipName]].groupName;
	}

	private IEnumerator XFade(float duration, AudioSource a1, AudioSource a2, SongCallBack runOnEndFunction)
	{
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + duration;
		if (!a2.isPlaying)
		{
			a2.Play();
		}
		float a1StartVolume = a1.volume;
		float a2StartVolume = a2.volume;
		float startMaxMusicVolume = a2.volume;
		bool passedFirstPause = false;
		bool passedFirstUnpause = true;
		float pauseTimeRemaining = 0f;
		while (isPaused || passedFirstPause || Time.realtimeSinceStartup < endTime)
		{
			if (isPaused)
			{
				if (!passedFirstPause)
				{
					pauseTimeRemaining = endTime - Time.realtimeSinceStartup;
					passedFirstPause = true;
					passedFirstUnpause = false;
				}
				yield return new WaitForFixedUpdate();
				continue;
			}
			if (!passedFirstUnpause)
			{
				float num = endTime;
				endTime = Time.realtimeSinceStartup + pauseTimeRemaining;
				startTime += endTime - num;
				passedFirstPause = false;
				passedFirstUnpause = true;
			}
			float volumePercent = 1f;
			if (endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
				endTime = startTime + duration;
			}
			float deltaPercent = (Time.realtimeSinceStartup - startTime) / duration;
			float a1DeltaVolume = deltaPercent * a1StartVolume;
			float a2DeltaVolume = deltaPercent * (startMaxMusicVolume - a2StartVolume);
			a1.volume = Mathf.Clamp01((a1StartVolume - a1DeltaVolume) * volumePercent);
			a2.volume = Mathf.Clamp01((a2DeltaVolume + a2StartVolume) * volumePercent);
			yield return null;
		}
		a1.volume = 0f;
		a2.volume = a2StartVolume;
		a1.Stop();
		a1.timeSamples = 0;
		runOnEndFunction?.Invoke();
	}

	private IEnumerator XOut(float duration, AudioSource a1, SongCallBack runOnEndFunction)
	{
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + duration;
		float maxVolume = a1.volume;
		bool passedFirstPause = false;
		bool passedFirstUnpause = true;
		float pauseTimeRemaining = 0f;
		while (isPaused || passedFirstPause || Time.realtimeSinceStartup < endTime)
		{
			if (isPaused)
			{
				if (!passedFirstPause)
				{
					pauseTimeRemaining = endTime - Time.realtimeSinceStartup;
					passedFirstPause = true;
					passedFirstUnpause = false;
				}
				yield return new WaitForFixedUpdate();
				continue;
			}
			if (!passedFirstUnpause)
			{
				float num = endTime;
				endTime = Time.realtimeSinceStartup + pauseTimeRemaining;
				startTime += endTime - num;
				passedFirstPause = false;
				passedFirstUnpause = true;
			}
			float volumePercent = 1f;
			if (endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
				endTime = startTime + duration;
			}
			float deltaVolume = (Time.realtimeSinceStartup - startTime) / duration * maxVolume;
			a1.volume = Mathf.Clamp01((maxVolume - deltaVolume) * volumePercent);
			yield return null;
		}
		a1.volume = 0f;
		a1.Stop();
		a1.timeSamples = 0;
		runOnEndFunction?.Invoke();
	}

	private IEnumerator XIn(float duration, AudioSource a1, SongCallBack runOnEndFunction)
	{
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + duration;
		float a1StartVolume = 0f;
		float startMaxMusicVolume = a1.volume;
		a1.volume = a1StartVolume;
		if (!a1.isPlaying)
		{
			a1.Play();
		}
		bool passedFirstPause = false;
		bool passedFirstUnpause = true;
		float pauseTimeRemaining = 0f;
		while (isPaused || passedFirstPause || Time.realtimeSinceStartup < endTime)
		{
			if (isPaused)
			{
				if (!passedFirstPause)
				{
					pauseTimeRemaining = endTime - Time.realtimeSinceStartup;
					passedFirstPause = true;
					passedFirstUnpause = false;
				}
				yield return new WaitForFixedUpdate();
				continue;
			}
			if (!passedFirstUnpause)
			{
				float num = endTime;
				endTime = Time.realtimeSinceStartup + pauseTimeRemaining;
				startTime += endTime - num;
				passedFirstPause = false;
				passedFirstUnpause = true;
			}
			float volumePercent = 1f;
			if (endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
				endTime = startTime + duration;
			}
			float deltaVolume = (Time.realtimeSinceStartup - startTime) / duration * (startMaxMusicVolume - a1StartVolume);
			a1.volume = Mathf.Clamp01((deltaVolume + a1StartVolume) * volumePercent);
			yield return null;
		}
		a1.volume = startMaxMusicVolume;
		runOnEndFunction?.Invoke();
	}

	private void Duck(SoundDuckingSetting duckingSetting, float duckVolume, float duckPitch, float unDuckedVolume, float unDuckedPitch, params AudioSource[] exceptions)
	{
		if (exceptions.Length > 0 && duckingSetting != 0)
		{
			if (!isDucking)
			{
				isDucking = true;
				preDuckVolume = maxVolume;
				preDuckVolumeMusic = maxMusicVolume;
				preDuckVolumeSFX = maxSFXVolume;
				preDuckPitch = GetPitch();
				preDuckPitchMusic = GetPitchMusic();
				preDuckPitchSFX = GetPitchSFX();
			}
			else
			{
				duckNumber++;
			}
			duckSource = exceptions[0];
			StartCoroutine(DuckTheTrack(duckingSetting, duckVolume, duckPitch, unDuckedVolume, unDuckedPitch, exceptions));
		}
	}

	private IEnumerator DuckTheTrack(SoundDuckingSetting duckingSetting, float duckVolume, float duckPitch, float unDuckedVolume, float unDuckedPitch, AudioSource[] exceptions)
	{
		int thisNumber = duckNumber;
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + duckStartSpeed;
		float startMaxPitch;
		float duckStartVolume;
		float startMaxVolume;
		float duckStartPitch;
		switch (duckingSetting)
		{
		default:
			yield break;
		case SoundDuckingSetting.DuckAll:
		{
			float maxSFXVolume;
			startMaxVolume = (maxSFXVolume = maxVolume);
			duckStartVolume = maxSFXVolume;
			startMaxPitch = (maxSFXVolume = GetPitch());
			duckStartPitch = maxSFXVolume;
			break;
		}
		case SoundDuckingSetting.OnlyDuckMusic:
		{
			float maxSFXVolume;
			startMaxVolume = (maxSFXVolume = maxMusicVolume);
			duckStartVolume = maxSFXVolume;
			startMaxPitch = (maxSFXVolume = GetPitchMusic());
			duckStartPitch = maxSFXVolume;
			break;
		}
		case SoundDuckingSetting.OnlyDuckSFX:
		{
			float maxSFXVolume;
			startMaxVolume = (maxSFXVolume = this.maxSFXVolume);
			duckStartVolume = maxSFXVolume;
			startMaxPitch = (maxSFXVolume = GetPitchSFX());
			duckStartPitch = maxSFXVolume;
			break;
		}
		}
		float unDuckStartVolume = 0f;
		float unDuckStartPitch = 1f;
		while (Time.realtimeSinceStartup < endTime)
		{
			if (thisNumber != duckNumber)
			{
				yield break;
			}
			float maxSoundVolume;
			switch (duckingSetting)
			{
			default:
				yield break;
			case SoundDuckingSetting.DuckAll:
				maxSoundVolume = maxVolume;
				break;
			case SoundDuckingSetting.OnlyDuckMusic:
				maxSoundVolume = maxMusicVolume;
				break;
			case SoundDuckingSetting.OnlyDuckSFX:
				maxSoundVolume = this.maxSFXVolume;
				break;
			}
			float volumePercent = (startMaxVolume != 0f) ? (maxSoundVolume / startMaxVolume) : 1f;
			if (endTime - Time.realtimeSinceStartup > duckStartSpeed)
			{
				startTime = Time.realtimeSinceStartup;
				endTime = startTime + duckStartSpeed;
			}
			float deltaPercent = (Time.realtimeSinceStartup - startTime) / duckStartSpeed;
			float duckDeltaVolume = deltaPercent * (duckStartVolume - duckVolume);
			float unDuckDeltaVolume = deltaPercent * (startMaxVolume - unDuckStartVolume);
			float duckDeltaPitch = deltaPercent * (duckStartPitch - duckPitch);
			float unDuckDeltaPitch = deltaPercent * (startMaxPitch - unDuckStartPitch);
			switch (duckingSetting)
			{
			default:
				yield break;
			case SoundDuckingSetting.DuckAll:
				SetVolume(Mathf.Clamp01((duckStartVolume - duckDeltaVolume) * volumePercent));
				SetPitch(Mathf.Clamp01((duckStartPitch - duckDeltaPitch) * volumePercent));
				foreach (AudioSource audioSource2 in exceptions)
				{
					SetVolumeSFX(Mathf.Clamp01((unDuckDeltaVolume + unDuckStartVolume) * volumePercent), true, audioSource2);
					SetPitchSFX(Mathf.Clamp01((unDuckDeltaPitch + unDuckStartPitch) * volumePercent), audioSource2);
				}
				break;
			case SoundDuckingSetting.OnlyDuckMusic:
				SetVolumeMusic(Mathf.Clamp01((duckStartVolume - duckDeltaVolume) * volumePercent));
				SetPitchMusic(Mathf.Clamp01((duckStartPitch - duckDeltaPitch) * volumePercent));
				foreach (AudioSource audioSource3 in exceptions)
				{
					SetVolumeSFX(Mathf.Clamp01((unDuckDeltaVolume + unDuckStartVolume) * volumePercent), true, audioSource3);
					SetPitchSFX(Mathf.Clamp01((unDuckDeltaPitch + unDuckStartPitch) * volumePercent), audioSource3);
				}
				break;
			case SoundDuckingSetting.OnlyDuckSFX:
				SetVolumeSFX(Mathf.Clamp01((duckStartVolume - duckDeltaVolume) * volumePercent));
				SetPitchSFX(Mathf.Clamp01((duckStartPitch - duckDeltaPitch) * volumePercent));
				foreach (AudioSource audioSource in exceptions)
				{
					SetVolumeSFX(Mathf.Clamp01((unDuckDeltaVolume + unDuckStartVolume) * volumePercent), true, audioSource);
					SetPitchSFX(Mathf.Clamp01((unDuckDeltaPitch + unDuckStartPitch) * volumePercent), audioSource);
				}
				break;
			}
			yield return null;
		}
		switch (duckingSetting)
		{
		default:
			yield break;
		case SoundDuckingSetting.DuckAll:
			SetVolume(duckVolume);
			SetPitch(duckPitch);
			foreach (AudioSource audioSource5 in exceptions)
			{
				SetVolumeSFX(unDuckedVolume, true, audioSource5);
				SetPitchSFX(unDuckedPitch, audioSource5);
			}
			break;
		case SoundDuckingSetting.OnlyDuckMusic:
			SetVolumeMusic(duckVolume);
			SetPitchMusic(duckPitch);
			foreach (AudioSource audioSource6 in exceptions)
			{
				SetVolumeSFX(unDuckedVolume, true, audioSource6);
				SetPitchSFX(unDuckedPitch, audioSource6);
			}
			break;
		case SoundDuckingSetting.OnlyDuckSFX:
			SetVolumeSFX(duckVolume);
			SetPitchSFX(duckPitch);
			foreach (AudioSource audioSource4 in exceptions)
			{
				SetVolumeSFX(unDuckedVolume, true, audioSource4);
				SetPitchSFX(unDuckedPitch, audioSource4);
			}
			break;
		}
		while (exceptions[0].isPlaying)
		{
			if (thisNumber != duckNumber)
			{
				yield break;
			}
			yield return null;
		}
		startTime = Time.realtimeSinceStartup;
		endTime = startTime + duckEndSpeed;
		switch (duckingSetting)
		{
		default:
			yield break;
		case SoundDuckingSetting.DuckAll:
		{
			duckStartVolume = maxVolume;
			startMaxVolume = preDuckVolume;
			duckStartPitch = GetPitch();
			float preDuckPitch2 = preDuckPitch;
			break;
		}
		case SoundDuckingSetting.OnlyDuckMusic:
		{
			duckStartVolume = maxMusicVolume;
			startMaxVolume = preDuckVolumeMusic;
			duckStartPitch = GetPitchMusic();
			float preDuckPitchMusic2 = preDuckPitchMusic;
			break;
		}
		case SoundDuckingSetting.OnlyDuckSFX:
		{
			duckStartVolume = this.maxSFXVolume;
			startMaxVolume = preDuckVolumeSFX;
			duckStartPitch = GetPitchSFX();
			float preDuckPitchSFX2 = preDuckPitchSFX;
			break;
		}
		}
		while (Time.realtimeSinceStartup < endTime)
		{
			if (thisNumber != duckNumber)
			{
				yield break;
			}
			float maxSoundVolume2;
			switch (duckingSetting)
			{
			default:
				yield break;
			case SoundDuckingSetting.DuckAll:
				maxSoundVolume2 = preDuckVolume;
				break;
			case SoundDuckingSetting.OnlyDuckMusic:
				maxSoundVolume2 = preDuckVolumeMusic;
				break;
			case SoundDuckingSetting.OnlyDuckSFX:
				maxSoundVolume2 = preDuckVolumeSFX;
				break;
			}
			float volumePercent = (startMaxVolume != 0f) ? (maxSoundVolume2 / startMaxVolume) : 1f;
			if (endTime - Time.realtimeSinceStartup > duckEndSpeed)
			{
				startTime = Time.realtimeSinceStartup;
				endTime = startTime + duckEndSpeed;
			}
			float deltaPercent = (Time.realtimeSinceStartup - startTime) / duckEndSpeed * (startMaxVolume - duckStartVolume);
			switch (duckingSetting)
			{
			default:
				yield break;
			case SoundDuckingSetting.DuckAll:
				SetVolume(Mathf.Clamp01((deltaPercent + duckStartVolume) * volumePercent));
				SetPitch(Mathf.Clamp01((deltaPercent + duckStartPitch) * volumePercent));
				break;
			case SoundDuckingSetting.OnlyDuckMusic:
				SetVolumeMusic(Mathf.Clamp01((deltaPercent + duckStartVolume) * volumePercent));
				SetPitchMusic(Mathf.Clamp01((deltaPercent + duckStartPitch) * volumePercent));
				break;
			case SoundDuckingSetting.OnlyDuckSFX:
				SetVolumeSFX(Mathf.Clamp01((deltaPercent + duckStartVolume) * volumePercent));
				SetPitchSFX(Mathf.Clamp01((deltaPercent + duckStartPitch) * volumePercent));
				break;
			}
			yield return null;
		}
		switch (duckingSetting)
		{
		default:
			yield break;
		case SoundDuckingSetting.DuckAll:
			SetVolume(preDuckVolume);
			SetPitch(preDuckPitch);
			break;
		case SoundDuckingSetting.OnlyDuckMusic:
			SetVolumeMusic(preDuckVolumeMusic);
			SetPitchMusic(preDuckPitchMusic);
			break;
		case SoundDuckingSetting.OnlyDuckSFX:
			SetVolumeSFX(preDuckVolumeSFX);
			SetPitchSFX(preDuckPitchSFX);
			break;
		}
		if (thisNumber == duckNumber)
		{
			duckNumber = 0;
			if (isDucking)
			{
				isDucking = false;
			}
		}
	}
}
