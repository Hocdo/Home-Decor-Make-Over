using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;

#if UNITY_EDITOR
using UnityEditor;

namespace Decor
{
    
    public class DesiginItemConverterWindow : EditorWindow
    {
        public GameObject items;

        [MenuItem("Window/DesiginItemConverterWindow")]
        public static void ShowWindow()
        {
            GetWindow(typeof(DesiginItemConverterWindow));
        }

        void OnGUI()
        {
            items = (GameObject)EditorGUILayout.ObjectField("Item Parent", items, typeof(GameObject), true);

            if (GUILayout.Button("Compress"))
            {
                if (items)
                    Compress(items);
            }

            if (GUILayout.Button("Decompress"))
            {
                if (items)
                    Decompress(items);
            }

            GUILayout.Space(100f);

            if (GUILayout.Button("Rename Assets"))
            {
                RenameAssets();
            }

            GUILayout.Space(100f);

            if (GUILayout.Button("Reduce cost"))
            {
                for (int i = 0; i < items.transform.childCount; i++)
                {
                    var item = items.transform.GetChild(i).GetComponent<DesignItemView>();
                    int cost = item.primaryData.costToUnlock.value;

                    float x = cost * 0.8f;
                    x /= 10;
                    cost = 10 * (int)x;

                    item.primaryData.costToUnlock = new Currency() { type = CurrencyType.Coin, value = cost };
                }
            }
        }

        public void RenameAssets()
        {
            string prefix = "c1_r9_";

            string dataPath = "C:/Unity/Projects/CookingHomeDesign/";
            string[] aFilePaths = Directory.GetFiles("C:/Unity/Projects/CookingHomeDesign/Assets/Room/9_Sky Restaurant/Texture2D");

            foreach (string sFilePath in aFilePaths)
            {
                if (!sFilePath.Contains(".meta"))
                {
                    string assetPath = sFilePath.Substring(dataPath.Length, sFilePath.Length - dataPath.Length);
                    string assetName = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath).name;

                    string newName = prefix + assetName.Substring(6, assetName.Length - 6);

                    AssetDatabase.RenameAsset(assetPath, newName);
                }
            }

            aFilePaths = Directory.GetFiles("C:/Unity/Projects/CookingHomeDesign/Assets/Room/9_Sky Restaurant/UI");

            foreach (string sFilePath in aFilePaths)
            {
                if (!sFilePath.Contains(".meta"))
                {
                    string assetPath = sFilePath.Substring(dataPath.Length, sFilePath.Length - dataPath.Length);
                    string assetName = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath).name;

                    string newName = prefix + assetName.Substring(6, assetName.Length - 6);

                    AssetDatabase.RenameAsset(assetPath, newName);
                }
            }
        }

        public void Compress(GameObject items)
        {
            Transform parent = items.transform;

            parent.parent.gameObject.name = "room";

            for (int i = 0; i < parent.childCount; i++)
            {
                // replicate data
                var xf = parent.GetChild(i);
                AddComponentIfNotAvailable<DesignItemTouchTrigger>(xf.gameObject);
                DesignItemView div = xf.gameObject.AddComponent<DesignItemView>();
                DecompressedDesignItem ddi = xf.GetComponent<DecompressedDesignItem>();
                div.primaryData = ddi.primaryData;
                div.effectType = ddi.effectType;
                div.previousId = ddi.previousItem;

                int subvisualCount = ddi.GetSubvisualCount();
                int variantCount = ddi.GetVariantCount();

                div.subVisuals = new DesignItemView.SubVisual[subvisualCount];

                if (subvisualCount >= 1)
                {
                    div.subVisuals[0] = new DesignItemView.SubVisual();

                    if (ddi.defaultSprite)
                    {
                        div.subVisuals[0].defaultVariant = new DesignItemView.Variant();
                        div.subVisuals[0].defaultVariant.sprite = ddi.defaultSprite;
                        div.subVisuals[0].defaultVariant.offset = ddi.default_variant1.transform.localPosition
                            + ddi.default_variant1.transform.parent.localPosition;
                    }

                    div.subVisuals[0].collectionOfVariant = new DesignItemView.Variant[variantCount];
                    for (int vi = 0; vi < variantCount; vi++)
                    {
                        div.subVisuals[0].collectionOfVariant[vi] = new DesignItemView.Variant();
                        div.subVisuals[0].collectionOfVariant[vi].sprite = ddi.sprites[vi];
                        div.subVisuals[0].collectionOfVariant[vi].offset = ddi.variants1[vi].transform.localPosition
                            + ddi.variants1[vi].transform.parent.localPosition;
                    }

                    var sub0 = xf.Find("sub0");
                    RemoveAllChildren(sub0);

                    AddComponentIfNotAvailable<SpriteRenderer>(sub0.gameObject);
                    AddComponentIfNotAvailable<PolygonCollider2D>(sub0.gameObject);
                }

                if (subvisualCount >= 2)
                {
                    div.subVisuals[1] = new DesignItemView.SubVisual();

                    if (ddi.defaultSprite_2)
                    {
                        div.subVisuals[1].defaultVariant = new DesignItemView.Variant();
                        div.subVisuals[1].defaultVariant.sprite = ddi.defaultSprite_2;
                        div.subVisuals[1].defaultVariant.offset = ddi.default_variant2.transform.localPosition
                            + ddi.default_variant2.transform.parent.localPosition;
                    }

                    div.subVisuals[1].collectionOfVariant = new DesignItemView.Variant[variantCount];
                    for (int vi = 0; vi < variantCount; vi++)
                    {
                        div.subVisuals[1].collectionOfVariant[vi] = new DesignItemView.Variant();
                        div.subVisuals[1].collectionOfVariant[vi].sprite = ddi.sprites_2[vi];
                        div.subVisuals[1].collectionOfVariant[vi].offset = ddi.variants2[vi].transform.localPosition
                            + ddi.variants2[vi].transform.parent.localPosition;
                    }

                    var sub0 = xf.Find("sub1");
                    RemoveAllChildren(sub0);

                    AddComponentIfNotAvailable<SpriteRenderer>(sub0.gameObject);
                    AddComponentIfNotAvailable<PolygonCollider2D>(sub0.gameObject);
                }

                // setup icon
                var icon_xf = xf.Find("icon");
                Vector3 icon_pos = icon_xf.localPosition;
                icon_xf.localPosition = new Vector3(icon_pos.x, icon_pos.y, -5f);
                icon_xf.GetComponent<SpriteRenderer>().sortingLayerName = "Icon";

                AddComponentIfNotAvailable<IconTouchHandler>(icon_xf.gameObject);
                AddComponentIfNotAvailable<BoxCollider2D>(icon_xf.gameObject);

                DestroyImmediate(ddi);

                div.Initialize();
                div.SetVariant(0);
            }
        }

        public void Decompress(GameObject items)
        {
            Transform parent = items.transform;

            parent.parent.gameObject.name = "dec_";

            //for (int item_idx = 0; item_idx < parent.childCount; item_idx++)
            //{
            //    var xf = parent.GetChild(item_idx);

            //    DesignItem di = xf.GetComponent<DesignItem>();

            //    if (xf.GetComponent<DesignItemTouchTrigger>())
            //        DestroyImmediate(xf.GetComponent<DesignItemTouchTrigger>());

            //    // replicate data
            //    var ddi = xf.gameObject.AddComponent<DecompressedDesignItem>();
            //    ddi.primaryData.displayName = di.displayName;
            //    ddi.primaryData.variantIndex = 0;
            //    ddi.primaryData.unlockedCountToUnlock = di.visibleUnlockCount;
            //    ddi.primaryData.costToUnlock = di.cost;

            //    if (di.effectType.type == ItemEffectType.InternalType.HorizontalBlendWithParticle)
            //        ddi.effectType = DesignItemEffectType.HorizontalBlendWithParticle;
            //    else if (di.effectType.type == ItemEffectType.InternalType.BounceScaleWithParticle)
            //        ddi.effectType = DesignItemEffectType.BounceScaleWithParticle;
            //    else if (di.effectType.type == ItemEffectType.InternalType.ParticleOnly)
            //        ddi.effectType = DesignItemEffectType.OnlyParticle;



            //    var icon_xf = xf.Find("icon");
            //    // remove icon box2d
            //    RemoveComponentIfAvailable<BoxCollider2D>(icon_xf.gameObject);
            //    // remove icon touch handler
            //    RemoveComponentIfAvailable<IconTouchHandler>(icon_xf.gameObject);
            //    // remove visual
            //    RemoveChildIfAvailable(xf, "visual");

            //    // add sub visual
            //    var subvisual = AddChild(xf.transform, "sub0", Vector3.zero);
            //    // add default
            //    if (di.defaultVariant.sprite)
            //    {
            //        AddChild(subvisual.transform, "default", di.defaultVariant.sprite, di.defaultVariant.offset);
            //        ddi.defaultSprite = di.defaultVariant.sprite;
            //    }        

            //    ddi.sprites = new Sprite[di.variantCollection.Length];
            //    // add vars
            //    for (int var_idx = 0; var_idx < di.variantCollection.Length; var_idx++)
            //    {
            //        AddChild(subvisual.transform, "var" + var_idx, di.variantCollection[var_idx].sprite, di.variantCollection[var_idx].offset);
            //        ddi.sprites[var_idx] = di.variantCollection[var_idx].sprite;
            //    }

            //    // remove design item component
            //    RemoveComponentIfAvailable<DesignItem>(xf.gameObject);
            //}
        }

        private GameObject AddChild(Transform parent, string name, Vector3 position, bool stayWorldPos = false)
        {
            GameObject var_child = new GameObject();
            var_child.name = name;
            var_child.transform.SetParent(parent, stayWorldPos);
            var_child.transform.localPosition = position;

            return var_child;
        }

        private GameObject AddChild(Transform parent, string name, Sprite sprite, Vector3 position, bool stayWorldPos = false)
        {
            GameObject var_child = new GameObject();
            var_child.name = name;
            var_child.transform.SetParent(parent, stayWorldPos);
            var_child.transform.localPosition = position;

            var sr = var_child.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            return var_child;
        }

        private void RemoveAllChildren(Transform xf)
        {
            int childs = xf.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                DestroyImmediate(xf.GetChild(i).gameObject);
            }
        }

        private T AddComponentIfNotAvailable<T>(GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (!comp)
                comp = go.AddComponent<T>();

            return comp;
        }

        private void RemoveComponentIfAvailable<T>(GameObject go) where T : Component
        {
            if (go.GetComponent<T>() != null)
                DestroyImmediate(go.GetComponent<T>());
        }

        private void RemoveChildIfAvailable(Transform parent, string childName)
        {
            var child = parent.Find(childName);
            if (child)
                DestroyImmediate(child.gameObject);
        }
    }
}


#endif
