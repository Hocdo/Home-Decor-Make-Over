using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinButton : MonoBehaviour
{
    public Transform spinRotator;

    public float rotateSpeed = -30f;

    public GameObject warningObject;

    //void Update()
    //{
    //    spinRotator.Rotate(new Vector3(0f, 0f, rotateSpeed * Time.deltaTime), Space.Self);
    //}

    public void Start()
    {
        bool spinAvailable = SpinUtility.Available();

        warningObject.SetActive(spinAvailable);
    }

    public void ButtonPressed()
    {
        var popupSpin = Popup.PopupSystem.GetOpenBuilder().
                SetType(PopupType.PopupSpin).
                SetCurrentPopupBehaviour(Popup.CurrentPopupBehaviour.Close).
                Open<PopupSpin>();
    }
}
