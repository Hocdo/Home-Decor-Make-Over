using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Decor
{
    public class DesignItemView : MonoBehaviour, ISpriteRenderersProvider
    {
        public static readonly int DefaultVariantIndex = -1;
        public string previousId = string.Empty;
        #region Inspector
        [Header("Model")]
        public DesignItemData primaryData = new DesignItemData();

        [Header("View")]
        public DesignItemEffectType effectType;

        public SubVisual[] subVisuals;

        public DesignItemView[] unlockDependencies;
        #endregion

        private SpriteRenderer[] subVisualSpriteRenderers;

        public SpriteRenderer[] GetSpriteRenderers()
        {
            return subVisualSpriteRenderers;
        }

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            subVisualSpriteRenderers = new SpriteRenderer[subVisuals.Length];

            for (int i = 0; i < subVisualSpriteRenderers.Length; i++)
            {
                var subvisualGameObject = transform.Find("sub" + i);
                subVisualSpriteRenderers[i] = subvisualGameObject.GetComponent<SpriteRenderer>();
            }

            int variantCostCount = primaryData.variantCosts.Length;
            for (int i = 0; i < primaryData.variantCount; i++)
            {
                if (i < variantCostCount)
                {
                    if (primaryData.variantCosts[i].type == CurrencyType.Star 
                        || primaryData.variantCosts[i].type == CurrencyType.None 
                        || primaryData.variantCosts[i].value <= 0)
                    {
                        primaryData.UnlockVariant(i);
                    }
                }
                else
                {
                    primaryData.UnlockVariant(i);
                }
            }
        }

        public void SetVisualActive(bool flag)
        {
            for (int i = 0; i < subVisualSpriteRenderers.Length; i++)
            {
                subVisualSpriteRenderers[i].gameObject.SetActive(flag);
            }
        }

        public void UpdatePhysicsShape()
        {
            List<Vector2> physicsShape = new List<Vector2>();

            for (int i = 0; i < subVisuals.Length; i++)
            {
                physicsShape.Clear();

                subVisualSpriteRenderers[i].sprite.GetPhysicsShape(0, physicsShape);
                subVisualSpriteRenderers[i].GetComponent<PolygonCollider2D>().points = physicsShape.ToArray();
            }
        }

        public void SetVariant(int idx)
        {
            int maxcount = GetVariantCount();
            if (idx > maxcount - 1)
            {
                idx = maxcount - 1;
            }
            primaryData.variantIndex = idx;

            if (primaryData.variantIndex == DefaultVariantIndex)
            {
                for (int i = 0; i < subVisuals.Length; i++)
                {
                    subVisualSpriteRenderers[i].sprite = subVisuals[i].defaultVariant.sprite;
                    subVisualSpriteRenderers[i].transform.localPosition = subVisuals[i].defaultVariant.offset;
                }
            }
            else if (idx >= 0 && idx < maxcount)
            {
                for (int i = 0; i < subVisuals.Length; i++)
                {
                    subVisualSpriteRenderers[i].sprite = subVisuals[i].collectionOfVariant[primaryData.variantIndex].sprite;
                    subVisualSpriteRenderers[i].transform.localPosition = subVisuals[i].collectionOfVariant[primaryData.variantIndex].offset;
                }
            }
        }

        public int GetVariantCount()
        {
            return subVisuals[0].collectionOfVariant.Length;
        }
     
        #region Internal Struct
        [Serializable]
        public class Variant
        {
            public Sprite sprite;
            public Vector3 offset;
        }

        [Serializable]
        public class SubVisual
        {
            public Variant defaultVariant;
            public Variant[] collectionOfVariant;
        }
        #endregion

        #region Validate
#if UNITY_EDITOR
        //void OnValidate()
        //{
        //    SetVariant(primaryData.variantIndex);
        //}
#endif
        #endregion
    }
}

public interface ISpriteRenderersProvider
{
    SpriteRenderer[] GetSpriteRenderers();
}
