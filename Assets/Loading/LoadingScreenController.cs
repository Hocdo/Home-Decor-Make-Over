using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingScreenController : MonoBehaviour
{
  
    [Header("Close")]
    public AnimationCurve dropCurve;

    public float dropDuration;

    [Header("Open")]
    public AnimationCurve liftCurve;

    public float liftDuration;

    [Header("References")]
    public Image bgImage;

    public RectTransform barRectTransform;

    public Text textLoading;

    public GameObject[] tipPack;

    public Image fillImg;

    [Header("GC")]
    public bool triggerGarbageCollector = true;

    private int showCount;

    private Vector2 sourcePos;

    private Vector2 targetPos;

    private float canvasWidth;

    public void Awake()
    {
        var canvasScaler = GetComponent<CanvasScaler>();

        Vector2 canvasSize = UIUtility.GetCanvasSize(canvasScaler);
        canvasWidth = canvasSize.x;
        sourcePos = barRectTransform.anchoredPosition;
        targetPos = new Vector2(-sourcePos.x * 2f + canvasSize.x, sourcePos.y); 
      
    }

    public void StartAnimating(string sceneName, Action PreLoadAction, Action PostLoadAction)
    {
        gameObject.SetActive(true);

        StartCoroutine(LoadingCoroutine(sceneName, PreLoadAction, PostLoadAction));
    }

    public IEnumerator LoadingCoroutine(string sceneName, Action PreloadAction, Action PostLoadAction)
    {
        AudioManager.Play_SFX(AudioClipId.LoadingPaper); 

        Color zeroAlpha = new Color(1f, 1f, 1f, 0f);

        for (int i = 0; i < tipPack.Length; i++)
        {
            tipPack[i].SetActive(false);
        }
      
        var tipImage = tipPack[(showCount++) % tipPack.Length].GetComponent<Image>();
        tipImage.gameObject.SetActive(true);
        tipImage.color = zeroAlpha;
        //Vector2 anchorPos = tipImage.rectTransform.anchoredPosition;
        //tipImage.rectTransform.anchoredPosition = anchorPos - new Vector2(50f, 0f);

        //textLoading.color = zeroAlpha;
        textLoading.color = Color.white;

        fillImg.fillAmount = 0f;

        //textLoading.DOColor(Color.white, 0.25f);
        tipImage.DOColor(Color.white, 0.25f);

        Material material = bgImage.material;
        material.SetFloat("_Slider", 1 /*barRectTransform.anchoredPosition.x / canvasWidth*/);
        //yield return new WaitForSeconds(0.15f);

        //barRectTransform.anchoredPosition = sourcePos;
        //barRectTransform.DOAnchorPos(targetPos, dropDuration).SetEase(dropCurve)
        //    .OnUpdate(() => material.SetFloat("_Slider", barRectTransform.anchoredPosition.x / canvasWidth));

        //yield return new WaitForSeconds(dropDuration - 0.25f);

        //tipImage.rectTransform.DOAnchorPos(anchorPos, 0.25f).SetEase(Ease.OutQuad);
        //textLoading.DOColor(Color.white, 0.25f);
        //tipImage.DOColor(Color.white, 0.25f);

        //yield return new WaitForSeconds(0.25f);
        while (fillImg.fillAmount < 0.9f)
        {
            fillImg.fillAmount += Time.deltaTime *.75f ;
            yield return null;
        }
        //yield return new WaitForSeconds(0.75f);
        PreloadAction?.Invoke();

        var asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {           
            yield return null;
        }
        while (fillImg.fillAmount < 1f)
        {
            fillImg.fillAmount += Time.deltaTime;
            yield return null;
        }
        if (triggerGarbageCollector)
        {
            GC.Collect();

#if UNITY_EDITOR
            Debug.LogFormat("Memory used after full collection:   {0:N0}", GC.GetTotalMemory(true));
#endif
        }

      
        textLoading.DOColor(zeroAlpha, 0.35f);
        tipImage.DOColor(zeroAlpha, 0.35f);

        yield return new WaitForSeconds(0.4f);
        
        PostLoadAction?.Invoke();

        //barRectTransform.anchoredPosition = targetPos;
        //barRectTransform.DOAnchorPos(sourcePos, dropDuration).SetEase(liftCurve)
        //    .OnUpdate(() => material.SetFloat("_Slider", barRectTransform.anchoredPosition.x / canvasWidth));

        //yield return new WaitForSeconds(liftDuration);

        gameObject.SetActive(false);
       
    }
}
