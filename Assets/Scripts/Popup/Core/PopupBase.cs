using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Popup
{
    public class PopupBase : MonoBehaviour
    {
        public Action PostAnimateShowEvent;

        public Action PreAnimateHideEvent;

        public Action PostAnimateHideEvent;

        public Action<object> AcceptEvent;

        public Action<object> DenyEvent;

        protected Transform cachedTransform;

        protected CanvasGroup canvasGroup;

        protected bool canClose;

        protected internal PopupType type;

        void Awake()
        {
            cachedTransform = transform;
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual bool CanClose()
        {
            return canClose;
        }

        public virtual void Show()
        {
        }

        public virtual void Close(bool forceDestroying = true)
        {
            TerminateInternal(forceDestroying);         
        }

        protected void TerminateInternal(bool forceDestroying = true)
        {
            if (forceDestroying)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);

            PopupSystem.Instance.OnPopupTerminate();
        }

        public void CloseInternal()
        {
            PopupSystem.Instance.ClosePopup();
        }        
    }
}
