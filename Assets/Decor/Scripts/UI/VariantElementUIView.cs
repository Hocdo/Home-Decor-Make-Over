using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariantElementUIView : MonoBehaviour
{
    public Button button;

    public Image image;

    public Image currencyImage;

    public Text costText;

    public void Setup(Sprite sprite, Currency currency)
    {
        if (currency != null && currency.type != CurrencyType.Star && currency.type != CurrencyType.None)
        {
            currencyImage.gameObject.SetActive(true);
            currencyImage.sprite = CurrencyDataTable.Instance.GetCurrencySprite(currency.type);

            if (currency.type != CurrencyType.Ads && currency.value > 0)
            {
                costText.gameObject.SetActive(true);
                costText.text = currency.value.ToString();
            }
            else
            {
                costText.gameObject.SetActive(false);
            }
        }
        else
        {
            currencyImage.gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
        }
       
        image.sprite = sprite;
        image.color = Color.white;

        RectTransform imageRectTransform = image.GetComponent<RectTransform>();
        RectTransform buttonRectTransform = button.GetComponent<RectTransform>();

        Vector2 spriteSize = sprite.bounds.size;
        float spriteSizeRatio = spriteSize.x / spriteSize.y;         
        Vector2 parentSize = buttonRectTransform.sizeDelta;
        Vector2 predictSize = new Vector2(parentSize.x, parentSize.x / spriteSizeRatio);

        if (predictSize.y > parentSize.y)
        {
            predictSize *= parentSize.y / predictSize.y;
        }

        imageRectTransform.sizeDelta = predictSize * 0.85f;
    }
}
