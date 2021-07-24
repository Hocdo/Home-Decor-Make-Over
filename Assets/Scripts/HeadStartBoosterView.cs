using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadStartBoosterView : MonoBehaviour
{
    public Image iconImage;

    public Image countBgImage;

    public Text countText;

    public Image tickButton;

    public GameObject costGameObject;
    
    private bool available = true;

    public bool IsAvailable() { return available; }

    public void Disable(Material disableMaterial)
    {
        iconImage.material = disableMaterial;
        var img =  this.GetComponent<Image>();

        if (img != null)
        {
            img.material = disableMaterial;
        }
        if (iconImage.transform.childCount > 0)
        {
            var childImage = iconImage.transform.GetChild(0).GetComponent<Image>();
            if (childImage != null)
                childImage.material = disableMaterial;
        }

        countBgImage.gameObject.SetActive(false);
        countText.gameObject.SetActive(false);
        tickButton.gameObject.SetActive(false);
        costGameObject.gameObject.SetActive(false);

        available = false;
    }

    public void SetCount(int count)
    {
        if (count > 0)
        {
            countText.text = count.ToString();

            countBgImage.gameObject.SetActive(true);
            tickButton.gameObject.SetActive(false);
            costGameObject.gameObject.SetActive(false);
        }
        else
        {
            countBgImage.gameObject.SetActive(false);
            tickButton.gameObject.SetActive(false);
            costGameObject.gameObject.SetActive(true);
        }
    }

    public void SetSelected(bool flag)
    {
        if (flag)
        {
            countBgImage.gameObject.SetActive(false);
            tickButton.gameObject.SetActive(true);
        }
        else
        {
            countBgImage.gameObject.SetActive(true);
            tickButton.gameObject.SetActive(false);
        }
    }
}
