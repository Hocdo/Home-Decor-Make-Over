using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Popup
{
    public class PopupOpenBuilder
    {
        protected internal PopupType type = PopupType.None;

        protected internal CurrentPopupBehaviour currentPopupBehaviour = CurrentPopupBehaviour.Close;

        protected internal bool backgroundEffectEnabled = true;

        protected internal Action backBlockerEvent = null;

        protected internal float delayTime = 0f;

        private PopupSystem popupSystem;

        public PopupOpenBuilder(PopupSystem popupSystem)
        {
            this.popupSystem = popupSystem;
            backBlockerEvent = popupSystem.ClosePopup;
        }

        public PopupOpenBuilder SetType(PopupType type)
        {
            this.type = type;
            return this;
        }

        public PopupOpenBuilder SetCurrentPopupBehaviour(CurrentPopupBehaviour currentPopupBehaviour)
        {
            this.currentPopupBehaviour = currentPopupBehaviour;
            return this;
        }

        public PopupOpenBuilder SetBackgroundEffectEnabled(bool backgroundEffectEnabled)
        {
            this.backgroundEffectEnabled = backgroundEffectEnabled;
            return this;
        }

        public PopupOpenBuilder SetBackBlockerEvent(Action backBlockerEvent)
        {
            this.backBlockerEvent = backBlockerEvent;
            return this;
        }

        public PopupOpenBuilder SetDelayTime(float delayTime)
        {
            this.delayTime = delayTime;
            return this;
        }

        public T Open<T>() where T : PopupBase
        {
            return PopupSystem.Instance.Open(this) as T;
        }

        public PopupBase Open()
        {
            return PopupSystem.Instance.Open(this);
        }
    }
}

