using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadSceneUtility
{
    public static string HomeDesignSceneName = "HomeDesignScene";

    public static string Match3SceneName = "Match3Scene";

    public static void LoadScene(string sceneName, Action PreLoadAction = null, Action PostloadAction = null)
    {
        LoadSceneManager.Instance.LoadScene(sceneName, PreLoadAction, PostloadAction);
    }

    public static void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public static string CurrentSceneName { get { return LoadSceneManager.Instance.currentSceneName; } } 
}

public class LoadSceneManager : MonoBehaviour
{
    private static LoadSceneManager instance;

    [NonSerialized]
    public string currentSceneName;

    public static LoadSceneManager Instance
    {
        get
        {
            return instance;
        }
    }

    public LoadingScreenController loadingScreenController;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName, Action PreloadAction = null, Action PostLoadAction = null)
    {
        Debug.Log("Loading scene " + sceneName);
        currentSceneName = sceneName;
        loadingScreenController.StartAnimating(sceneName, PreloadAction, PostLoadAction);      
    }
}
