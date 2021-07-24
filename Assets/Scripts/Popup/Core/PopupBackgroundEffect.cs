using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Popup
{
    public abstract class PopupBackgroundEffect : MonoBehaviour
    {
        public float enabledDuration = 0.25f;

        public float disabledDuration = 0.25f;

        public abstract void SetActive(bool flag);

        public abstract void SetActiveImmediately(bool flag);

        public abstract bool IsActive();
    }
}

