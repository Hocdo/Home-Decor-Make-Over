using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelInputField : MonoBehaviour
{
    public Text levelText;

    public void LoadLevel()
    {
        int.TryParse(levelText.text, out int level);
        
        if (level > 0 && level <= ServerDataTable.MAX_LEVEL)
        {
            //MonoSingleton<PlayerDataManager>.Instance.CurrentLevel = level;
            //MonoSingleton<PlayerDataManager>.Instance.lastPlayedLevel = level;
            SoundSFX.Play(SFXIndex.ButtonClick);
            //MonoSingleton<SceneControlManager>.Instance.LoadScene(SceneType.Game, SceneChangeEffect.Color);

            //MonoSingleton<PopupSystem>.Instance.Open(PopupType.PopupGameStart);        
        }
    }
}
