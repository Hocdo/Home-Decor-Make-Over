using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using IronSourceJSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class IronSourceDependenciesManager : EditorWindow
{
    private const string jsonURL = "http://ssa.public.s3.amazonaws.com/Ironsource-Integration-Manager/IronSourceSDKInfo.json";
    private const string ironSourceDownloadDir = "Assets/IronSource/Editor/";
    private const string sdk = "sdk";
    private const string errorMessage = "SDK and adapters data are not available right now. Try again soon.";
    private const int Width = 760;
    private const int Height = 760;
    private const string Android = "Android";
    private const string iOS = "iOS";
    private readonly SortedSet<providerInfo> providersSet = new SortedSet<providerInfo>(new ProviderInfoComparor());
    private providerInfo ironSourceProviderInfo;
    private UnityWebRequest downloadWebClient;
    private string messageData;
    private IronSourceEditorCoroutines mEditorCoroutines;

    private GUIStyle headerStyle;
    private GUIStyle textStyle;
    private GUIStyle boldTextStyle;
    private readonly GUILayoutOption buttonWidth = GUILayout.Width(90);

    public class providerInfo
    {
        public Status currentStatues;
        public string providerName;
        public string currentUnityVersion;
        public string latestUnityVersion;
        public string downloadURL;
        public string displayProviderName;
        public bool isNewProvider;
        public string fileName;
        public Dictionary<string, string> sdkVersionDic;

        public providerInfo()
        {
            isNewProvider = false;
            fileName = string.Empty;
            downloadURL = string.Empty;
            currentUnityVersion = "none";
            sdkVersionDic = new Dictionary<string, string>();
        }

        public enum Status
        {
            installed = 1,
            none = 2,
            updated = 3
        }

        public bool GetFromJson(string name, Dictionary<string, object> dic)
        {
            providerName = name;
            object obj;

            dic.TryGetValue("keyname", out obj);
            if (obj != null)
            {
                this.displayProviderName = obj as string;
            }
            else this.displayProviderName = providerName;

            dic.TryGetValue("isNewProvider", out obj);
            if (obj != null)
            {
                this.isNewProvider = bool.Parse(obj as string);
            }

            //Get Unity versions
            if (dic.TryGetValue("Unity", out obj))
            {
                Dictionary<string, object> remoteVersions = obj as Dictionary<string, object>;
                if (remoteVersions != null)
                {
                    if (remoteVersions.TryGetValue("DownloadUrl", out obj))
                    {
                        this.downloadURL = obj as string;
                    }
                    if (remoteVersions.TryGetValue("FileName", out obj))
                    {
                        this.fileName = obj as string;
                    }
                    if (remoteVersions.TryGetValue("UnityAdapterVersion", out obj))
                    {
                        this.latestUnityVersion = obj as string;
                    }
                }
            }
            ////Get Android version
            if (dic.TryGetValue(Android, out obj))
            {
                Dictionary<string, object> androidVersion = obj as Dictionary<string, object>;
                if (androidVersion != null)
                {
                    androidVersion.TryGetValue("version", out obj);
                    androidVersion = obj as Dictionary<string, object>;
                    if (androidVersion != null)
                    {
                        if (androidVersion.TryGetValue(sdk, out obj))
                        {
                            this.sdkVersionDic.Add(Android, obj as string);
                        }
                    }
                }
            }

            //Get iOS version
            dic.TryGetValue(iOS, out obj);
            Dictionary<string, object> iosVersion = obj as Dictionary<string, object>;
            if (iosVersion != null)
            {
                iosVersion.TryGetValue("version", out obj);
                iosVersion = obj as Dictionary<string, object>;
                if (iosVersion != null)
                {
                    if (iosVersion.TryGetValue(sdk, out obj))
                    {
                        this.sdkVersionDic.Add(iOS, obj as string);
                    }
                }
            }

            currentUnityVersion = GetVersionFromXML(fileName);

            if (currentUnityVersion.Equals("none"))
            {
                currentStatues = Status.none;
            }

            else
            {
                if (isNewerVersion(currentUnityVersion, latestUnityVersion))
                {
                    currentStatues = Status.installed;
                }
                else
                {
                    currentStatues = Status.updated;
                }
            }

            return true;
        }
    }

    private static string GetVersionFromXML(string fileName)
    {
        XmlDocument xmlDoc = new XmlDocument();
        string version = "none";
        try
        {
            xmlDoc.LoadXml(File.ReadAllText(ironSourceDownloadDir + fileName));
        }
        catch (Exception)
        {
            return version;
        }
        var unityVersion = xmlDoc.SelectSingleNode("dependencies/unityversion");
        if (unityVersion != null)
        {
            return (unityVersion.InnerText);
        }
        File.Delete(ironSourceDownloadDir + fileName);
        return version;
    }

    private IEnumerator GetVersions()
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(jsonURL);
        var webRequest = unityWebRequest.SendWebRequest();

        while (!webRequest.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (!unityWebRequest.isHttpError && !unityWebRequest.isNetworkError)
        {
            string json = unityWebRequest.downloadHandler.text;
            providersSet.Clear();
            ironSourceProviderInfo = new providerInfo();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                dic = Json.Deserialize(json) as Dictionary<string, object>;
            }

            catch (Exception e)
            {
                Debug.Log("Error getting response " + e.ToString());
            }
            if (dic != null && dic.Count != 0)
            {
                object providersJson;
                if (dic.TryGetValue("SDKSInfo", out providersJson))
                {
                    if (providersJson != null)
                    {
                        foreach (var item in providersJson as Dictionary<string, object>)
                        {
                            providerInfo info = new providerInfo();
                            if (info.GetFromJson(item.Key, item.Value as Dictionary<string, object>))
                            {
                                if (item.Key.ToLower().Contains("ironsource"))
                                {
                                    ironSourceProviderInfo = info;
                                }
                                else
                                {
                                    providersSet.Add(info);
                                }
                            }
                        }
                    }
                }

                if (ironSourceProviderInfo.currentStatues == providerInfo.Status.installed || ironSourceProviderInfo.currentStatues == providerInfo.Status.none)
                {
                    if (dic.TryGetValue("UpdateMessage", out providersJson))
                    {
                        messageData = providersJson.ToString();
                    }
                }
                else
                {
                    if (dic.TryGetValue("LatestMessage", out providersJson))
                    {
                        messageData = providersJson.ToString();
                    }
                }
            }
        }

        Repaint();
    }

    private void CancelDownload()
    {
        // if downloader object is still active
        if (downloadWebClient != null)
        {
            downloadWebClient.Abort();
            return;
        }

        if (mEditorCoroutines != null)
        {
            mEditorCoroutines.StopEditorCoroutine();
            mEditorCoroutines = null;
        }

        downloadWebClient = null;
    }

    public static void ShowISDependenciesManager()
    {
        var win = GetWindowWithRect<IronSourceDependenciesManager>(new Rect(0, 0, Width, Height), true);
        win.titleContent = new GUIContent("ironSource Integration Manager");
        win.Focus();
    }

    void Awake()
    {
        headerStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            fixedHeight = 20,
            stretchWidth = true,
            fixedWidth = Width / 4 + 5,
            clipping = TextClipping.Overflow,
            alignment = TextAnchor.MiddleLeft
        };
        textStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleLeft

        };
        boldTextStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold
        };
        CancelDownload();
    }

    private void OnEnable()
    {
        mEditorCoroutines = IronSourceEditorCoroutines.StartEditorCoroutine(GetVersions());
    }

    private void OnDestroy()
    {
        CancelDownload();
        AssetDatabase.Refresh();
    }

    void DrawProviderItem(providerInfo providerData)
    {
        if (!providerData.Equals(default(providerInfo)))
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                GUI.enabled = true;
                bool isNew = providerData.isNewProvider;
                string isNewAddition = isNew ? " - New Network" : string.Empty;
                string androidVersion = "";
                string iosVersion = "";
                string tooltipText = "Latest Version: \n " + providerData.providerName + " Adapter Version " + providerData.latestUnityVersion;
                if (!providerData.sdkVersionDic.TryGetValue(Android, out androidVersion))
                {
                    androidVersion = "";
                }
                else tooltipText = tooltipText + "\n Android SDK version " + androidVersion;
                if (!providerData.sdkVersionDic.TryGetValue(iOS, out iosVersion))
                {
                    iosVersion = "";
                }
                else tooltipText = tooltipText + "\n iOS SDK version " + iosVersion;

                EditorGUILayout.LabelField(providerData.displayProviderName + isNewAddition, isNew ? boldTextStyle : textStyle);
                EditorGUILayout.LabelField(providerData.currentUnityVersion, textStyle);
                EditorGUILayout.LabelField(providerData.latestUnityVersion, textStyle);

                if (providerData.currentStatues == providerInfo.Status.none)
                {
                    bool btn = GUILayout.Button(new GUIContent
                    {
                        text = "Install",
                        tooltip = tooltipText
                    }, buttonWidth);
                    if (btn && downloadWebClient == null)
                    {
                        GUI.enabled = true;
                        IronSourceEditorCoroutines.StartEditorCoroutine(DownloadFile(providerData.downloadURL));
                    }

                }
                else if (providerData.currentStatues == providerInfo.Status.installed)
                {
                    var btn = GUILayout.Button(new GUIContent
                    {
                        text= "Update",
                        tooltip = tooltipText
                    }
                    ,buttonWidth);
                    if (btn && downloadWebClient == null)
                    {
                        GUI.enabled = true;
                        IronSourceEditorCoroutines.StartEditorCoroutine(DownloadFile(providerData.downloadURL));
                    }
                }
                else
                {
                    GUI.enabled = false;
                    GUILayout.Button(new GUIContent
                    {
                        text = "Updated",
                        tooltip = tooltipText
                    }, buttonWidth);
                }
                GUILayout.Space(5);
                GUI.enabled = true;
            }
        }
    }

    void OnGUI()
    {
        if (ironSourceProviderInfo == null)
        {
            GUILayout.Label(errorMessage);
            return;
        }

        GUILayout.Space(10);
        using (new EditorGUILayout.VerticalScope("box"))
        {
            DrawSDKHeader();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            DrawProviderItem(ironSourceProviderInfo);
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        GUILayout.Space(15);
        DrawAdaptersHeader();
        GUILayout.Space(15);

        foreach (var provider in providersSet)
        {
            DrawProviderItem(provider);
            GUILayout.Space(2);
        }
        GUILayout.Space(30);
        if (!string.IsNullOrEmpty(messageData))
        {
            using (new EditorGUILayout.VerticalScope("box", GUILayout.ExpandHeight(true)))
            {
                GUILayout.Space(5);
                using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
                {
                    EditorGUILayout.SelectableLabel(messageData, EditorStyles.textField, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                }
                GUILayout.Space(5);
            }
            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(false)))
            {
                GUILayout.Space(15);
            }
        }

    }

    private void DrawSDKHeader()
    {
        using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
        {
            EditorGUILayout.LabelField("Current SDK Version", new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 13,
                fixedHeight = 20,
                stretchWidth = true,
                fixedWidth = Width / 4,
                clipping = TextClipping.Overflow,
                padding = new RectOffset(Width / 4 + 15, 0, 0, 0)
            });
            GUILayout.Space(85);
            EditorGUILayout.LabelField("Latest SDK Version", new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 13,
                fixedHeight = 20,
                stretchWidth = true,
                fixedWidth = Screen.width / 4,
                clipping = TextClipping.Overflow,
            });
        }
    }

    private void DrawAdaptersHeader()
    {
        using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
        {
            EditorGUILayout.LabelField("Network", headerStyle);
            EditorGUILayout.LabelField("Current Adapter Version", headerStyle);
            EditorGUILayout.LabelField("Latest Adapter Version", headerStyle);
            GUILayout.Space(30);
            EditorGUILayout.LabelField("Action", headerStyle);
        }
    }

    private IEnumerator DownloadFile(string downloadFileUrl)
    {
        int fileNameIndex = downloadFileUrl.LastIndexOf("/") + 1;
        string downloadFileName = downloadFileUrl.Substring(fileNameIndex);
        string fileDownloading = string.Format("Downloading {0}", downloadFileName);
        string genericFileName = Regex.Replace(downloadFileName, @"_v+(\d\.\d\.\d\.\d|\d\.\d\.\d)", "");
        string path = Path.Combine(ironSourceDownloadDir, genericFileName);
        bool isCancelled = false;
        downloadWebClient = new UnityWebRequest(downloadFileUrl);
        downloadWebClient.downloadHandler = new DownloadHandlerFile(path);
        downloadWebClient.SendWebRequest();
        if (!downloadWebClient.isHttpError && !downloadWebClient.isNetworkError)
        {
            while (!downloadWebClient.isDone)
            {
                yield return new WaitForSeconds(0.1f);
                if (EditorUtility.DisplayCancelableProgressBar("Download Manager", fileDownloading, downloadWebClient.downloadProgress))
                {
                    if (downloadWebClient.error != null)
                    {
                        Debug.LogError(downloadWebClient.error);
                    }
                    CancelDownload();
                    isCancelled = true;
                }
            }
        }
        else
        {
            Debug.LogError("Error Downloading " + genericFileName + " : " + downloadWebClient.error);
        }

        EditorUtility.ClearProgressBar();

        if (genericFileName.EndsWith(".unitypackage") && !isCancelled)
        {
            AssetDatabase.ImportPackage(Path.Combine(ironSourceDownloadDir, genericFileName), true);
        }
        else
        {
            // in case the download was cancelled, delete the file
            if(isCancelled && File.Exists(ironSourceDownloadDir + genericFileName))
            {
                File.Delete(ironSourceDownloadDir + genericFileName);
            }

            IronSourceEditorCoroutines.StartEditorCoroutine(GetVersions());
        }

        //clean the downloadWebClient object regardless of whether the request succeeded or failed 
        downloadWebClient.Dispose();
        downloadWebClient = null;

        IronSourceEditorCoroutines.StartEditorCoroutine(GetVersions());
    }

    private static bool isNewerVersion(string current, string latest)
    {
        bool isNewer = false;
        try
        {
            int[] currentVersion = Array.ConvertAll(current.Split('.'), int.Parse);
            int[] remoteVersion = Array.ConvertAll(latest.Split('.'), int.Parse);
            int remoteBuild = 0;
            int curBuild = 0;
            if (currentVersion.Length > 3)
            {
                curBuild = currentVersion[3];
            }
            if (remoteVersion.Length > 3)
            {
                remoteBuild = remoteVersion[3];

            }
            System.Version cur = new System.Version(currentVersion[0], currentVersion[1], currentVersion[2], curBuild);
            System.Version remote = new System.Version(remoteVersion[0], remoteVersion[1], remoteVersion[2], remoteBuild);
            isNewer = cur < remote;
        }
        catch (Exception)
        {

        }
        return isNewer;

    }

    internal class ProviderInfoComparor : IComparer<providerInfo>
    {
        public int Compare(providerInfo x, providerInfo y)
        {
            return x.providerName.CompareTo(y.providerName);
        }
    }
}
