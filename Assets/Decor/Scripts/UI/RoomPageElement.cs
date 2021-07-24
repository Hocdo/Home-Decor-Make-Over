using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Decor
{
    public class RoomPageElement : MonoBehaviour
    {
        public Text unlockCountText;

        public Button button;

        public Image circleImage;

        public Image roomImage;

        public Action<int> PressAction;

        private int index;

        public Material graymaterial;

        public void SetIndex(int idx)
        {
            index = idx;
        }

        public void SetRoomSprite(Sprite roomSprite)
        {
            roomImage.sprite = roomSprite;
        }

        public void SetUnLocked(int unlockedCount, int totalCount, Sprite circleUnlockedSprite)
        {
            unlockCountText.transform.parent.gameObject.SetActive(true);
            unlockCountText.text = unlockedCount.ToString() + "/" + totalCount.ToString();
            circleImage.sprite = circleUnlockedSprite;
            roomImage.material = null;
            circleImage.material = null;
        }

        public void SetLocked()
        {
            unlockCountText.transform.parent.gameObject.SetActive(false);
            roomImage.material = graymaterial;
            circleImage.material = graymaterial;
        }

        public void OnPress()
        {
            PressAction?.Invoke(index);
        }
    }

}
