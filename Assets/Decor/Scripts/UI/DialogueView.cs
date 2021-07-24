using DG.Tweening;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIEdgeSnapPosition
{
    public UIEdgeSnapPosition(RectTransform rectTransform, Vector2 offset)
    {
        target = rectTransform;
        showPosition = target.anchoredPosition;

        if (offset.x == 0f) hidePosition.x = showPosition.x; else hidePosition.x = target.rect.size.x * offset.x;
        if (offset.y == 0f) hidePosition.y = showPosition.y; else hidePosition.y = target.rect.size.y * offset.y;
    }

    public void SetPositionVisibility(bool flag)
    {
        if (flag) target.anchoredPosition = showPosition; else target.anchoredPosition = hidePosition;
    }

    public Tween Show(float duration, bool doKill = true)
    {
        if (doKill) target.DOKill();
        var tween = target.DOAnchorPos(showPosition, duration);
        return tween;
    }

    public Tween Hide(float duration, bool doKill = true)
    {
        if (doKill) target.DOKill();
        var tween = target.DOAnchorPos(hidePosition, duration);
        return tween;
    }

    public RectTransform target;
    public Vector2 showPosition;
    public Vector2 hidePosition;
}

public class DialogueView : MonoBehaviour
{
    public Image panel;

    public Image topPanel;

    public Image botPanel;

    public Text tapAnywhereToContinueText;

    public Button skipButton;

    public Transform rightCharacter;

    public Text dialogText;

    public Transform leftCharacter;

    public float characterInterval = 0.1f;

    private bool showingLastDialogue;

    private bool isOpening, isClosing, canTap;

    public Action OnDialogueCloseAction;

    public Action OnSkipAction;

    private UIEdgeSnapPosition topPanelSnapPos;

    private UIEdgeSnapPosition botPanelSnapPos;

    private bool isAppendingCharacter;

    private string currentContent;   

    public void Awake()
    {
        panel.GetComponent<Image>().color = Color.clear;
        dialogText.text = "";

        topPanelSnapPos = new UIEdgeSnapPosition(topPanel.rectTransform, new Vector2(0f, 1.2f));
        botPanelSnapPos = new UIEdgeSnapPosition(botPanel.rectTransform, new Vector2(0f, -1.2f));

        skipButton.onClick.AddListener(() => 
        {
            Finish();
            OnSkipAction?.Invoke();
        });

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => OnPointerDown(null));
        panel.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    public void ShowDialogue(string content, int side, bool isFirstDialog, bool isLastDialog)
    {
        if (string.IsNullOrEmpty(content)) 
            content = "_";

        currentContent = content;
        showingLastDialogue = isLastDialog;
        float delayTime = 0.5f;

        if (isFirstDialog)
        {
            canTap = true;
            isOpening = true;
            delayTime = 0.75f;

            dialogText.gameObject.SetActive(true);

            topPanel.gameObject.SetActive(true);
            botPanel.gameObject.SetActive(true);

            topPanelSnapPos.SetPositionVisibility(false);
            botPanelSnapPos.SetPositionVisibility(false);
            topPanelSnapPos.target.DOAnchorPos(topPanelSnapPos.showPosition, 0.5f);
            botPanelSnapPos.target.DOAnchorPos(botPanelSnapPos.showPosition, 0.5f).OnComplete(() => isOpening = false);
        }

        dialogText.text = "";

        if (side == 1)
        {
            SetCharacterActive(leftCharacter.transform, false);
            SetCharacterActive(rightCharacter.transform, true);
        }       
        else
        {
            SetCharacterActive(leftCharacter.transform, true);
            SetCharacterActive(rightCharacter.transform, false);
        }

        StartCoroutine(AppendCharacterCoroutine(delayTime));
       
    }

    public IEnumerator AppendCharacterCoroutine(float delayTime = 0f)
    {
        if (delayTime > 0f)
            yield return new WaitForSeconds(delayTime);

        StringBuilder sb = new StringBuilder(currentContent.Length);
        var waitForCharacterInterval = new WaitForSeconds(characterInterval);
        int charIdx = 0;
        isAppendingCharacter = true;

        char c = ' ';
        int t = 0;


        while (charIdx < currentContent.Length)
        {
            if (isAppendingCharacter)
            {
                c = currentContent[charIdx];

                if (c == '<')
                {
                    if (t == 0)
                    {
                        sb.Append(currentContent.Substring(charIdx, 16));
                        charIdx += 16;

                        t++;
                    }
                    else
                    {
                        if (charIdx + 9 >= currentContent.Length)
                        {
                            sb.Append(currentContent.Substring(charIdx, 8));
                            charIdx += 8;
                        }
                        else
                        {
                            sb.Append(currentContent.Substring(charIdx, 9));
                            charIdx += 9;
                        }

                        t = 0;
                    }
                }
                else
                {
                    sb.Append(c);
                    charIdx++;

                }

                if (t > 0)
                    dialogText.text = sb.ToString() + "</color>";
                else
                    dialogText.text = sb.ToString();

                yield return waitForCharacterInterval;
            }
            else
            {
                break;
            }
        }

        dialogText.text = currentContent;      
        isAppendingCharacter = false;
    }

    public void Finish()
    {
        isClosing = true;       

        dialogText.gameObject.SetActive(false);

        topPanelSnapPos.SetPositionVisibility(true);
        botPanelSnapPos.SetPositionVisibility(true);
        topPanelSnapPos.target.DOAnchorPos(topPanelSnapPos.hidePosition, 0.5f).OnComplete(() => { topPanelSnapPos.target.gameObject.SetActive(false); });
        botPanelSnapPos.target.DOAnchorPos(botPanelSnapPos.hidePosition, 0.5f).OnComplete(() =>
        { 
            botPanelSnapPos.target.gameObject.SetActive(false);
            gameObject.SetActive(false);
            isClosing = false;
        });

        SetCharacterActive(leftCharacter.transform, false);
        SetCharacterActive(rightCharacter.transform, false);
    }

    private void SetCharacterActive(Transform characterTransform, bool flag)
    {
        if (flag)
        {
            if (!characterTransform.gameObject.activeSelf)
            {
                characterTransform.localScale = new Vector3(0.925f, 0.875f, 1f);
                characterTransform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetDelay(0.25f).OnStart(() => characterTransform.gameObject.SetActive(true));
            }
        }
        else
        {
            if (characterTransform.gameObject.activeSelf)
            {
                characterTransform.gameObject.SetActive(false);
            }
        }       
    }

    bool clicked;
    public void OnPointerDown(PointerEventData eventData)
    {        
        if (!isClosing && !isOpening && canTap && !clicked)
        {
            canTap = false;
            clicked = true;
            this.ExecuteAfterSeconds(0.2f, () => 
            {
                canTap = true;
                clicked = false;
            });

            if (isAppendingCharacter)
            {
                dialogText.text = currentContent;
                isAppendingCharacter = false;
            }
            else
            {
                if (showingLastDialogue && !clicked)
                    Finish();

                OnDialogueCloseAction?.Invoke();
            }
        }  
    }
}
