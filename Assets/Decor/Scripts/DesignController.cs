using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decor
{
    // Business decor logic - change variant, buy item, ...
    public class DesignController : MonoBehaviour
    {
        public VariantSelectingUIView variantSelectView;

        public SelectedItemInfoUIView selectedItemInfoView;

        public CurrencyUIView currencyView;

        public EditRoomUIView editRoomView;

        public DesignItemEffectController itemEffectHandler;

        private List<DesignItemView> remainingItems = new List<DesignItemView>();

        private List<DesignItemView> unlockedItems = new List<DesignItemView>();

        private List<GameObject> activeIconList = new List<GameObject>();

        private DesignItemView selectedItem;

        private int selectedItemVariantIndex;

        public Action<DesignItemView> UnlockItemFinishEvent;

        public Action<DesignItemView> UnlockItemFailedEvent;

        public string roomName;

        private float iconScale = 1f;

        public bool IsTouchEnabled { get; set; }

        public bool IsAllItemsUnlock()
        {
            return remainingItems.Count == 0;
        }

        public void Initialize(DesignItemView[] items)
        {
            if (items.Length > 0) 
                iconScale = items[0].GetIcon().transform.localScale.x;

            for (int i = 0; i < items.Length; i++)
            {
                DesignItemView designItem = items[i];
                designItem.GetIcon().SetActive(false);

                if (designItem.primaryData.IsUnlocked())
                {
                    unlockedItems.Add(designItem);
                    designItem.SetVariant(designItem.primaryData.variantIndex);
                    designItem.UpdatePhysicsShape();                    
                }
                else
                {
                    if (designItem.HasDefaultVariant())
                    {
                        designItem.SetVariant(DesignItemView.DefaultVariantIndex);
                        designItem.UpdatePhysicsShape();
                    }
                    else
                    {
                        designItem.SetVisualActive(false);
                    }

                    remainingItems.Add(designItem);
                }

                designItem.GetComponent<DesignItemTouchTrigger>().touchAction = () => OnDesignItemTouched(designItem);
                designItem.GetIcon().GetComponent<IconTouchHandler>().OnPointerClickAction = () => OnDesignIconTouched(designItem);
            }

            UpdateVisibleIcon();

            currencyView.stamina = PlayerData.current.stamina;

            selectedItemInfoView.UnlockCondition = ItemUnlockCondition;
            selectedItemInfoView.UnlockConfirmEvent = ItemUnlockConfirmed;
            selectedItemInfoView.PlayDecreaseCoinEffectEvent = currencyView.PlayChangeCoinAnim;

            variantSelectView.ShowEvent = ShowItemVariant;
            variantSelectView.ChangeEvent = ChangeItemVariant;
            variantSelectView.ApplyPredicateEvent = ApplyItemVariantPredicate;
            variantSelectView.ApplyValidateEvent = ApplyItemVariantValidate;
            variantSelectView.ExitEvent = ExitItemVariant;

            editRoomView.Initialize(items);
            editRoomView.IconClickEvent = OnDesignItemTouched;
        }

        public void Clear()
        {

        }

        public int GetUnlockedItemCount()
        {
            return unlockedItems.Count;
        }

        #region EventCallback
        public void OnDesignItemTouched(DesignItemView item)
        {
            if (IsTouchEnabled == false) return;

            selectedItem = item;
            selectedItemVariantIndex = selectedItem.primaryData.variantIndex;

            if (item.primaryData.IsUnlocked())
            {
                AudioManager.Instance.PlaySFX(AudioClipId.ItemSelect);

                EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.ShowVariant);                   
            }
        }

        public void OnDesignIconTouched(DesignItemView item)
        {
            if (IsTouchEnabled == false) return;

            selectedItem = item;
            selectedItemVariantIndex = selectedItem.primaryData.variantIndex;

            AudioManager.Instance.PlaySFX(AudioClipId.ItemSelect);

            #region SetupData
            string displayName = item.primaryData.displayName;
            int costValue = item.primaryData.costToUnlock.value;
            Sprite currencySprite = CurrencyDataTable.Instance.GetCurrencySprite(item.primaryData.costToUnlock.type);
            Vector2 position = item.GetIcon().transform.position;
            #endregion

            selectedItemInfoView.Setup(displayName, costValue, currencySprite, position);
            EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.ItemInfoOpen);
        }

        void ItemUnlockConfirmed()
        {
            PlayerData playerData = PlayerData.current;
            
            int costInCoin = selectedItem.primaryData.costToUnlock.value;
            if (playerData.cointCount >= costInCoin 
                && selectedItem.primaryData.IsUnlocked() == false) // not necessary
            {
                playerData.AddCoin(-costInCoin);
                EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.CoinChange, playerData.cointCount); //  not necessary
                
                UpdateVisibleIcon(selectedItem);

                GlobalEventObserver.InvokeDecorUnlockEvent();
            }
        }

        bool ItemUnlockCondition()
        {
            int costInCoin = selectedItem.primaryData.costToUnlock.value;
            if (PlayerData.current.cointCount >= costInCoin)
            {
                return true;
            }
            else
            {
                UnlockItemFailedEvent?.Invoke(selectedItem);
            }

            return false;
        }

        void ShowItemVariant()
        {
            #region SetupData
            AssetBundle roomAssetBundle = RoomDataTable.Instance.GetRoomDataWithId(Model.Instance.playRoomData.id).GetAssetBundle();
            int variantCount = selectedItem.GetVariantCount();

            Sprite[] variantSprites = new Sprite[variantCount];
            Currency[] currencies = new Currency[variantCount];
            int[] costs = new int[variantCount];

            for (int i = 0; i < variantCount; i++)
            {
                variantSprites[i] = roomAssetBundle.LoadAsset<Sprite>(selectedItem.primaryData.id + "_" + (i + 1) + "_mini");

                if (variantSprites[i] == null)
                {
                    variantSprites[i] = roomAssetBundle.LoadAsset<Sprite>(selectedItem.primaryData.id + "_" + (i + 1));
                }

                if (i < selectedItem.primaryData.variantCosts.Length && !selectedItem.primaryData.IsVariantUnlocked(i))
                {
                    currencies[i] = selectedItem.primaryData.variantCosts[i];
                }
                else
                {
                    currencies[i] = null;
                }
            }
            int variantIndexOnShow = (selectedItem.primaryData.variantIndex == -1) ? 0 : selectedItem.primaryData.variantIndex;
            #endregion

            variantSelectView.Setup(selectedItem.primaryData, variantSprites, currencies, selectedItem.GetIcon().transform.position, selectedItem.primaryData.IsUnlocked());
            StartCoroutine(variantSelectView.SelectVariantCoroutine(variantIndexOnShow));

            selectedItem.SetVisualActive(true);

            itemEffectHandler.SetTarget(selectedItem.GetSpriteRenderers(), selectedItem.effectType);
            itemEffectHandler.PlayOnSelect();
        }

        void ChangeItemVariant(int variantIndex)
        {
            if (variantIndex != selectedItem.primaryData.variantIndex && selectedItem.primaryData.variantIndex != -1)
                AudioManager.Instance.PlaySFX(AudioClipId.ItemSelect);

            selectedItem.SetVariant(variantIndex);            

            itemEffectHandler.PlayOnChange();
        }

        bool ApplyItemVariantPredicate()
        {
            var itemData = selectedItem.primaryData;

            if (itemData.IsVariantUnlocked(itemData.variantIndex))
            {
                return true;
            }
            else
            {
                Currency cost = itemData.variantCosts[itemData.variantIndex];

                if (cost.type == CurrencyType.Ads && AdvertisementManager.Instance.IsRewardedVideoAvailable)
                {
                    return true;
                }
                else if (cost.type == CurrencyType.Gem && PlayerData.current.gemCount >= cost.value)
                {
                    return true;
                }
                else
                    return true;
            }
        }

        void ApplyItemVariantValidate(Action response)
        {
            var item = selectedItem; // create a local variable for delegate callback
            var itemData = item.primaryData;
            
            Action SucessApplyEvent = () =>
            {
                AudioManager.Instance.PlaySFX(AudioClipId.ItemApply);
                itemEffectHandler.PlayOnApply();

                item.UpdatePhysicsShape();
                itemData.UnlockVariant(item.primaryData.variantIndex);

                response?.Invoke();

               // Analytics.Feature_DECORATION.ACTION_NAME actionName = Analytics.Feature_DECORATION.ACTION_NAME._swap_decoration;

                if (itemData.IsUnlocked() == false)
                {                   
                    UnlockItem(item);
                    UnlockItemFinishEvent?.Invoke(item);

                   // actionName = Analytics.Feature_DECORATION.ACTION_NAME._buy_decoration;
                }

                selectedItem = null;
            };

            if (itemData.IsVariantUnlocked(itemData.variantIndex))
            {
                SucessApplyEvent();
            }
            else
            {
                Currency cost = itemData.variantCosts[itemData.variantIndex];

                if (cost.type == CurrencyType.Ads && AdvertisementManager.Instance.IsRewardedVideoAvailable)
                {
                    AdvertisementManager.Instance.ShowRewardedVideo(SucessApplyEvent);
                }
                else if (cost.type == CurrencyType.Gem && PlayerData.current.gemCount >= cost.value)
                {
                    PlayerData.current.AddGem(-cost.value);
                    EventDispatcher<GlobalEventId>.Instance.NotifyEvent(GlobalEventId.GemChange, PlayerData.current.gemCount);

                    SucessApplyEvent();
                }
                else if (cost.type == CurrencyType.None)
                {
                    SucessApplyEvent();
                }
            }

           
        }

        void ExitItemVariant()
        {
            itemEffectHandler.PlayOnDeselect();

            selectedItem.SetVariant(selectedItemVariantIndex);
            selectedItem = null;         
        }
        #endregion

        #region ControlMethod
        public void UpdateVisibleIcon(DesignItemView item = null)
        {
            if (item == null)
            {
               
                for (int i = 0; i < remainingItems.Count; i++)
                {
                    
                    //if (activeIconList.Count < 3)
                    //{
                    //    var activeIcon = remainingItems[i].GetIcon();
                    //    if (activeIconList.Contains(activeIcon) == false)
                    //    {
                    //        activeIcon.SetActive(true);
                    //        activeIconList.Add(activeIcon);
                    //    }
                    //}

                    var activeIcon = remainingItems[i].GetIcon();
                    if (activeIconList.Count < 4 && activeIconList.Contains(activeIcon) == false
                        && remainingItems[i].IsDependenciesUnlocked()
                        && CheckPreviousItemUnlocked(remainingItems[i] )
                        /*&& remainingItems[i].GetUnlockedCountToUnlock() <= unlockedItems.Count*/)
                    {
                        activeIcon.SetActive(true);
                        activeIconList.Add(activeIcon);
                    }
                }
            }
            else
            {
               
                for (int i = 0; i < remainingItems.Count; i++)
                {
                    var activeIcon = remainingItems[i].GetIcon();
                    if (activeIconList.Count <= 4 && activeIconList.Contains(activeIcon) == false
                        && remainingItems[i].IsDependenciesUnlocked(item)
                        //&& CheckPreviousItemUnlocked(remainingItems[i])
                        )
                    {
                        if (remainingItems[i].previousId == item.GetId())
                        {
                            activeIcon.SetActive(true);
                            activeIconList.Add(activeIcon);
                        }
                        else if (CheckPreviousItemUnlocked(remainingItems[i]))
                        {
                            activeIcon.SetActive(true);
                            activeIconList.Add(activeIcon);
                        }
                    }
                }
            }
        }
        public bool CheckPreviousItemUnlocked(DesignItemView item )
        {
            if (string.IsNullOrEmpty(item.previousId)) return true;
            
            for(int i=0; i < remainingItems.Count; i++)
            {
                if (remainingItems[i].primaryData.id.Equals(item.previousId)) return false;
            }
            return true;
        }
        public void SetActiveIconsVisibleWithAnim(bool active)
        {
            SetActiveIconsVisible(active, true);
        }

        public void SetActiveIconsVisible(bool active, bool fadeAnim = true)
        {
            int count = activeIconList.Count;
            if (fadeAnim)
            {
                if (active)
                {
                    for (int i = 0; i < count; i++)
                    {
                        activeIconList[i].SetActive(true);

                        SpriteRenderer spriteRenderer = activeIconList[i].GetComponent<SpriteRenderer>();
                        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
                        spriteRenderer.DOKill();
                        spriteRenderer.DOFade(1f, 0.25f).SetEase(Ease.InSine);

                        Transform xf = activeIconList[i].transform;
                        xf.DOKill();
                        xf.DOScale(iconScale, 0.25f).SetEase(Ease.OutBack);
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        SpriteRenderer spriteRenderer = activeIconList[i].GetComponent<SpriteRenderer>();
                        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
                        spriteRenderer.DOKill();
                        spriteRenderer.DOFade(0f, 0.25f).SetEase(Ease.InSine);

                        Transform xf = activeIconList[i].transform;
                        xf.DOKill();
                        xf.DOScale(0f, 0.15f).SetEase(Ease.InSine)
                            .OnComplete(() => xf.gameObject.SetActive(false));
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    activeIconList[i].SetActive(active);
                }
            }
        }

        public void UnlockItem(DesignItemView item)
        {
            item.SetVisualActive(true);

            remainingItems.Remove(item);
            unlockedItems.Add(item);

            var activeIcon = item.GetIcon();
            activeIcon.SetActive(false);
            activeIconList.Remove(activeIcon);

            item.primaryData.Unlock();        
        }
        #endregion
    }
}

