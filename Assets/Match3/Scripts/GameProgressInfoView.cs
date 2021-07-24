using DG.Tweening;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
	public class GameProgressInfoView : MonoBehaviour
	{
		[Header("Goal")]
		public Transform GoalGroupTransform;

		public GameObject GoalSample;

		[Header("Move")]
		public Text TextLevel;

		public Text TextMoveCount;

		public GameObject throwingTarget;

		[Header("Bonus Coin")]
		public Transform bonusCoin;

        public Image bonusCoinImage;

        public Text bonusCoinText;

		private readonly float textTweenTime = 1.2f;

		private readonly float throwEffectTime = 1.5f;	

		private bool enableCreateCollectEffect = true;

		public Dictionary<CollectBlockType, CollectBlockPlayView> dicCollectObjs = new Dictionary<CollectBlockType, CollectBlockPlayView>();

        private BonusScaleAnimation bonusCoinAnim;

        void Start()
        {
            bonusCoinAnim = bonusCoin.GetComponent<BonusScaleAnimation>();
        }

        public void UpdateBonusCoin(int count, bool playBonusEffect = false)
        {
			bonusCoinText.text = count.ToString();

            if (playBonusEffect)
            {
                bonusCoinAnim.Trigger();
            }
        }

        public void OnFestivalStart()
        {
            foreach (var collectBlockPair in dicCollectObjs)
            {
                var collectBlockView = collectBlockPair.Value;
                collectBlockView.gameObject.SetActive(false);

                collectBlockView.targetImage.transform.DOKill();
                collectBlockView.targetImage.transform.DOScale(0f, 0.35f);

                collectBlockView.targetCountText.transform.DOKill();
                collectBlockView.targetCountText.transform.DOScale(0f, 0.35f);
            }

            bonusCoinImage.gameObject.SetActive(true);
            bonusCoinText.gameObject.SetActive(true);

            bonusCoin.localScale = Vector3.zero;
            bonusCoin.DOScale(1f, 0.35f).SetDelay(0.3f).SetEase(Ease.OutBack);

			//bonusCoinScaleCoroutine = StartCoroutine();

			//bonusCoinImage.transform.localScale = Vector3.zero;
			//bonusCoinImage.transform.DOScale(1f, 0.35f).SetDelay(0.3f);

			//bonusCoinText.transform.localScale = Vector3.zero;
			//bonusCoinText.transform.DOScale(0.6f, 0.35f).SetDelay(0.3f);
		}

		public virtual void Reset(MapDataCollectBlock[] collectBlocks, int gid)
		{
			TextLevel.text = gid.ToString();

			for (int j = 0; j < collectBlocks.Length; j++)
			{
				if (string.IsNullOrEmpty(collectBlocks[j].blockType) || collectBlocks[j].count <= 0 || !(GoalSample != null))
				{
					continue;
				}

				GameObject gameObject = (j == 0) ? GoalSample : UnityEngine.Object.Instantiate(GoalSample);
				if (!gameObject)
				{
					continue;
				}

				CollectBlockPlayView component = gameObject.GetComponent<CollectBlockPlayView>();
				if (!component)
				{
					continue;
				}

				if (!string.IsNullOrEmpty(collectBlocks[j].blockType) && collectBlocks[j].count > 0)
				{
					component.gameObject.transform.SetParent(GoalGroupTransform, worldPositionStays: false);
					CollectBlockType collectBlockType = collectBlocks[j].GetCollectBlockType();

					component.SetData(collectBlockType, collectBlocks[j].count,
						CollectBlockView.GetCollectSprite(collectBlockType, MapData.main.collectBlocks[j].blockType));
					dicCollectObjs[collectBlockType] = component;
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(component);
				}
			}

			foreach(var block in dicCollectObjs)
            {
				block.Value.UpdateSize();
            }

			MonoSingleton<AnimationController>.Instance.RegistGoalAnimation();
		}

		public void UpdateCollect(CollectBlockType collectBlockType, int count)
		{
			if (!dicCollectObjs.ContainsKey(collectBlockType))
			{
				return;
			}

			dicCollectObjs[collectBlockType].UpdateTargetCount(count);			

			if (base.gameObject.activeSelf && enableCreateCollectEffect)
			{
				enableCreateCollectEffect = false;
				Invoke("SetEnableCreateCollectEffect", 0.1f);
				GameObject spawnEffectObject = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.BringDownUIEffectDecrease);
				if ((bool)spawnEffectObject)
				{
					spawnEffectObject.transform.parent = dicCollectObjs[collectBlockType].targetImage.transform;
					spawnEffectObject.transform.localPosition = new Vector3(0f, 0f, -10f);
					PoolManager.PoolGameEffect.Despawn(spawnEffectObject.transform, 0.9f);
				}
			}
		}

		public void UpdateTextScore(int score)
        {
			//TextScore.text = score.ToString();
		}

		private void SetEnableCreateCollectEffect()
		{
			enableCreateCollectEffect = true;
		}

		public GameObject GetCollectObject(CollectBlockType collectBlockType)
        {
			if (dicCollectObjs.ContainsKey(collectBlockType))
			{
				return dicCollectObjs[collectBlockType].gameObject;
			}

			return null;
		}

		public Vector3 GetCollectObjectPosition(CollectBlockType collectBlockType)
		{
			if (dicCollectObjs.ContainsKey(collectBlockType))
			{
				if (dicCollectObjs[collectBlockType] == null)
				{
					return Vector3.zero;
				}
				return dicCollectObjs[collectBlockType].targetImage.transform.position;
			}
			if (GoalGroupTransform == null)
			{
				return Vector3.zero;
			}
			return GoalGroupTransform.position;
		}

		public IEnumerator BuyEffect(SpawnStringEffectType EffectType, GameObject effTargetObject, int NumOfItem, Vector3 startPos = default(Vector3))
		{
			GameObject effAddMove = SpawnStringEffect.GetSpawnEffectObject(EffectType);
			if (!effAddMove)
			{
				yield return null;
			}

			GameMain.main.throwingMoveEffectCount++;
			effAddMove.transform.position = Camera.main.transform.position;
			if (MapData.main.target == GoalTarget.Digging)
			{
				effAddMove.transform.position = startPos;
			}

			effAddMove.transform.localScale = Vector3.zero;
			effAddMove.GetComponentInChildren<SpriteRenderer>().gameObject.layer = LayerMask.NameToLayer("GameEffect");
			effAddMove.transform.DOScale(Vector3.one, 0.5f);

			PoolManager.PoolGameEffect.Despawn(effAddMove.transform, throwEffectTime + 0.5f);
			SoundSFX.Play(SFXIndex.BuyBooster);

			yield return new WaitForSeconds(0.5f);

			Ease xEaseThrowItem = Ease.InOutBack;
			Ease yEaseThrowItem = Ease.InOutCubic;

			Transform transform = effAddMove.transform;
			Vector3 gameCameraPosition = GetGameCameraPosition(effTargetObject);
			transform.DOMoveX(gameCameraPosition.x, throwEffectTime).SetEase(xEaseThrowItem);
			Transform transform2 = effAddMove.transform;
			Vector3 gameCameraPosition2 = GetGameCameraPosition(effTargetObject);
			transform2.DOMoveY(gameCameraPosition2.y, throwEffectTime).SetEase(yEaseThrowItem);

			ShortcutExtensions.DOScale(endValue: effAddMove.transform.localScale * 1.18f, target: effAddMove.transform, duration: throwEffectTime * 0.5f);
			effAddMove.transform.DOScale(0.5f, throwEffectTime * 0.5f).SetDelay(throwEffectTime * 0.5f);
			yield return new WaitForSeconds(throwEffectTime);

			SoundSFX.Play(SFXIndex.IncreaseBooster);
			switch (EffectType)
			{
			case SpawnStringEffectType.SuccessBuyMove:
				GameMain.main.IncreaseMoveCount(NumOfItem);
				if (NumOfItem == 5 || NumOfItem == 3)
				{
					GameMain.main.DoGameContinue();
				}
				break;
			case SpawnStringEffectType.SuccessBuyMagicHammer:
			case SpawnStringEffectType.SuccessBuyCandyPack:
			case SpawnStringEffectType.SuccessBuyBoosterHBomb:
			case SpawnStringEffectType.SuccessBuyBoosterVBomb:
			{
				BoosterType boosterType = BoosterType.Hammer;
				switch (EffectType)
				{
				case SpawnStringEffectType.SuccessBuyCandyPack:
					boosterType = BoosterType.CandyPack;
					break;
				case SpawnStringEffectType.SuccessBuyBoosterHBomb:
					boosterType = BoosterType.HBomb;
					break;
				case SpawnStringEffectType.SuccessBuyBoosterVBomb:
					boosterType = BoosterType.VBomb;
					break;
				}
				//MonoSingleton<PlayerDataManager>.Instance.IncreaseBoosterData(boosterType, NumOfItem, earnedBy);
				Match3GameUI.Instance.UpdateTextBoosterCount();
				break;
			}
			default:
				Match3GameUI.Instance.UpdateTextBoosterCount();
				break;
			}
			GameObject effect = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.UIEffectAddMove);
			if ((bool)effect)
			{
				effect.transform.SetParent(effTargetObject.transform);
				effect.transform.localPosition = new Vector3(0f, 0f, -10f);
				PoolManager.PoolGameEffect.Despawn(effect.transform, 1f);
			}
			GameObject effAdd1MoveText = SpawnStringEffect.GetSpawnEffectObject(SpawnStringEffectType.TextEffectAddStuffs);
			if ((bool)effAdd1MoveText)
			{
				effAdd1MoveText.GetComponent<Text>().text = "+" + NumOfItem;
				effAdd1MoveText.transform.SetParent(effTargetObject.transform);
				effAdd1MoveText.transform.localPosition = Vector3.zero;
				effAdd1MoveText.transform.localScale = Vector3.one;
				PoolManager.PoolGameEffect.Despawn(effAdd1MoveText.transform, textTweenTime);
				effAdd1MoveText.transform.DOLocalMoveY(30f, textTweenTime).SetEase(Ease.OutBack);
			}
			GameMain.main.throwingMoveEffectCount--;
		}

		private Vector3 GetGameCameraPosition(GameObject obj)
		{
			return GameMain.main.GameEffectCamera.ViewportToWorldPoint(GameMain.main.UIGameCamera.WorldToViewportPoint(obj.transform.position));
		}
	}
}
