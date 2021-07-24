using UnityEngine;
using UnityEngine.UI;

public class ButtonActiveGameobject : Button
{
	public GameObject ObjNormalState;

	public GameObject ObjDisabledState;

	protected override void Start()
	{
		base.Start();
		base.transition = Transition.None;
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		switch (state)
		{
		case SelectionState.Disabled:
			if ((bool)ObjNormalState)
			{
				ObjNormalState.SetActive(value: false);
			}
			if ((bool)ObjDisabledState)
			{
				ObjDisabledState.SetActive(value: true);
			}
			break;
		case SelectionState.Normal:
			if ((bool)ObjNormalState)
			{
				ObjNormalState.SetActive(value: true);
			}
			if ((bool)ObjDisabledState)
			{
				ObjDisabledState.SetActive(value: false);
			}
			break;
		}
	}
}
