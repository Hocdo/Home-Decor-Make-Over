using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Match3
{
    public class TutorialView : MonoBehaviour
    {
        [Header("Mask")]
        public Camera maskCamera;

        public RawImage rawImage;

        public Canvas canvasMask;

        public GameObject slotSingleMaskPrefab;

        public GameObject slotCombinedMaskPrefab;

        public GameObject circleMaskPrefab;

        public GameObject imageMaskPrefab;

        [Header("Base")]
        public Image blockingPanel;

        public Button triggerButton;

        public Button skipButton;

        public bool showSkipButton;

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

        private Camera renderCamera;

        private RenderTexture maskTexture;

        private float maskCameraPositionZ;

        private float renderCameraPositionZ;

        private float maskScaleRatio;

        private List<GameObject> slotMasks = new List<GameObject>();

        public void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            renderCamera = GetComponent<Canvas>().worldCamera;
            textBaseScale = textBaseImage.transform.localScale.x;
            panelColor = new Color(1f, 1f, 1f, 0.6f);
            maskScaleRatio = maskCamera.orthographicSize / Camera.main.orthographicSize;
            scaleFromCameraToCanvas = UIUtility.GetCanvasSize(canvasScaler).y / (renderCamera.orthographicSize * 2f);
            renderCameraPositionZ = maskCamera.transform.position.z;
            skipButton.gameObject.SetActive(false);

            SetupMask();
        }

        public void OnDestroy()
        {
            if (maskTexture.IsCreated())
                maskTexture.Release();

            ClearMasks();
        }

        public void OnUpdateMainCamera()
        {

        }

        private void SetupMask()
        {
            maskTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 0);
            maskCamera.targetTexture = maskTexture;

            rawImage.texture = maskTexture;
            rawImage.color = Color.white;

            maskCameraPositionZ = maskCamera.transform.position.z;
        }

        public void ClearMasks()
        {
            for (int i = 0; i < slotMasks.Count; i++)
                Destroy(slotMasks[i]);

            slotMasks.Clear();

            //for (int i = canvasMask.transform.childCount; i >= 0; i--)
            //{
            //    Destroy(canvasMask.transform.GetChild(i));
            //}
        }

        public Transform PlaceSlotCombinedMask(Vector2 screenPosition, Vector2 offset = default(Vector2), float angle = 0f)
        {
            float delta = 15f * Screen.height / 1080f;

            GameObject slotMask = Instantiate(slotCombinedMaskPrefab);
            slotMask.transform.position = (Vector2)maskCamera.ScreenToWorldPoint(new Vector3((screenPosition.x + offset.x * delta) * 0.5f, (screenPosition.y + offset.y * delta) * 0.5f, -maskCameraPositionZ));
            slotMask.transform.localEulerAngles = new Vector3(0f, 0f, angle);
            slotMask.transform.localScale *= maskScaleRatio;
            slotMasks.Add(slotMask);

            return slotMask.transform;
        }

        public Transform PlaceSlotMask(Vector2 screenPosition)
        {
            GameObject slotMask = Instantiate(slotSingleMaskPrefab);
            slotMask.transform.position = (Vector2)maskCamera.ScreenToWorldPoint(new Vector3((screenPosition.x) * 0.5f, (screenPosition.y) * 0.5f, -maskCameraPositionZ));
            slotMask.transform.localScale *= maskScaleRatio;
            slotMasks.Add(slotMask);

            return slotMask.transform;
        }

        public Transform PlaceCircleMask(Vector2 screenPosition, float size)
        {
            GameObject circleMask = Instantiate(circleMaskPrefab);
            circleMask.transform.position = (Vector2)maskCamera.ScreenToWorldPoint(new Vector3((screenPosition.x) * 0.5f, (screenPosition.y) * 0.5f, -maskCameraPositionZ));
            circleMask.transform.localScale *= size;
            slotMasks.Add(circleMask);

            return circleMask.transform;
        }

        public RectTransform PlaceImageMask(Vector2 screenPosition, Image targetImage)
        {
            GameObject imageMask = Instantiate(imageMaskPrefab, canvasMask.transform);
            imageMask.transform.position = (Vector2)maskCamera.ScreenToWorldPoint(new Vector3((screenPosition.x) * 0.5f, (screenPosition.y) * 0.5f, -maskCameraPositionZ));
            slotMasks.Add(imageMask);

            Image image = imageMask.GetComponent<Image>();
            RectTransform rectTransform = image.rectTransform;

            rectTransform.sizeDelta = targetImage.rectTransform.sizeDelta * 0.5f;
            image.sprite = targetImage.sprite;
            

            return rectTransform;
        }

        public void SetTriggerInvisible(Vector2 screenPosition, SpriteRenderer spriteRenderer, Vector2 paddingCameraSpace = default(Vector2))
        {
            triggerButton.gameObject.SetActive(true);
            triggerButton.GetComponent<Image>().color = Color.clear;
            Vector3 targetPosition = renderCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -renderCameraPositionZ));
            triggerButton.transform.position = new Vector3(targetPosition.x, targetPosition.y, triggerButton.transform.position.z);

            Vector2 worldSize = spriteRenderer.bounds.size;
            worldSize += paddingCameraSpace;

            SetupTriggerButton(worldSize);
        }

        public void SetTriggerInvisible(Vector2 screenPosition, Image targetImage)
        {
            triggerButton.gameObject.SetActive(true);
            triggerButton.GetComponent<Image>().color = Color.clear;

            //triggerButton.transform.position = (Vector2)renderCamera.ScreenToWorldPoint(new Vector3((screenPosition.x) * 0.5f, (screenPosition.y) * 0.5f, -maskCameraPositionZ));

            Vector3 targetPosition = targetImage.transform.position;
            triggerButton.transform.position = new Vector3(targetPosition.x, targetPosition.y, triggerButton.transform.position.z);

            Image image = triggerButton.GetComponent<Image>();
            RectTransform rectTransform = image.rectTransform;

            rectTransform.sizeDelta = targetImage.rectTransform.sizeDelta;

            //SetupTriggerButton(worldSize);
        }

        void SetupTriggerButton(Vector2 worldSize)
        {
            RectTransform triggerRectTransform = triggerButton.GetComponent<RectTransform>();
            triggerRectTransform.sizeDelta = worldSize * scaleFromCameraToCanvas / 40f;
        }

        public void SetHand(Vector2 screenPosition, string animationClipName)
        {
            float positionZ = transform.position.z;

            Vector3 position = renderCamera.ScreenToWorldPoint(screenPosition);
            position.z = positionZ;
            handRoot.position = position;

            useHand = true;
            handRoot.gameObject.SetActive(useHand);
            handRoot.GetChild(0).transform.localPosition = Vector3.zero;
            handRoot.GetChild(0).GetComponent<Animator>().Play(animationClipName);
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

        public void SetInvisibleActive()
        {
            gameObject.SetActive(true);
            character.gameObject.SetActive(false);
            textBaseImage.gameObject.SetActive(false);
            handRoot.gameObject.SetActive(false);
            rawImage.color = Color.clear;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            character.gameObject.SetActive(true);
            textBaseImage.gameObject.SetActive(true);
            maskCamera.enabled = true;
            canvasMask.gameObject.SetActive(true);
            rawImage.DOColor(panelColor, fadeTime).OnComplete(() => { if (showSkipButton) skipButton.gameObject.SetActive(true); });
        }

        public void Hide(bool keepActive)
        {
            triggerButton.gameObject.SetActive(false);
            character.gameObject.SetActive(false);
            textBaseImage.gameObject.SetActive(false);
            handRoot.gameObject.SetActive(false);
            useHand = false;
            maskCamera.enabled = false;
            canvasMask.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);

            rawImage.DOColor(Color.clear, fadeTime).OnComplete(() =>
            {
                if (!keepActive)
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
