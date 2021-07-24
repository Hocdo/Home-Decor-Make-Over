using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Decor
{
    public class RoomScrollPageView : MonoBehaviour
    {
        public float maxScale = 1f;

        public float minScale = 0.85f;

        public float distanceToMinScale = 100f;

        public LocalizationText roomNameText;

        public LocalizationText descriptionText;

        public Button enterButton;

        public Button exitButton;

        public HorizontalScrollSnap scroll;

        public Sprite unlockedCircleSprite;

        public ParticleSystem[] particleSystems;

        public Image lightImage;

        public GameObject sampleRoomPage;

        private RoomPageElement[] roomPages;

        private int selectedRoomId;

        private bool enabledWithAwake = true;

        private PlayRoomData playRoomData;

        private PlayerHomeDesignData playerHomeDesignData;

        private float targetAlpha;

        private float currentAlpha;

        public RoomPageElement GetRoomPageElement(int idx)
        {
            if (roomPages != null && idx >= 0 && idx < roomPages.Length)
                return roomPages[idx];

            return null;
        }

        public void Awake()
        {           
            playRoomData = Model.Instance.playRoomData;
            roomPages = new RoomPageElement[RoomDataTable.Instance.data.Length];

            for (int i = 0; i < roomPages.Length; i++)
            {
                var page = i == 0 ? sampleRoomPage : Instantiate(sampleRoomPage);
                page.transform.SetParent(sampleRoomPage.transform.parent, false);
                roomPages[i] = page.transform.GetChild(0).GetComponent<RoomPageElement>();
                roomPages[i].SetIndex(i);

                roomPages[i].PressAction = OnRoomPressed;
            }
            scroll.enabled = true;

            exitButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySFX(AudioClipId.ClickFailed);
                Exit();
            });

            enterButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySFX(AudioClipId.PanelIn);

                if (playRoomData.id == selectedRoomId)
                {
                    Exit();
                }
                else
                {
                    playRoomData.id = selectedRoomId;
                    Model.Instance.UpdatePlayerHomeDesignData();                
                    LoadSceneUtility.ReloadCurrentScene();                    
                }
            });

            //for (int i = 0; i < roomPages.Length; i++)
            //    roomPages[i].OnRoomSelected = OnSelectRoom;

            scroll.OnSelectionPageChangedEvent.RemoveAllListeners();
            scroll.OnSelectionPageChangedEvent.AddListener(OnSelectionPageChanged);
        }

        private void OnEnable()
        {
            RoomDataTable roomDataTable = RoomDataTable.Instance;
            Model.Instance.UpdatePlayerHomeDesignData();
            playerHomeDesignData = PlayerData.current.homeDesignData;

            for (int i = 0; i < roomPages.Length; i++)
            {
                RoomPageElement roomPage = roomPages[i];
                var roomData = roomDataTable.data[i];
                var unlockedRoomData = playerHomeDesignData.GetUnlockedRoomData(roomData.id);

                roomPage.SetRoomSprite(roomData.circleSprite);

                if (unlockedRoomData != null)
                {                   
                    roomPage.SetUnLocked(unlockedRoomData.boughtItemData.Count, roomData.maxItemCount, unlockedCircleSprite);
                }
                else
                {
                    roomPage.SetLocked();
                }

                if (roomData != null && roomData.id == playRoomData.id)
                {
                    scroll.StartingScreen = i;

                    if (enabledWithAwake)
                    {
                        OnSelectionPageChanged(scroll.StartingScreen);
                        enabledWithAwake = false;
                    }
                }
            }

            //this.ExecuteNextFrame(() =>
            //{
            //    for (int i = 0; i < roomPages.Length; i++)
            //    {
            //        RectTransform rectTf = roomPages[i].GetComponent<RectTransform>();
            //        rectTf.pivot = new Vector2(0.5f, 0.5f);
            //        rectTf.anchorMin = new Vector2(0.5f, 0.5f);
            //        rectTf.anchorMax = new Vector2(0.5f, 0.5f);
            //    }
            //});
        }

        private void OnRoomPressed(int idx)
        {
            scroll.ChangePage(idx);
        }

        private void OnSelectionPageChanged(int page)
        {
            AudioManager.Instance.PlaySFX(AudioClipId.ScrollRoomChage);

            Debug.Log("Selection Page Changed");
            RoomData roomData = RoomDataTable.Instance.data[page];

            bool showingComingSoonPage = false;
            if (!showingComingSoonPage)
            {
                roomNameText.ApplyTextWithKey(roomData.name);
                var unlockRoomData = playerHomeDesignData.GetUnlockedRoomData(roomData.id);
                
                if (unlockRoomData != null)
                {
                    targetAlpha = 1f;
                    for (int i = 0; i < particleSystems.Length; i++)                         
                        particleSystems[i].Play();

                    enterButton.gameObject.SetActive(true);
                    selectedRoomId = unlockRoomData.roomId;
                }
                else
                {
                    targetAlpha = 0f;
                    for (int i = 0; i < particleSystems.Length; i++)                      
                        particleSystems[i].Stop();

                    enterButton.gameObject.SetActive(false);
                }
            }
            else
            {
                roomNameText.GetComponent<Transform>().parent.gameObject.SetActive(false);
            }

            descriptionText.ApplyTextWithKey(roomData.description);
        }

        private void Exit()
        {
            EventDispatcher<UITransitionEventId>.Instance.NotifyEvent(UITransitionEventId.HideRoomView);
        }

        private void LateUpdate()
        {
          
            float sign = Mathf.Sign(targetAlpha - currentAlpha);
            
            currentAlpha += sign * Time.deltaTime * 4f;

            for (int i = 0; i < roomPages.Length; i++)
            {
                Transform tf = roomPages[i].transform;

                float scale = Mathf.Clamp(0, (distanceToMinScale - Mathf.Abs(tf.position.x)) / distanceToMinScale, 1f) * (maxScale - minScale) + minScale;
                tf.localScale = new Vector3(scale, scale, scale);
            }

            lightImage.color = new Color(1f, 1f, 1f, currentAlpha);            
        }
    }

}

