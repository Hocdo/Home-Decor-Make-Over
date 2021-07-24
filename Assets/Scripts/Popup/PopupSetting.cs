//#define CHEATING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Popup;
using DG.Tweening;

public class PopupSetting : PopupBase
{
    [Header("Music")]
    public Sprite musicEnabledSprite;

    public Sprite musicDisabledSprite;

    public Button musicButton;

    [Header("SFX")]
    public Sprite sfxEnabledSprite;

    public Sprite sfxDisabledSprite;

    public Button sfxButton;

    [Header("Quit")]
    public Button quitButton;

    public Button replayButton;

    [Header("Cheat")]
    public GameObject cheatObject;

    public InputField inputField;

    private bool musicEnabled = false;

    private bool sfxEnabled = false;

    private bool replayEnabled = false;

    public void ApplyCheating()
    {
        string cheatingInput = inputField.text;

        var ips = cheatingInput.Split('_');

        if (ips.Length == 2 && ips[0].Length > 0 && ips[1].Length > 0)
        {
            string amountString = ips[1];
            string targetString = ips[0];

            int.TryParse(amountString, out int amount);

            var playerData = PlayerData.current;

            if (targetString.Equals("coin"))
            {
                playerData.cointCount = Mathf.Max(0, playerData.cointCount + amount);
                EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinChange, playerData.cointCount);
            }
            else if (targetString.Equals("gem"))
            {
                playerData.gemCount = Mathf.Max(0, playerData.gemCount + amount);
                EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, playerData.gemCount);
            }
            else if (targetString.Equals("level"))
            {
                playerData.match3Data.level = Mathf.Clamp(amount, 0, 800);
                MapData.main = new MapData(playerData.match3Data.level);
            }
        }
    }

    public void OnEnable()
    {
//#if !CHEATING
//        cheatObject.SetActive(false);
//#endif

        musicEnabled = PlayerPrefs.GetInt("music_enabled", 1) == 1 ? true : false;
        sfxEnabled = PlayerPrefs.GetInt("sfx_enabled", 1) == 1 ? true : false;

        if (musicEnabled)
        {
            musicButton.GetComponent<Image>().sprite = musicEnabledSprite;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = musicDisabledSprite;
        }

        if (sfxEnabled)
        {
            sfxButton.GetComponent<Image>().sprite = sfxEnabledSprite;
        }
        else
        {
            sfxButton.GetComponent<Image>().sprite = sfxDisabledSprite;
        }

        if (LoadSceneUtility.CurrentSceneName.Equals(LoadSceneUtility.Match3SceneName))
        {
            replayButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(false);
            replayEnabled = true;
        }
        else
        {
            replayButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(true);
            replayEnabled = false;
        }    
    }

    public override void Show()
    {
        canClose = false;

        PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f);

        PopupAnimationUtility.AnimateScale(musicButton.transform, Ease.OutBack, 0.25f, 0.6f, 0.2f, 0.1f);
        PopupAnimationUtility.AnimateScale(sfxButton.transform, Ease.OutBack, 0.25f, 0.6f, 0.2f, 0.15f);
        PopupAnimationUtility.AnimateScale(replayEnabled ? replayButton.transform : quitButton.transform, Ease.OutBack, 0.25f, .6f, 0.2f, 0.2f).OnComplete(() => canClose = true);
    }  

    public override void Close(bool forceDestroying = true)
    {
        PopupAnimationUtility.AnimadeAlpha(GetComponent<CanvasGroup>(), Ease.Linear, 1f, 0f, 0.1f, 0f, false);
        PopupAnimationUtility.AnimateScale(transform, Ease.OutQuart, 1f, 0.8f, 0.1f, 0f)
            .OnComplete(() => TerminateInternal(forceDestroying));
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;

        if (musicEnabled)
        {
            musicButton.GetComponent<Image>().sprite = musicEnabledSprite;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = musicDisabledSprite;
        }

        PlayerPrefs.SetInt("music_enabled", musicEnabled == true ? 1 : 0);
        AudioSettingInterface.SetMusicEnabled(musicEnabled);
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;

        if (sfxEnabled)
        {
            sfxButton.GetComponent<Image>().sprite = sfxEnabledSprite;
        }
        else
        {
            sfxButton.GetComponent<Image>().sprite = sfxDisabledSprite;
        }

        PlayerPrefs.SetInt("sfx_enabled", sfxEnabled == true ? 1 : 0);
        AudioSettingInterface.SetSFXEnabled(sfxEnabled);
    }

    public void Replay()
    {       
        PopupSystem.Instance.ShowPopup(PopupType.PopupGiveup, CurrentPopupBehaviour.Close, true, true);

        //AppEventTracker.LogEventLevelStatus(PlayerData.current.match3Data.level,
        //Analytics.Feature_LEVEL_STATUS.ACTION_TYPE._break,
        //Analytics.Feature_LEVEL_STATUS.STATUS_PLAY._normal,
        //Analytics.Feature_LEVEL_STATUS.RESULT.NONE);
    }

    public void LanguageButtonPress()
    {
        PopupSystem.Instance.ShowPopup(PopupType.PopupLanguage, CurrentPopupBehaviour.KeepShowing, true, true);
    }
    public void ContactButtonPress()
    {
        Application.OpenURL("https://www.facebook.com/homedecorgame2021");
    }
    public void AboutButtonPress()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
