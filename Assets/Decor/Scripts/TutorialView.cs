using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Decor
{
    public class TutorialView : MonoBehaviour
    {
        public Camera uiCamera;

        [Header("Base")]
        public Image blockingPanel;

        public SpriteRenderer panelRenderer;

        public Button triggerButton;

        public SpriteMask spriteMask;

        public float fadeTime = 0.25f;

        [Header("Customize")]
        public AnimationCurve baseTextAnimCur;

        public Transform handRoot;

        public RectTransform character;

        public Image textBaseImage;

        public LocalizationText tutorialText;

        private CanvasScaler canvasScaler;

        private Color panelColor;

        private float textBaseScale;

        private float scaleFromCameraToCanvas;

        private bool useHand;

        public void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            panelColor = panelRenderer.color;
            textBaseScale = textBaseImage.transform.localScale.x;

            scaleFromCameraToCanvas = UIUtility.GetCanvasSize(canvasScaler).y / (uiCamera.orthographicSize * 2f);
        }

        public void HighlightTarget(RectTransform target, Vector2 paddingCameraSpace = default(Vector2), Vector3 offset = default(Vector3))
        {
            triggerButton.GetComponent<Image>().enabled = true;           
            Vector3 targetPosition = target.transform.position + offset;
            spriteMask.transform.position = targetPosition;
            triggerButton.transform.position = new Vector3(targetPosition.x, targetPosition.y, triggerButton.transform.position.z);

            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);
            Vector2 worldSize = new Vector2(Vector3.Distance(corners[0], corners[3]), Vector3.Distance(corners[0], corners[1]));
            worldSize += paddingCameraSpace;

            Vector2 spriteSize = spriteMask.sprite.bounds.size;
            spriteMask.transform.localScale = new Vector3(worldSize.x / spriteSize.x, worldSize.y / spriteSize.y, 1f);

            SetupTriggerButton(worldSize);
        }

        public void HighlightTarget(SpriteRenderer spriteRenderer, Vector2 paddingCameraSpace = default(Vector2), Vector3 offset = default(Vector3))
        {
            triggerButton.GetComponent<Image>().enabled = true;
            //3.22 19.12
            Vector3 targetPosition = spriteRenderer.transform.position + offset;
            spriteMask.transform.position = targetPosition;
            triggerButton.transform.position = new Vector3(targetPosition.x, targetPosition.y, triggerButton.transform.position.z);

            Vector2 worldSize = spriteRenderer.bounds.size;
            worldSize += paddingCameraSpace;

            Vector2 spriteSize = spriteMask.sprite.bounds.size;
            spriteMask.transform.localScale = new Vector3(worldSize.x / spriteSize.x, worldSize.y / spriteSize.y, 1f);

            SetupTriggerButton(worldSize);
        }

        void SetupTriggerButton(Vector2 worldSize)
        {
            RectTransform triggerRectTransform = triggerButton.GetComponent<RectTransform>();
            triggerRectTransform.sizeDelta = worldSize * scaleFromCameraToCanvas + Vector2.one * 62f / 2.5f;
        }

        public void SetHand(Transform target, Vector2 offset = default(Vector2))
        {
            float angleRad = Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f);

            handRoot.position = target.position + new Vector3(offset.x, offset.y, 0f);

            useHand = true;
            handRoot.gameObject.SetActive(useHand);
        }

        public void SetCharacterPosition(Vector2 position)
        {
            character.anchoredPosition = position;
        }

        public void SetText(string atext)
        {
            tutorialText.ApplyTextWithKey(atext);
            ShowText();
        }

        public void SetTextColor(string text, string colorHex)
        {
            tutorialText.ApplyTextColorWithKey(text, colorHex);
            ShowText();
        }

        public void SetInvisibleActive()
        {
            triggerButton.GetComponent<Image>().enabled = false;

            gameObject.SetActive(true);
            character.gameObject.SetActive(false);
            textBaseImage.gameObject.SetActive(false);
            handRoot.gameObject.SetActive(false);
            panelRenderer.color = Color.clear;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            character.gameObject.SetActive(true);
            textBaseImage.gameObject.SetActive(true);

            panelRenderer.DOColor(panelColor, fadeTime);
        }

        public void Hide(bool active)
        {
            triggerButton.GetComponent<Image>().enabled = false;

            character.gameObject.SetActive(false);
            textBaseImage.gameObject.SetActive(false);
            handRoot.gameObject.SetActive(false);
            useHand = false;

            panelRenderer.DOColor(Color.clear, fadeTime).OnComplete(() =>
            {
                if (!active)
                {
                    gameObject.SetActive(false);
                }
            });
        }

        private void ShowText()
        {
            Transform textBaseTransform = textBaseImage.GetComponent<Transform>();
            textBaseTransform.localScale = textBaseScale * 0.8f * Vector3.one;
            textBaseTransform.DOKill();
            textBaseTransform.DOScale(textBaseScale, 0.2f).SetEase(baseTextAnimCur);
        }
    }
}

