using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decor
{
    [ExecuteInEditMode]
    public class DecompressedDesignItem : MonoBehaviour
    {
        public bool replicateSpriteNameToId = true;

        public string previousItem = string.Empty;

        [Header("Model")]
        public DesignItemData primaryData = new DesignItemData();

        [Header("View")]
        public Sprite defaultSprite;

        public Sprite defaultSprite_2;

        public Sprite[] sprites;

        public Sprite[] sprites_2;

        public DesignItemEffectType effectType;

        private GameObject icon;

        [NonSerialized] public GameObject default_variant1;

        [NonSerialized] public GameObject default_variant2;

        [NonSerialized] public List<GameObject> variants1 = new List<GameObject>();

        [NonSerialized] public List<GameObject> variants2 = new List<GameObject>();

        private bool hasChanged = false;

        private int subvisualCount = 1;

        public void SetVariant(int index)
        {
            primaryData.variantIndex = index;

            if (index == -1)
            {
                if (default_variant1)
                {
                    default_variant1.SetActive(true);
                    default_variant1.GetComponent<SpriteRenderer>().sprite = defaultSprite;
                }

                if (default_variant2)
                {
                    default_variant2.SetActive(true);
                    default_variant2.GetComponent<SpriteRenderer>().sprite = defaultSprite_2;
                }

                for (int i = 0; i < variants1.Count; i++)
                    variants1[i].SetActive(false);

                for (int i = 0; i < variants2.Count; i++)
                    variants2[i].SetActive(false);
            }
            else
            {
                if (default_variant1)
                    default_variant1.SetActive(false);

                if (default_variant2)
                    default_variant2.SetActive(false);


                for (int i = 0; i < variants1.Count; i++)
                    variants1[i].SetActive(false);

                for (int i = 0; i < variants2.Count; i++)
                    variants2[i].SetActive(false);

                primaryData.variantIndex = Mathf.Clamp(primaryData.variantIndex, 0, sprites.Length - 1);

                variants1[primaryData.variantIndex].SetActive(true);
                variants1[primaryData.variantIndex].GetComponent<SpriteRenderer>().sprite = sprites[primaryData.variantIndex];

                if (subvisualCount >= 2)
                {
                    variants2[primaryData.variantIndex].SetActive(true);
                    variants2[primaryData.variantIndex].GetComponent<SpriteRenderer>().sprite = sprites_2[primaryData.variantIndex];
                }
            }
        }

        public int GetSubvisualCount()
        {
            return transform.childCount - 1;
        }

        public int GetVariantCount()
        {
            return variants1.Count;
        }

        public void Awake()
        {
        }

        public void OnValidate()
        {
            hasChanged = true;
        }

        public void Update()
        {
            if (!hasChanged) return;

            hasChanged = false;

            variants1.Clear();
            variants2.Clear();

            icon = transform.Find("icon").gameObject;

            subvisualCount = GetSubvisualCount();

            if (subvisualCount >= 1)
            {
                var subvisual = transform.Find("sub0");
                var dfxf = subvisual.Find("default");
                if (dfxf)
                    default_variant1 = dfxf.gameObject;

                for (int i = (dfxf != null) ? 1 : 0; i < subvisual.childCount; i++)
                {
                    variants1.Add(subvisual.GetChild(i).gameObject);
                }
            }

            if (subvisualCount >= 2)
            {
                var subvisual2 = transform.Find("sub1");
                var dfxf2 = subvisual2.Find("default");
                if (dfxf2)
                    default_variant2 = dfxf2.gameObject;

                for (int i = (dfxf2 != null) ? 1 : 0; i < subvisual2.childCount; i++)
                {
                    variants2.Add(subvisual2.GetChild(i).gameObject);
                }
            }

            if (replicateSpriteNameToId && sprites != null && sprites.Length > 0 && sprites[0])
            {
                string spritename = sprites[0].name;
                gameObject.name = primaryData.id = spritename.Substring(0, spritename.Length - 2);
            }
            else if (!string.IsNullOrEmpty(primaryData.id))
            {
                gameObject.name = primaryData.id;
            }

            SetVariant(primaryData.variantIndex);
        }
    }
}

