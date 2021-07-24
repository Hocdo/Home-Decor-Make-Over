using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Decor
{
    public class EditRoomUIView : MonoBehaviour
    {
        public Sprite unlockedAllSprite;

        public Sprite unlockedSprite;

        public GameObject iconButtonPrefab;

        public Transform iconButtonPackTransform;

        public Button exitButton;

        private RectTransform exitButtonRectTransform;

        private Vector2 exitButtonShowPosition;

        private Vector2 exitButtonHidePosition;

        private Button[] iconButtons;

        private bool canPressOut = true;

        private float iconButtonScale;

        private DesignItemView[] items;

        public Action<DesignItemView> IconClickEvent;

        public void Initialize(DesignItemView[] aitems)
        {
            exitButtonRectTransform = exitButton.GetComponent<RectTransform>();
            exitButtonShowPosition = exitButtonRectTransform.anchoredPosition;
            exitButtonHidePosition = new Vector2(exitButtonShowPosition.x, exitButtonRectTransform.rect.height * (-1.2f));
            exitButtonRectTransform.anchoredPosition = exitButtonHidePosition;
            exitButton.onClick.AddListener(() =>
            {
                if (canPressOut)
                {
                    AudioManager.Instance.PlaySFX(AudioClipId.PanelOut);

                    EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.HideEdit);

                    PopupUtility.ForceClosePopupLiteMessage();
                }
            });

            items = aitems;
            iconButtons = new Button[items.Length];
            for (int i = 0; i < iconButtons.Length; i++)
            {
                int tempIdx = i;
                var iconButtonGameobject = Instantiate(iconButtonPrefab, iconButtonPackTransform);

                Button iconButton = iconButtonGameobject.GetComponent<Button>();
                iconButton.onClick.AddListener(() =>
                {
                    OnItemClicked(tempIdx);
                });

                iconButtons[i] = iconButton;
            }

            iconButtonPrefab.SetActive(false);
        }

        void OnItemClicked(int idx)
        {
            var item = items[idx];
            if (item.primaryData.IsUnlocked())
            {
                AudioManager.Instance.PlaySFX(AudioClipId.ItemSelect);
                IconClickEvent?.Invoke(item);
            }
            else
            {
                PopupUtility.OpenPopupLiteMesage("Continue to decorate the room to unlock " + items[idx].primaryData.displayName);
                AudioManager.Instance.PlaySFX(AudioClipId.ClickFailed);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ShowAllIconButtons();

            canPressOut = false;
            exitButtonRectTransform.DOAnchorPos(exitButtonShowPosition, 0.25f).SetEase(Ease.OutSine).OnComplete(() => canPressOut = true);
        }

        public void Hide()
        {
            HideAllIconButtons();

            exitButtonRectTransform.DOAnchorPos(exitButtonHidePosition, 0.25f).SetEase(Ease.InSine).OnComplete(() => gameObject.SetActive(false));
        }

        public void OnApplyOrExitSelectVariant(object param)
        {
            ShowAllIconButtons();
        }

        public void ShowAllIconButtons()
        {
            for (int i = 0; i < iconButtons.Length; i++)
            {
                var item = items[i];
                var iconButton = iconButtons[i];
                iconButton.transform.position = item.GetIcon().transform.position;

                if (item.primaryData.IsUnlocked())
                {
                    if (item.primaryData.IsAllVariantUnlocked())
                    {
                        iconButton.GetComponent<Image>().sprite = unlockedAllSprite;
                    }
                    else
                    {
                        iconButton.GetComponent<Image>().sprite = unlockedSprite;
                    }
                    
                }
            }

            SetActiveAllIconButtons(true);
            iconButtonScale = 0.2f;
            canPressOut = false;
            DOTween.To(() => iconButtonScale, (value) => iconButtonScale = value, 1f, 0.2f).SetEase(Ease.OutBack).
                OnUpdate(() =>
                {
                    Vector3 scale = Vector3.one * iconButtonScale;

                    for (int i = 0; i < iconButtons.Length; i++)
                        iconButtons[i].transform.localScale = scale;
                }).
                OnComplete(() =>
                {
                    canPressOut = true;
                });
        }

        public void HideAllIconButtons()
        {
            iconButtonScale = 1f;
            canPressOut = false;
            DOTween.To(() => iconButtonScale, (value) => iconButtonScale = value, 0.2f, 0.16f).
                SetEase(Ease.InSine).
                OnUpdate(() =>
                {
                    Vector3 scale = Vector3.one * iconButtonScale;

                    for (int i = 0; i < iconButtons.Length; i++)
                        iconButtons[i].transform.localScale = scale;
                }).
                OnComplete(() =>
                {
                    SetActiveAllIconButtons(false);
                    canPressOut = true;
                });
        }

        void SetActiveAllIconButtons(bool flags)
        {
            for (int i = 0; i < iconButtons.Length; i++)
                iconButtons[i].gameObject.SetActive(flags);
        }
    }

}
