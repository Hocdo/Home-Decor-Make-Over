using cookapps;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class LocalDataManager
{
    public static bool loadFromEncryptedFile = false;

    public static readonly string FILE_NAME_TABLE_SPEC_COIN = "SpecCoin.txt";

    public static readonly string FILE_NAME_TABLE_SPEC_ITEM = "SpecItem.txt";

    public static readonly string FILE_NAME_TABLE_OPTION = "Option.txt";

    public static readonly string FILE_NAME_TABLE_LEVEL_DATA = "LevelData.txt";

    public static readonly string FILE_NAME_TABLE_MAP_DATA = "MapData.txt";

    private const string C_FOLDER_DATA_ENCRYPT = "table";
    private const string C_FOLDER_DATA_DECRYPT = "table_decrypt";

    public static string GetFileContents(string fileName, bool onlyPersistentPath = false)
    {
        string text = string.Empty;
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            using (new MemoryStream())
            {
                try
                {
                    text = DataEncryption.DecryptString(File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    text = null;
                    File.Delete(path);
                    Debug.Log("JungleGame LocalDataManager.GetFileContents MemoryStream ex: " + ex.Message);
                }
            }
        }
        if (string.IsNullOrEmpty(text))
        {
            Debug.Log("JungleGame LocalDataManager.GetFileContents start fileName:" + fileName);
            if (onlyPersistentPath)
            {
                text = string.Empty;
            }
            try
            {
                if (loadFromEncryptedFile)
                    text = DataEncryption.DecryptString(Utils.GetResourcesStringFile(RemoveFileNameExtension(C_FOLDER_DATA_ENCRYPT + "/" + fileName)));
                else
                    text = Utils.GetResourcesStringFile(RemoveFileNameExtension(C_FOLDER_DATA_DECRYPT + "/" + fileName));

#if UNITY_EDITOR
                //System.IO.File.WriteAllText(Application.dataPath + "/Resources/" + C_FOLDER_DATA_DECRYPT + "/" + fileName, text);
                //UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (Exception ex)
            {
                Debug.Log("JungleGame LocalDataManager.GetFileContents ex: " + ex.Message);
            }
        }
        Debug.Log("JungleGame LocalDataManager.GetFileContents end fileName:" + fileName);
        return text;
    }

    public static void SaveFile(bool isResourcesFolder, string fileName, string jsonData)
    {
        try
        {
            if (!string.IsNullOrEmpty(jsonData) && !string.IsNullOrEmpty(fileName))
            {
                if (!Application.isEditor)
                {
                    isResourcesFolder = false;
                }

                string path = (!isResourcesFolder) ? Path.Combine(Application.persistentDataPath, fileName)
                    : (Application.dataPath + "/Resources/" + C_FOLDER_DATA_ENCRYPT + "/network/" + fileName);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.WriteAllText(path, DataEncryption.EncryptString(jsonData));
            }
        }
        catch (Exception ex)
        {
            Debug.Log("JungleGame LocalDataManager.SaveFile ex: " + ex.Message);
        }
    }

    private static string RemoveFileNameExtension(string fileName)
    {
        if (fileName.Length > 4)
        {
            return fileName.Substring(0, fileName.Length - 4);
        }
        return string.Empty;
    }
}