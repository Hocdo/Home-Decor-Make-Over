using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageUpdater : MonoBehaviour
{
    public LanguageType currentLanguage = LanguageType.ENGLISH;

    private LanguageType previousLanguage = LanguageType.ENGLISH;

#if UNITY_EDITOR
    private void Update()
    {
        if (currentLanguage != previousLanguage)
        {
            previousLanguage = currentLanguage;
            CustomLocalization.SetLanguage(currentLanguage);
        }
    }
#endif
}
