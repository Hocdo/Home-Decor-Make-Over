
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decor
{
    public enum UITransitionEventId
    {
        ItemInfoOpen,
        ItemInfoUnlockConfirmed,
        ItemInfoUnlockFailed,
        ItemInfoClose,

        ShowVariant,
        ApplyOrExitVariant,

        ShowEdit,
        HideEdit,

        ShowRoomView,
        HideRoomView,

        ShowDialog,
        HideDialog
    }

    public class GameUITransitionDirector : MonoBehaviour
    {
        public VariantSelectingUIView variantSelectView;

        public SelectedItemInfoUIView selectedItemInfoView;

        public EditRoomUIView editRoomView;

        public DirectUIView directView;

        public RoomScrollPageView roomPageView;
        
        public Action<bool> SetActiveIconsVisible;

        public void ForceCloseAllView(bool flag)
        {
            if (flag)
                directView.Show();
            else
                directView.Hide(true);

            if (selectedItemInfoView.gameObject.activeSelf)
                selectedItemInfoView.Hide(true);
        }

        public void Awake()
        {
            variantSelectView.gameObject.SetActive(false);
            selectedItemInfoView.gameObject.SetActive(false);

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.ItemInfoOpen, (param) => 
            {                   
                directView.Hide(false);                  
                selectedItemInfoView.Show();               
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.ItemInfoClose, (param) =>
            {
                directView.Show();
                selectedItemInfoView.Hide(true);
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.ItemInfoUnlockConfirmed, (param) =>
            {
                selectedItemInfoView.Hide(false);
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.ShowVariant, (param) =>
            {
                if (editRoomView.isActiveAndEnabled)
                {
                    editRoomView.HideAllIconButtons();
                }
                else
                {
                    SetActiveIconsVisible?.Invoke(false);
                    directView.Hide(false);
                }
                
                variantSelectView.Show();
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.ApplyOrExitVariant, (param) =>
            {
                if (editRoomView.isActiveAndEnabled)
                { 
                    editRoomView.ShowAllIconButtons();
                    //this.ExecuteAfterSeconds(0.75f, () => editRoomView.ShowAllIconButtons());            
                }
                else
                {
                    SetActiveIconsVisible?.Invoke(true);
                    directView.Show();
                }                
                variantSelectView.Hide(true);
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.ShowEdit, (param) =>
            {
                directView.Hide(true);
                editRoomView.Show();
                SetActiveIconsVisible?.Invoke(false);
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.HideEdit, (param) =>
            {
                directView.Show();
                editRoomView.Hide();
                SetActiveIconsVisible?.Invoke(true);
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.ShowRoomView, (param) =>
            {
                roomPageView.gameObject.SetActive(true);
                directView.Hide(true);
                SetActiveIconsVisible?.Invoke(false);
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.HideRoomView, (param) =>
            {
                roomPageView.gameObject.SetActive(false);
                directView.Show();
                SetActiveIconsVisible?.Invoke(true);
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.ShowDialog, (param) =>
            {                
                directView.Hide(true);
                SetActiveIconsVisible?.Invoke(false);
            });

            EventDispatcher<UITransitionEventId>.Instance.RegisterEvent(UITransitionEventId.HideDialog, (param) =>
            {
                directView.Show();
                SetActiveIconsVisible?.Invoke(true);
            });
        }

        public void OnDestroy()
        {
            
        }
    } 
}

