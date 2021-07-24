using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LocalizationText : MonoBehaviour
{
    public string key;

    public bool autoDisplayOnAwake = true;

#if UNITY_EDITOR
    public LanguageType previewLanguage = LanguageType.ENGLISH;
#endif
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();

        if (autoDisplayOnAwake)
        {
            CreateValue();
        }

        CustomLocalization.UpdateLanguageEvent += OnUpdateLanguage;
    }

    private void OnDestroy()
    {
        CustomLocalization.UpdateLanguageEvent -= OnUpdateLanguage;
    }

#if UNITY_EDITOR
    public void CreateKey()
    {
        key = CustomLocalization.GetKey(GetText(), out previewLanguage);
    }
#endif

    public void CreateValue()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SetText(CustomLocalization.Get(key, previewLanguage));
            SetFont(CustomLocalization.GetFont(previewLanguage));
        }
        else
        {
            SetText(CustomLocalization.Get(key));
            SetFont(CustomLocalization.GetFont());
        }
#else
        string value = CustomLocalization.Get(key);
        if (value == null) value = key;
            SetText(value);

        SetFont(CustomLocalization.GetFont());
#endif
    }

    public void OnUpdateLanguage(LanguageType language)
    {
        CreateValue();
    }

    public void ApplyTextWithKey(string key)
    {
        if (text == null)
        {
            text = GetComponent<Text>();
        }

        this.key = key;
        CreateValue();
    }


    public void ApplyTextColorWithKey(string key, string colorHex)
    {
        if (text == null)
        {
            text = GetComponent<Text>();
        }

        this.key = key;

        string value = CustomLocalization.Get(key);

        if (value != null)
        {
            value = value.Replace("(", "<color=#" + colorHex + ">");
            if (value != null)
                text.text = value.Replace(")", "</color>");
        }
    }

    public void SetText(string value)
    {
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(value))
        {
            if (!Application.isPlaying)
            {
                GetComponent<Text>().text = value;
            }
            else
            {
                GetComponent<Text>().text = value;
            }
        }
#else
        if (!string.IsNullOrEmpty(value))
            text.text = value;
#endif        
    }

    public void SetFont(Font font)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GetComponent<Text>().font = font;
        }
        else
        {
            GetComponent<Text>().font = font;
        }
#else
    text.font = font;
#endif        
    }

    private string GetText()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return GetComponent<Text>().text;
        }
        else
        {
            return text.text;
        }
#else
    return text.text;
#endif      
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LocalizationText))]
public class LocalizationTextEditor : Editor
{
    public LocalizationText textController;

    public void OnEnable()
    {
        textController = target as LocalizationText;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(25f);

        if (GUILayout.Button("Key -> Value"))
        {
            textController.CreateValue();
        }

        if (GUILayout.Button("Value -> Key"))
        {
            textController.CreateKey();
        }
    }
}
#endif
