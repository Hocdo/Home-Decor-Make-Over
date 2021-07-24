using UnityEngine;
using UnityEngine.UI;
using Popup;
public class PopupLiteTutorial : PopupBase
{
	private string[] tutorialDesc = new string[8]
	{
		"Try to match 3 or more same color Gems",
		"Match the Gems within the Lock to free them",
		"Match the Gems around the Box to break it",
		"Match the Gems on the Tile to break it",
		"Match the Gems on the Paint to spread it",
		"Bring the Blueprints down to the bottom of the board",
		"Find the Trowel behind the Tiles",
		"Match the Gems & find the Wrenchs!"
	};

	private string[] tutorialTitle = new string[0];

	public GameObject[] ObjTutorailGroups;

	public Text TextTutorialDesc;

    public void Start()
    {
        canClose = true;
    }

	public void SetData(int tutorialIndex)
	{
		ObjTutorailGroups[tutorialIndex].SetActive(value: true);
		TextTutorialDesc.text = tutorialDesc[tutorialIndex];
	}

    public override void Close(bool forceDestroying = true)
    {
		AcceptEvent?.Invoke(null);

		TerminateInternal(forceDestroying);
    }
}
