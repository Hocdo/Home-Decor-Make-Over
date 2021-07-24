using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoSingleton<AnimationController>
{
	public enum IdleAnimationType
	{
		None,
		Pause,
		Crow,
		MagicalCrow,
		ChocolateJail,
		PastryBag,
		RescueFriend,
		Chameleon,
		ColorHVBomb,
		SimpleBomb,
		Rainbow,
		SmallCandy,
		GingermanUnderTile,
		MIlk,
		SweetRoadExit,
		BringdownEntryExit,
		RailPortal,
		BoardLine,
		Hint
	}

	public enum QueueType
	{
		Loop,
		Goal
	}

	public static readonly string IdleAnimationName = "SetIdle";

	public static readonly string IdleStopAnimationName = "StopIdle";

	public static readonly string NoAnimationName = "NoMotion";

	public static readonly float AnimationTerm = 0.1f;

	public static float hintTime = 2.4f;

	public static float waitingNextAnimationTime = 3f;

	private readonly Queue<KeyValuePair<IdleAnimationType, AnimationProperty>> animationQueueGoal = new Queue<KeyValuePair<IdleAnimationType, AnimationProperty>>();

	private readonly Queue<KeyValuePair<IdleAnimationType, AnimationProperty>> animationQueueLoop = new Queue<KeyValuePair<IdleAnimationType, AnimationProperty>>();

	public Dictionary<IdleAnimationType, float> dicAnimationTimeOfType = new Dictionary<IdleAnimationType, float>
	{
		{
			IdleAnimationType.Pause,
			0f
		},
		{
			IdleAnimationType.Crow,
			10.767f
		},
		{
			IdleAnimationType.MagicalCrow,
			11.15f
		},
		{
			IdleAnimationType.ChocolateJail,
			1.5f
		},
		{
			IdleAnimationType.PastryBag,
			1f
		},
		{
			IdleAnimationType.RescueFriend,
			2f
		},
		{
			IdleAnimationType.Chameleon,
			1f
		},
		{
			IdleAnimationType.ColorHVBomb,
			1.5f
		},
		{
			IdleAnimationType.SimpleBomb,
			1f
		},
		{
			IdleAnimationType.Rainbow,
			3f
		},
		{
			IdleAnimationType.SmallCandy,
			5f
		},
		{
			IdleAnimationType.GingermanUnderTile,
			2f
		},
		{
			IdleAnimationType.MIlk,
			2f
		},
		{
			IdleAnimationType.SweetRoadExit,
			2.483f
		},
		{
			IdleAnimationType.BringdownEntryExit,
			1f
		},
		{
			IdleAnimationType.RailPortal,
			1f
		},
		{
			IdleAnimationType.BoardLine,
			2f
		}
	};

	private readonly List<IdleAnimationType> listLoopAnimation = new List<IdleAnimationType>();

	private void Start()
	{
		RegistAnimation(IdleAnimationType.ColorHVBomb, QueueType.Loop);
		RegistAnimation(IdleAnimationType.SimpleBomb, QueueType.Loop);
		RegistAnimation(IdleAnimationType.SmallCandy, QueueType.Loop);
		RegistAnimation(IdleAnimationType.Rainbow, QueueType.Loop);
		RegistAnimation(IdleAnimationType.Chameleon, QueueType.Loop);
		RegistAnimation(IdleAnimationType.BoardLine, QueueType.Loop);
	}

	public void ClearAnimationQueueGoalAndStopAllCoroutines()
    {
		StopAllCoroutines();

		animationQueueGoal.Clear();
	}

	public void RegistGoalAnimation()
	{
		using (Dictionary<CollectBlockType, CollectBlockPlayView>.KeyCollection.Enumerator enumerator = Match3GameUI.Instance.progressInfoView.dicCollectObjs.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case CollectBlockType.RescueGingerMan:
					RegistAnimation(IdleAnimationType.GingermanUnderTile, QueueType.Goal, 2, 0f, 5);
					break;
				case CollectBlockType.RescueFriend:
					RegistAnimation(IdleAnimationType.RescueFriend, QueueType.Goal, 2, 0f, 5);
					break;
				case CollectBlockType.ChocolateJail:
					RegistAnimation(IdleAnimationType.ChocolateJail, QueueType.Goal, 2, 0f, 5);
					break;
				case CollectBlockType.Crow:
					RegistAnimation(IdleAnimationType.Crow, QueueType.Goal, 1, 0f, 5);
					break;
				case CollectBlockType.MagicalCrow:
					RegistAnimation(IdleAnimationType.MagicalCrow, QueueType.Goal, 1, 0f, 5);
					break;
				}
				IdleAnimationType aniType = IdleAnimationType.Pause;
				QueueType queuetype = QueueType.Goal;
				float pauseTIme = waitingNextAnimationTime;
				RegistAnimation(aniType, queuetype, 1, pauseTIme);
				aniType = IdleAnimationType.Pause;
				queuetype = QueueType.Goal;
				pauseTIme = waitingNextAnimationTime;
				RegistAnimation(aniType, queuetype, 1, pauseTIme);
			}
		}
	}

	private void RegistAnimation(IdleAnimationType aniType, QueueType queuetype, int count = 1, float pauseTIme = 0f, int numOfAniObj = -1)
	{
		AnimationProperty animationProperty = new AnimationProperty();
		if (queuetype == QueueType.Loop)
		{
			animationProperty.isLoop = true;
			listLoopAnimation.Add(aniType);
		}
		animationProperty.time = ((aniType != IdleAnimationType.Pause) ? (dicAnimationTimeOfType[aniType] * (float)count) : pauseTIme);
		animationProperty.numOfAniObj = numOfAniObj;
		animationProperty.numOfAni = count;
		GetQueueByOrder(queuetype).Enqueue(new KeyValuePair<IdleAnimationType, AnimationProperty>(aniType, animationProperty));
	}

	public bool IsRegistedLoopAnimation(IdleAnimationType type)
	{
		if (listLoopAnimation.Contains(type))
		{
			return true;
		}
		return false;
	}

	public void ProcessAnimation()
	{
		StartCoroutine(ProcessAnimationByQueueOrder(QueueType.Goal));
	}

	private IEnumerator ProcessAnimationByQueueOrder(QueueType queuetype)
	{
		yield return StartCoroutine(Utils.WaitFor(GameMain.main.CanIWait, 1f));
		while (true)
		{
			Queue<KeyValuePair<IdleAnimationType, AnimationProperty>> queueByOrder = GetQueueByOrder(queuetype);
			foreach (KeyValuePair<IdleAnimationType, AnimationProperty> pair in queueByOrder)
			{
				if (pair.Value != null)
				{
					if (pair.Key != IdleAnimationType.Pause)
					{
						PlayAnimation(pair);
					}
					yield return new WaitForSeconds(pair.Value.time);
					pair.Value.aniCount = 0;
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void PlayAnimation(KeyValuePair<IdleAnimationType, AnimationProperty> pair)
	{
		if (GameMain.main.isGameResult || !BoardManager.main.slotGroup || pair.Value == null)
		{
			return;
		}
		AnimationObject[] array = null;
		if (pair.Key == IdleAnimationType.Hint)
		{
			return;
		}
		if (pair.Key == IdleAnimationType.GingermanUnderTile)
		{
			array = BoardManager.main.slotGroup.GetComponentsInChildren<AnimationGingermanUnderTile>();
		}
		else if (pair.Key == IdleAnimationType.RescueFriend)
		{
			array = BoardManager.main.slotGroup.GetComponentsInChildren<RescueFriend>();
		}
		else if (pair.Key == IdleAnimationType.ChocolateJail)
		{
			array = BoardManager.main.slotGroup.GetComponentsInChildren<ChocolateJail>();
		}
		else if (pair.Key == IdleAnimationType.Crow)
		{
			array = BoardManager.main.slotGroup.GetComponentsInChildren<Crow>();
		}
		else if (pair.Key == IdleAnimationType.MagicalCrow)
		{
			array = BoardManager.main.slotGroup.GetComponentsInChildren<MagicalCrow>();
		}
		if (array == null || array.Length <= 0)
		{
			return;
		}
		if (pair.Value.numOfAniObj == -1)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					array[i].PlayIdleAnimation(pair.Key, pair.Value);
				}
			}
			return;
		}
		List<int> list = new List<int>();
		for (int j = 0; j < pair.Value.numOfAniObj; j++)
		{
			int num = Random.Range(0, array.Length);
			while (list.Contains(num))
			{
				num = Random.Range(0, array.Length);
			}
			list.Add(num);
			array[num].PlayIdleAnimation(pair.Key, pair.Value);
			if (list.Count == array.Length)
			{
				break;
			}
		}
	}

	private Queue<KeyValuePair<IdleAnimationType, AnimationProperty>> GetQueueByOrder(QueueType queuetype)
	{
		switch (queuetype)
		{
		case QueueType.Loop:
			return animationQueueLoop;
		case QueueType.Goal:
			return animationQueueGoal;
		default:
			return animationQueueLoop;
		}
	}
}
