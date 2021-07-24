using UnityEngine;

public class DesignItemEffectController : MonoBehaviour
{
    public DesignItemEffect[] collection;

    private int playingInstanceCount;

    public void SetTarget(SpriteRenderer[] atargetRenderer, DesignItemEffectType effectType)
    {
        playingInstanceCount = Mathf.Min(atargetRenderer.Length, collection.Length);

        for (int i = 0; i < playingInstanceCount; i++)
        {
            collection[i].SetTarget(atargetRenderer[i], effectType);
        }
    }

    public void PlayOnSelect()
    {
        for (int i = 0; i < playingInstanceCount; i++)
        {
            collection[i].PlayOnSelect();
        }
    }

    public void PlayOnDeselect()
    {
        for (int i = 0; i < playingInstanceCount; i++)
        {
            collection[i].PlayOnDeselect();
        }
    }

    public void PlayOnChange()
    {
        for (int i = 0; i < playingInstanceCount; i++)
        {
            collection[i].PlayOnChange();
        }
    }

    public void PlayOnApply()
    {
        for (int i = 0; i < playingInstanceCount; i++)
        {
            collection[i].PlayOnApply();
        }
    }
}
