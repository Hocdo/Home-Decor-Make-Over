using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Decor
{
    public delegate bool Predicate();
    public class SelectedItemInfoUIView : MonoBehaviour, IPointerClickHandler
    {
        public FlyingCoinsController flyingCoin;

        public PointerDownTrigger exitPanel;

        public Text costText;

        public Text nameText;

        public Image currencyImage;

        private RectTransform rectTransform;

        private CanvasGroup canvasGroup;

        private Vector2 iconPosition;

        private int cost;

        public Predicate UnlockCondition;

        public Action UnlockConfirmEvent;

        public Action<float, int> PlayDecreaseCoinEffectEvent;

        private bool isOpening, isClosing;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Setup(string name, int cost, Sprite currencySprite, Vector2 position)
        {
            this.cost = cost;
            nameText.text = name;
            costText.text = cost.ToString();
            currencyImage.sprite = currencySprite;

            iconPosition = position;
            float posZ = transform.position.z;
            float offsetY = 10f;
            if (position.y > 0f)
            {
                transform.position = new Vector3(position.x, position.y - offsetY, posZ);
            }
            else
            {
                transform.position = new Vector3(position.x, position.y + offsetY, posZ);
            }
        }

        public void Show(TweenCallback OnAnimationComplete = null)
        {
            isOpening = true;

            exitPanel.gameObject.SetActive(true);
            gameObject.SetActive(true);

            canvasGroup.DOKill();
            canvasGroup.alpha = 0.25f;
            canvasGroup.DOFade(1f, 0.2f);

            rectTransform.DOKill();
            rectTransform.localScale = Vector3.zero;
            rectTransform.DOScale(1f, 0.25f).SetEase(Ease.OutBack)
                .OnComplete(() => { isOpening = false; OnAnimationComplete?.Invoke(); });

            exitPanel.OnPointerDownAction = () => EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.ItemInfoClose);
        }

        public void Hide(bool deactivateExitPanel, Action OnAnimationComplete = null)
        {
            isClosing = true;

            exitPanel.OnPointerDownAction = null;
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, 0.25f).OnComplete(() =>
            {
                isClosing = false;

                if (deactivateExitPanel)
                    exitPanel.gameObject.SetActive(false);
                gameObject.SetActive(false);

                OnAnimationComplete?.Invoke();
            });

            rectTransform.DOKill();
            rectTransform.localScale = Vector3.one;
            rectTransform.DOScale(0.25f, 0.25f).SetEase(Ease.InQuad);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isOpening || isClosing) return;

            if (UnlockCondition == null || (UnlockCondition != null && UnlockCondition() == true))
            {
                EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.ItemInfoUnlockConfirmed);

                AudioManager.Instance.PlaySFX(AudioClipId.CoinFly);

                flyingCoin.StartWithTarget(iconPosition, out float duration, () => 
                    {
                        AudioManager.Instance.PlaySFX(AudioClipId.CoinFlyUnlock);

                        UnlockConfirmEvent?.Invoke();
                        EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.ShowVariant); 
                    });

                PlayDecreaseCoinEffectEvent?.Invoke(duration, -cost);
            }
        }
    }

}
