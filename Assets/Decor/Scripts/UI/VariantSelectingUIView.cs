using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Decor
{
    public class VariantSelectingUIView : MonoBehaviour
    {
        public delegate bool ApplyPredicate();

        public delegate void ApplyValidate(Action response);

        public PointerDownTrigger exitPanel;

        public Image selectedOutline;

        public Button applyButton;

        public Button exitButton;

        public Sprite selectedSprite;

        public Sprite normalSprite;

        public Sprite applySpriteNormal;

        public Sprite applySpriteDisable;

        public Sprite applySpriteAd;

        public Sprite applySpriteGem;

        public VariantElementUIView[] elements;

        public Action ShowEvent;

        public ApplyPredicate ApplyPredicateEvent;

        public ApplyValidate ApplyValidateEvent;

        public Action ExitEvent;

        public Action<int> ChangeEvent;

        private RectTransform rectTransform;

        private bool canExit = true;

        private float canvasWidth;

        private Currency[] variantCurrency;

        private bool predicateResult;

        private DesignItemData itemData;

        void Awake()
        {
            var canvasScale = transform.parent.parent.GetComponent<CanvasScaler>();

            float rw = Screen.width / canvasScale.referenceResolution.x;
            float rh = Screen.height / canvasScale.referenceResolution.y;
            float match = canvasScale.matchWidthOrHeight;
            float scale = Mathf.Pow(rw, 1f - match) * Mathf.Pow(rh, match);
            canvasWidth = Screen.width / scale;

            rectTransform = GetComponent<RectTransform>();

            for (int i = 0; i < elements.Length; i++)
            {
                int idx = i;
                elements[i].button.onClick.AddListener(() =>
                {
                    SelectVariant(idx);
                });
            }

            applyButton.onClick.AddListener(() =>
            {
                if (predicateResult)
                {
                    ApplyValidateEvent(() =>
                    {
                        EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.ApplyOrExitVariant);
                    });
                }              
            });
            exitButton.onClick.AddListener(() =>
            {
                ExitEvent?.Invoke();

                EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.ApplyOrExitVariant);
            });
        }

        public void Setup(DesignItemData aitemData, Sprite[] variantSprites, Currency[] currencies, Vector2 position, bool isItemUnlocked)
        {
            itemData = aitemData;

            if (variantCurrency == null || variantCurrency.Length != variantSprites.Length)
            {
                variantCurrency = new Currency[variantSprites.Length];
            }

            float offsetY = 15f;
            if (position.y > 0f)
                transform.position = new Vector3(position.x, position.y - offsetY, transform.position.z);
            else
                transform.position = new Vector3(position.x, position.y + offsetY, transform.position.z);

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].Setup(variantSprites[i], currencies[i]);
                if (currencies[i] != null)
                    variantCurrency[i] = currencies[i];
            }

            if (isItemUnlocked)
                exitPanel.GetComponent<PointerDownTrigger>().OnPointerDownAction = () => 
                {
                    if (canExit)
                    {
                        EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.ApplyOrExitVariant);
                        ExitEvent?.Invoke();                      
                    }                  
                };
            else
                exitPanel.GetComponent<PointerDownTrigger>().OnPointerDownAction = null;
        }

        public IEnumerator SelectVariantCoroutine(int index)
        {
            yield return null;

            SelectVariant(index);
        }

        void SelectVariant(int index)
        {

            index = Mathf.Clamp(index, 0, elements.Length - 1);

            selectedOutline.transform.position = elements[index].button.transform.position;

            ChangeEvent?.Invoke(index);

            predicateResult = ApplyPredicateEvent() == true;

            Sprite applySprite = null;
            if (predicateResult)
            {
                if (itemData.IsVariantUnlocked(index))
                {
                    applySprite = applySpriteNormal;
                }
                else
                {
                    Currency cost = itemData.variantCosts[itemData.variantIndex];

                    if (cost.type == CurrencyType.Ads)
                    {
                        applySprite = applySpriteAd;
                    }
                    else if (cost.type == CurrencyType.Gem)
                    {
                        applySprite = applySpriteNormal;
                    }
                    else
                    {
                        applySprite = applySpriteNormal;
                    }
                }            
            }
            else
            {
                applySprite = applySpriteDisable;
            }

            applyButton.GetComponent<Image>().sprite = applySprite;
        }

        public void Show(Action AnimationCompleteEvent = null)
        {
            canExit = false;
            gameObject.SetActive(true);
            exitPanel.gameObject.SetActive(true);
            transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            transform.DOKill();
            transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                canExit = true;
                AnimationCompleteEvent?.Invoke();      
            });

            ShowEvent?.Invoke();

            float offset = 5f;
            Vector2 rectPosition = rectTransform.localPosition;
            float hw = rectTransform.rect.width * 0.5f;
            if (rectPosition.x - hw < -canvasWidth * 0.5f)
                rectPosition.x = hw - canvasWidth * 0.5f + offset;
            else if (rectPosition.x + hw > canvasWidth * 0.5f)
                rectPosition.x = canvasWidth * 0.5f - hw - offset;
            rectTransform.localPosition = rectPosition;
        }

        public void Hide(bool animation, Action CompleteEvent = null)
        {
            if (canExit)
            {
                exitPanel.OnPointerDownAction = null;

                if (animation)
                {
                    transform.DOKill();
                    transform.DOScale(0.25f, 0.15f).SetEase(Ease.InSine).OnComplete(() =>
                    {
                        CompleteEvent?.Invoke();

                        gameObject.SetActive(false);
                        exitPanel.gameObject.SetActive(false);
                    });
                }
                else
                {
                    transform.localScale = Vector3.zero;
                    gameObject.SetActive(false);
                    exitPanel.gameObject.SetActive(false);
                }
            }
        }
    }

}
