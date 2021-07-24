//@TODO ENABLE_ZLIB
//#define ENABLE_ZLIB

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_ZLIB
using Ionic.Zlib;
#endif

public class Utils
{
    public static readonly Side[] allSides = new Side[8]
    {
        Side.Top,
        Side.Bottom,
        Side.Right,
        Side.Left,
        Side.TopLeft,
        Side.TopRight,
        Side.BottomRight,
        Side.BottomLeft
    };

    public static readonly Side[] straightSides = new Side[4]
    {
        Side.Top,
        Side.Bottom,
        Side.Right,
        Side.Left
    };

    public static readonly Side[] slantedSides = new Side[4]
    {
        Side.TopLeft,
        Side.TopRight,
        Side.BottomRight,
        Side.BottomLeft
    };

    private static readonly StringBuilder tempStringBuilder = new StringBuilder();

    private static readonly string strCurrencyHeader = "{0:#,0}";

    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static DeviceOrientation deviceOrientationAtAutoRotationLocked = DeviceOrientation.Unknown;

    public static bool IsDeviceOrientationLock;

    public static ScreenOrientation ScreenOrientationLockedTimeValue = ScreenOrientation.Unknown;

    private static readonly List<RaycastResult> hitRayCastResult = new List<RaycastResult>();

    public static Side MirrorSide(Side s)
    {
        switch (s)
        {
            case Side.Bottom:
                return Side.Top;
            case Side.Top:
                return Side.Bottom;
            case Side.Left:
                return Side.Right;
            case Side.Right:
                return Side.Left;
            case Side.BottomLeft:
                return Side.TopRight;
            case Side.BottomRight:
                return Side.TopLeft;
            case Side.TopLeft:
                return Side.BottomRight;
            case Side.TopRight:
                return Side.BottomLeft;
            default:
                return Side.Null;
        }
    }

    public static int SideOffsetX(Side s)
    {
        switch (s)
        {
            case Side.Top:
            case Side.Bottom:
                return 0;
            case Side.Left:
            case Side.TopLeft:
            case Side.BottomLeft:
                return -1;
            case Side.Right:
            case Side.TopRight:
            case Side.BottomRight:
                return 1;
            default:
                return 0;
        }
    }

    public static int SideOffsetY(Side s)
    {
        switch (s)
        {
            case Side.Right:
            case Side.Left:
                return 0;
            case Side.Bottom:
            case Side.BottomRight:
            case Side.BottomLeft:
                return -1;
            case Side.Top:
            case Side.TopRight:
            case Side.TopLeft:
                return 1;
            default:
                return 0;
        }
    }

    public static Side SideHorizontal(Side s)
    {
        switch (s)
        {
            case Side.Left:
            case Side.TopLeft:
            case Side.BottomLeft:
                return Side.Left;
            case Side.Right:
            case Side.TopRight:
            case Side.BottomRight:
                return Side.Right;
            default:
                return Side.Null;
        }
    }

    public static Side SideVertical(Side s)
    {
        switch (s)
        {
            case Side.Top:
            case Side.TopRight:
            case Side.TopLeft:
                return Side.Top;
            case Side.Bottom:
            case Side.BottomRight:
            case Side.BottomLeft:
                return Side.Bottom;
            default:
                return Side.Null;
        }
    }

    public static bool IsDiagonalSide(Side s)
    {
        switch (s)
        {
            case Side.TopRight:
            case Side.TopLeft:
            case Side.BottomRight:
            case Side.BottomLeft:
                return true;
            default:
                return false;
        }
    }

    public static string StringReplaceAt(string value, int index, char newchar)
    {
        if (value.Length <= index)
        {
            return value;
        }
        StringBuilder stringBuilder = new StringBuilder(value);
        stringBuilder[index] = newchar;
        return stringBuilder.ToString();
    }

    public static IEnumerator WaitFor(Func<bool> Action, float delay)
    {
        float time = 0f;
        while (time <= delay)
        {
            time = ((!Action()) ? 0f : (time + Time.unscaledDeltaTime));
            yield return 0;
        }
    }

    public static RescueGingerManSize GetRescueGingerManSize(int w, int h)
    {
        RescueGingerManSize rescueGingerManSize = RescueGingerManSize.Null;
        try
        {
            return (RescueGingerManSize)Enum.Parse(typeof(RescueGingerManSize), $"Size{w}x{h}");
        }
        catch
        {
            return RescueGingerManSize.Null;
        }
    }

    public static int GetRescueGingerManSizeWidth(RescueGingerManSize size)
    {
        switch (size)
        {
            case RescueGingerManSize.Size1x2:
                return 1;
            case RescueGingerManSize.Size2x1:
            case RescueGingerManSize.Size2x4:
                return 2;
            case RescueGingerManSize.Size3x6:
                return 3;
            case RescueGingerManSize.Size4x2:
            case RescueGingerManSize.Size4x8:
                return 4;
            case RescueGingerManSize.Size6x3:
                return 6;
            case RescueGingerManSize.Size8x4:
                return 8;
            default:
                return 0;
        }
    }

    public static int GetRescueGingerManSizeHeight(RescueGingerManSize size)
    {
        switch (size)
        {
            case RescueGingerManSize.Size2x1:
                return 1;
            case RescueGingerManSize.Size1x2:
            case RescueGingerManSize.Size4x2:
                return 2;
            case RescueGingerManSize.Size6x3:
                return 3;
            case RescueGingerManSize.Size2x4:
            case RescueGingerManSize.Size8x4:
                return 4;
            case RescueGingerManSize.Size3x6:
                return 6;
            case RescueGingerManSize.Size4x8:
                return 8;
            default:
                return 0;
        }
    }

    public static void Shuffle<T>(IList<T> list)
    {
        if (list != null)
        {
            int num = list.Count;
            System.Random random = new System.Random();
            while (num > 1)
            {
                int index = random.Next(0, num) % num;
                num--;
                T value = list[index];
                list[index] = list[num];
                list[num] = value;
            }
        }
    }

    public static string GetCurrencyNumberString(int number)
    {
        tempStringBuilder.Length = 0;
        tempStringBuilder.AppendFormat(strCurrencyHeader, number);
        return tempStringBuilder.ToString();
    }

    public static string GetCurrencyNumberString(uint number)
    {
        tempStringBuilder.Length = 0;
        tempStringBuilder.AppendFormat(strCurrencyHeader, number);
        return tempStringBuilder.ToString();
    }

    public static string GetCurrencyNumberString(double number)
    {
        tempStringBuilder.Length = 0;
        tempStringBuilder.AppendFormat(strCurrencyHeader, number);
        return tempStringBuilder.ToString();
    }

    public static string GetResourcesStringFile(string fileName)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        if (textAsset == null)
        {
            return string.Empty;
        }
        return textAsset.text;
    }

    public static WWWForm JsonToWWWForm(string strInputJson)
    {
        WWWForm wWWForm = new WWWForm();
        Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(strInputJson);
        foreach (KeyValuePair<string, object> item in dictionary)
        {
            if (item.Value != null)
            {
                wWWForm.AddField(item.Key, item.Value.ToString());
            }
        }
        if (wWWForm.data.Length == 0)
        {
            wWWForm.AddField("this post is null", 0);
        }
        return wWWForm;
    }

    public static Dictionary<string, object> JsonToDict(string strInputJson)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, object>>(strInputJson);
    }

    public static void EnableAllSpriteRenderer(GameObject obj)
    {
        if ((bool)obj)
        {
            SpriteRenderer[] componentsInChildren = obj.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            foreach (SpriteRenderer spriteRenderer in componentsInChildren)
            {
                spriteRenderer.enabled = true;
            }
        }
    }

    public static void DisableAllSpriteRenderer(GameObject obj)
    {
        if ((bool)obj)
        {
            SpriteRenderer[] componentsInChildren = obj.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer spriteRenderer in componentsInChildren)
            {
                spriteRenderer.enabled = false;
            }
        }
    }

    public static void SetLayers(GameObject obj, int layer)
    {
        if ((bool)obj)
        {
            Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>();
            foreach (Transform transform in componentsInChildren)
            {
                transform.gameObject.layer = layer;
            }
        }
    }

    public static void PlayChildrenParticleSystem(GameObject obj)
    {
        if ((bool)obj)
        {
            ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particleSystem in componentsInChildren)
            {
                particleSystem.Play();
            }
        }
    }

    public static void RestartChildrenParticleSystem(GameObject obj)
    {
        if ((bool)obj)
        {
            ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particleSystem in componentsInChildren)
            {
                particleSystem.Clear();
                particleSystem.Simulate(0f, withChildren: true, restart: true);
                particleSystem.Play();
            }
        }
    }

    public static int ConvertToTimestamp(DateTime value)
    {
        return (int)(value - Epoch).TotalSeconds;
    }

    public static float ConvertToTimestampDouble(DateTime value)
    {
        return (float)(value - Epoch).TotalSeconds;
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        return Epoch.AddSeconds(unixTimeStamp).ToUniversalTime();
    }

    public static string GetTimeFormat(int time)
    {
        if (time < 3600)
        {
            return $"{time / 60:00}:{time % 60:00}";
        }
        if (time >= 86400)
        {
            return $"{time / 86400}d:{time / 3600 % 24:00}:{time / 60 % 60:00}:{time % 60:00}";
        }
        return $"{time / 3600:00}:{time / 60 % 60:00}:{time % 60:00}";
    }

    public static Vector3 Bezier(float t, Vector3 startPos, Vector3 pointPos, Vector3 TargetPos)
    {
        Vector3 a = Vector3.Lerp(startPos, pointPos, t);
        Vector3 b = Vector3.Lerp(pointPos, TargetPos, t);
        return Vector3.Lerp(a, b, t);
    }

    public static IEnumerator LookAtTarget(GameObject objThrowItem, float throwFlyingTime)
    {
        float elapse_time = 0f;
        Vector3 prePos = objThrowItem.transform.position;
        while (elapse_time < throwFlyingTime)
        {
            yield return null;
            elapse_time += Time.deltaTime;
            Vector3 curPos = objThrowItem.transform.position;
            Vector3 velocity = (curPos - prePos) / Time.deltaTime;
            Vector3 target = objThrowItem.transform.position + velocity;
            Vector3 relative = objThrowItem.transform.InverseTransformPoint(target);
            float angle = Mathf.Atan2(relative.x, relative.y) * 57.29578f;
            objThrowItem.transform.Rotate(0f, 0f, 0f - angle);
            prePos = objThrowItem.transform.position;
        }
    }

    public static bool IsUGUIHit(Vector3 _scrPos)
    {
        if (EventSystem.current == null)
        {
            return false;
        }
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        if (pointerEventData == null)
        {
            return false;
        }
        pointerEventData.position = _scrPos;
        hitRayCastResult.Clear();
        EventSystem.current.RaycastAll(pointerEventData, hitRayCastResult);
        int count = hitRayCastResult.Count;
        hitRayCastResult.Clear();
        return count > 1;
    }

    public static void StartVibrate()
    {
        Handheld.Vibrate();
    }

    public static void CompressDirectory(string sInDir, string sOutFile)
    {
#if ENABLE_ZLIB
        string[] files = Directory.GetFiles(sInDir, "*.vBugSlice", SearchOption.AllDirectories);
        int startIndex = (sInDir[sInDir.Length - 1] != Path.DirectorySeparatorChar) ? (sInDir.Length + 1) : sInDir.Length;
        using (FileStream stream = new FileStream(sOutFile, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Compress))
            {
                string[] array = files;
                foreach (string text in array)
                {
                    string sRelativePath = text.Substring(startIndex);
                    CompressFile(sInDir, sRelativePath, zipStream);
                }
            }
        }
#endif
    }

#if ENABLE_ZLIB
    public static void CompressFile(string sDir, string sRelativePath, GZipStream zipStream)
    {
        char[] array = sRelativePath.ToCharArray();
        zipStream.Write(BitConverter.GetBytes(array.Length), 0, 4);
        char[] array2 = array;
        foreach (char value in array2)
        {
            zipStream.Write(BitConverter.GetBytes(value), 0, 2);
        }
        byte[] array3 = File.ReadAllBytes(Path.Combine(sDir, sRelativePath));
        zipStream.Write(BitConverter.GetBytes(array3.Length), 0, 4);
        zipStream.Write(array3, 0, array3.Length);
    }

    public static bool DecompressFile(string sDir, GZipStream zipStream)
    {
        byte[] array = new byte[4];
        int num = zipStream.Read(array, 0, 4);
        if (num < 4)
        {
            return false;
        }
        int num2 = BitConverter.ToInt32(array, 0);
        array = new byte[2];
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < num2; i++)
        {
            zipStream.Read(array, 0, 2);
            char value = BitConverter.ToChar(array, 0);
            stringBuilder.Append(value);
        }
        string path = stringBuilder.ToString();
        array = new byte[4];
        zipStream.Read(array, 0, 4);
        int num3 = BitConverter.ToInt32(array, 0);
        array = new byte[num3];
        zipStream.Read(array, 0, array.Length);
        string path2 = Path.Combine(sDir, path);
        string directoryName = Path.GetDirectoryName(path2);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        using (FileStream fileStream = new FileStream(path2, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            fileStream.Write(array, 0, num3);
        }
        return true;
    }
#endif

    public static void DecompressToDirectory(string sCompressedFile, string sDir)
    {
#if ENABLE_ZLIB
        using (FileStream stream = new FileStream(sCompressedFile, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true))
            {
                while (DecompressFile(sDir, zipStream))
                {
                }
            }
        }
#endif
    }

    public static bool IsEqualOrHighlIOSVersion(string targetVersion)
    {
        string[] array = targetVersion.Split('.');
        int[] array2 = new int[3];
        int[] array3 = new int[3];
        for (int i = 0; i < array.Length && i < array2.Length; i++)
        {
            int.TryParse(array[i], out array2[i]);
        }
        string text = SystemInfo.operatingSystem.Replace("iPhone OS ", string.Empty);
        text = text.Replace("iOS ", string.Empty);
        string[] array4 = text.Split('.');
        for (int j = 0; j < array4.Length && j < array3.Length; j++)
        {
            int.TryParse(array4[j], out array3[j]);
        }
        if (array4.Length > 0)
        {
            for (int k = 0; k < array3.Length; k++)
            {
                if (array3[k] != array2[k])
                {
                    if (array3[k] > array2[k])
                    {
                        return true;
                    }
                    if (array3[k] < array2[k])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }
}