using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Popup
{
    public enum CurrentPopupBehaviour
    {
        KeepShowing,
        HideTemporary,
        Close
    }

    public class PopupSystem : SingletonMonoBehaviour<PopupSystem>
    {
        [Serializable]
        public struct PopupTypePrefabPair
        {
            public PopupType type;
            public GameObject prefab;
        }

        public class PopupInfo
        {
            public PopupType type;
            public PopupBase instance;
            public bool enableBackgroundEffect;
            public Action backBlockerPressedEvent;
        }

        public Camera popupCamera;

        public PopupBackgroundEffect bgEffect;

        public BlockingImage backBlocker;

        public Transform rootTransform;

        public Transform addOnTransform;

        public Stack<PopupInfo> popupInfoStack = new Stack<PopupInfo>();

        public List<PopupTypePrefabPair> popupList;

        private Dictionary<PopupType, GameObject> popupTable = new Dictionary<PopupType, GameObject>();

        public Action<PopupBase> ShowPopupEvent;

        public Action<PopupBase> ClosePopupEvent;

        public Action ClearPopupEvent;

        public Action<PopupType> PlaySFXOnShowEvent;

        public Action<PopupType> PlaySFXOnCloseEvent;

        public override void Awake()
        {
            base.Awake();

            if (backBlocker.transform.parent != rootTransform)
                backBlocker.transform.parent = rootTransform;

            backBlocker.gameObject.SetActive(false);

            for (int i = 0; i < popupList.Count; i++)
            {
                popupTable.Add(popupList[i].type, popupList[i].prefab);
            }
        }

        public bool IsShowingPopup()
        {
            return popupInfoStack.Count > 0;
        }

        public void ForceBackgroundEffectActive(bool flag)
        {
            bgEffect.SetActive(flag);
        }

        private void HandleCurrentPopup(CurrentPopupBehaviour currentPopupBehaviour)
        {
            var currentPopupInfo = popupInfoStack.Peek();
            var currentPopup = currentPopupInfo.instance;
            if (currentPopup != null) // not necessary
            {
                switch (currentPopupBehaviour)
                {
                    case CurrentPopupBehaviour.Close:
                        {
                            popupInfoStack.Pop().instance.Close();
                            break;
                        }
                    case CurrentPopupBehaviour.HideTemporary:
                        {
                            currentPopup.Close(false);
                            break;
                        }
                    case CurrentPopupBehaviour.KeepShowing:
                    default:
                        {
                            break;
                        }
                }
            }
        }

        public void SetBackBlockerPressedEventOfCurrentPopup(Action BackBlockerPressedEvent)
        {
            if (popupInfoStack.Count > 0)
            {
                popupInfoStack.Peek().backBlockerPressedEvent = BackBlockerPressedEvent;
                backBlocker.PointerDownAction = BackBlockerPressedEvent;
            }
        }

        public static PopupOpenBuilder GetOpenBuilder()
        {
            return new PopupOpenBuilder(Instance);
        }

        public PopupBase Open(PopupOpenBuilder openBuilder)
        {
            //Debug.Log(popupType);

            if (popupTable.ContainsKey(openBuilder.type) == false)
                return null;

            GameObject prefab = popupTable[openBuilder.type];
            PopupBase popup = Instantiate(prefab, rootTransform).GetComponent<PopupBase>();
            popup.type = openBuilder.type;

            if (popup == null)
                return null;

            if (openBuilder.delayTime > 0f)
                popup.gameObject.SetActive(false);

            ShowPopupEvent?.Invoke(popup);

            if (popupInfoStack.Count > 0)
            {
                HandleCurrentPopup(openBuilder.currentPopupBehaviour);
            }

            PopupInfo popupInfo = new PopupInfo();
            popupInfo.type = openBuilder.type;
            popupInfo.instance = popup;
            popupInfo.enableBackgroundEffect = openBuilder.backgroundEffectEnabled;
            popupInfo.backBlockerPressedEvent = openBuilder.backBlockerEvent;

            backBlocker.transform.SetSiblingIndex(rootTransform.childCount - 2);
            backBlocker.gameObject.SetActive(true);
            backBlocker.PointerDownAction = openBuilder.backBlockerEvent;

            popupInfoStack.Push(popupInfo);

            if (openBuilder.delayTime > 0f)
            {
                this.ExecuteAfterSeconds(openBuilder.delayTime, () =>
                {
                    popup.gameObject.SetActive(true);

                    popup.Show();
                    ForceBackgroundEffectActive(openBuilder.backgroundEffectEnabled);
                    PlaySFXOnShowEvent?.Invoke(openBuilder.type);
                });
            }
            else
            {
                popup.Show();
                ForceBackgroundEffectActive(openBuilder.backgroundEffectEnabled);
                PlaySFXOnShowEvent?.Invoke(openBuilder.type);
            }

            return popup;
        }

        public PopupBase ShowPopup(PopupType popupType, CurrentPopupBehaviour currentPopupBehaviour = CurrentPopupBehaviour.Close, bool exitByBackBlocker = true, bool enableBackgroundEffect = true, float delayTime = 0f)
        {
            if (exitByBackBlocker)
                return ShowPopup(popupType, currentPopupBehaviour, ClosePopup, enableBackgroundEffect, delayTime);
            else
                return ShowPopup(popupType, currentPopupBehaviour, null, enableBackgroundEffect, delayTime);
        }

        public PopupBase ShowPopup(PopupType popupType, CurrentPopupBehaviour currentPopupBehaviour, Action BackBlockerPressedEvent = null, bool enableBackgroundEffect = true, float delayTime = 0f)
        {
            if (popupTable.ContainsKey(popupType) == false)
                return null;

            GameObject prefab = popupTable[popupType];
            PopupBase popup = Instantiate(prefab, rootTransform).GetComponent<PopupBase>();
            popup.type = popupType;

            if (popup == null)
                return null;

            if (delayTime > 0f)
                popup.gameObject.SetActive(false);

            ShowPopupEvent?.Invoke(popup);

            if (popupInfoStack.Count > 0)
            {
                HandleCurrentPopup(currentPopupBehaviour);
            }

            PopupInfo popupInfo = new PopupInfo();
            popupInfo.type = popupType;
            popupInfo.instance = popup;
            popupInfo.enableBackgroundEffect = enableBackgroundEffect;
            popupInfo.backBlockerPressedEvent = BackBlockerPressedEvent;

            backBlocker.transform.SetSiblingIndex(rootTransform.childCount - 2);
            backBlocker.gameObject.SetActive(true);
            backBlocker.PointerDownAction = BackBlockerPressedEvent;

            popupInfoStack.Push(popupInfo);

            if (delayTime > 0f)
            {
                this.ExecuteAfterSeconds(delayTime, () => 
                {                        
                    popup.gameObject.SetActive(true);
                    popup.Show();
                    ForceBackgroundEffectActive(enableBackgroundEffect);
                    PlaySFXOnShowEvent?.Invoke(popupType);
                });
            }
            else
            {
                popup.Show();
                ForceBackgroundEffectActive(enableBackgroundEffect);
                PlaySFXOnShowEvent?.Invoke(popupType);
            }         

            return popup;
        }

        public void ClosePopup()
        {
            if (popupInfoStack.Count > 0)
            {
                PopupInfo popupInfo = popupInfoStack.Peek();
                PopupBase popup = popupInfo.instance;

                if (popup.CanClose())
                {
                    popupInfoStack.Pop();

                    ClosePopupEvent?.Invoke(popup);

                    ClosePopup(popup);                  

                    PlaySFXOnCloseEvent?.Invoke(popupInfo.type);

                    if (popupInfoStack.Count == 0)
                    {
                        ClearPopupEvent?.Invoke();

                        bgEffect.SetActive(false);
                    }
                    else
                    {
                        popupInfo = popupInfoStack.Peek();
                        if (popupInfo.instance.gameObject.activeSelf == false)
                        {
                            popupInfo.instance.gameObject.SetActive(true);
                            popupInfo.instance.Show();
                        }

                        backBlocker.PointerDownAction = popupInfo.backBlockerPressedEvent;
                        bgEffect.SetActive(popupInfo.enableBackgroundEffect);
                    }
                }                 
            }
        }

        public void CloseAllPopupsImmediately()
        {
            foreach (PopupInfo popupInfo in popupInfoStack)
            {
                ClosePopup(popupInfo.instance);
            }
            popupInfoStack.Clear();

            backBlocker.gameObject.SetActive(false);
            backBlocker.PointerDownAction = null;

            bgEffect.SetActiveImmediately(false);      
        }

        public void CloseAllPopups()
        {
            foreach (PopupInfo popupInfo in popupInfoStack)
            {
                ClosePopup(popupInfo.instance);
            }
            popupInfoStack.Clear();

            backBlocker.gameObject.SetActive(false);
            backBlocker.PointerDownAction = null;

            bgEffect.SetActive(false);
        }

        private void ClosePopup(PopupBase popup, bool forceDestroying = true)
        {
            popup.Close(forceDestroying);
        }

        public void OnPopupTerminate()
        {
            if (popupInfoStack.Count == 0)
            {
                backBlocker.gameObject.SetActive(false);
                backBlocker.PointerDownAction = null;
            }
            else
            {
                var popupInfo = popupInfoStack.Peek();

                backBlocker.transform.SetSiblingIndex(Mathf.Max(0, rootTransform.childCount - 3));
                backBlocker.gameObject.SetActive(true);
                backBlocker.PointerDownAction = popupInfo.backBlockerPressedEvent;
            }
        }
    }
}
