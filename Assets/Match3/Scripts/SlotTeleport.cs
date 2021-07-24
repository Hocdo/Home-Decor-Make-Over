using UnityEngine;

public class SlotTeleport : MonoBehaviour
{
	private readonly float delay = 0.15f;

	private float lastTime = -10f;

	public Slot slot;

	public Slot target;

	public int targetID;

	private void Start()
	{
		slot = GetComponent<Slot>();
		Initialize();
	}

	private void Initialize()
	{
		int2 @int = ConvertIDtoPosition(targetID);
		target = BoardManager.main.GetSlot(@int.x, @int.y);
		target.teleportTarget = true;
	}

	private void Update()
	{
		if ((bool)target && GameMain.main.CanIGravity() && (bool)slot.GetChip() && slot.GetChip().canMove && !target.GetChip() && !slot.GetBlock() && !(slot.GetChip().transform.position != slot.transform.position) && !(lastTime + delay > Time.time))
		{
			lastTime = Time.time;
			AnimationAssistant.main.TeleportChip(slot.GetChip(), target);
		}
	}

	public static int2 ConvertIDtoPosition(int teleportID)
	{
		int2 result = default(int2);
		result.y = Mathf.FloorToInt(1f * (float)(teleportID - 1) / 12f);
		result.x = teleportID - 1 - result.y * 12;
		return result;
	}
}
