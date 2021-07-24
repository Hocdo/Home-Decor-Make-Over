using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelUnlockView : MonoBehaviour
{
    public Image selectedImage;

    public Button unlockButton;

    public Text gemCostText;

    public int unlockGemCost;

    public string id;

    public Action<int> UnlockEvent;

    [NonSerialized] public int index;

    [NonSerialized] public bool isUnlocked;

    public void SetSelected(bool flag)
    {
        selectedImage.gameObject.SetActive(flag);
    }

    public void SetUnlocked(bool flag)
    {
        isUnlocked = flag;
        unlockButton.gameObject.SetActive(!flag);

        if (!flag)
        {
            gemCostText.text = unlockGemCost.ToString();
        }
    }

    public void UnlockButtonPressed()
    {
        if (PlayerData.current.gemCount >= unlockGemCost)
        {
            AudioManager.Instance.PlaySFX(AudioClipId.Purchased);

            PlayerData.current.AddGem(-unlockGemCost);
            SetUnlocked(true);

            var match3Data = PlayerData.current.match3Data;
            match3Data.jewelPackId = id;
            if (!match3Data.unlockedJewelPackIds.Contains(id))
            {
                match3Data.unlockedJewelPackIds.Add(id);
            }

            UnlockEvent?.Invoke(index);
        }
        else
        {
            Popup.PopupSystem.Instance.ShowPopup(PopupType.PopupGetMoreGems, Popup.CurrentPopupBehaviour.KeepShowing, true, true);
        }
    }
}
