using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderCardUI : MonoBehaviour
{
    const float ORDER_CARD_UI_WIDTH = 35;
    const float ORDER_CARD_UI_HEIGHT = 100;

    [Tooltip("The main body transform of the card to enable/disable")]
    [SerializeField] RectTransform _card;
    [Tooltip("The image holding the product sprite")]
    [SerializeField] Image _productImage;
    [Tooltip("The images holding the ingredient sprites")]
    [SerializeField] Image[] _ingredientsImages;
    [Tooltip("The slider displaying the timer")]
    [SerializeField] Slider _timeSlider;

    public bool Active { get; private set; }

    void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Set the timer slider value
    /// </summary>
    /// <param name="time"></param>
    public void SetTimer(float time)
    {
        _timeSlider.value = time;
    }

    /// <summary>
    /// Display information of a given order
    /// </summary>
    /// <param name="order"></param>
    public void SetOrder(OrderSO order)
    {
        if (order == null)
        {
            Active = false;
            _card.gameObject.SetActive(false);
            return;
        }

        _card.gameObject.SetActive(true);
        Active = true;

        int ingredientNum = order.requestedItemRecipe.ingredients.Length;

        _card.sizeDelta = new Vector2(ORDER_CARD_UI_WIDTH * ingredientNum, ORDER_CARD_UI_HEIGHT);
        
        _timeSlider.maxValue = order.timeLimit;
        _timeSlider.value = _timeSlider.maxValue;
        _productImage.sprite = order.requestedItemRecipe.ProductItem.icon;

        for (int i = 0; i < _ingredientsImages.Length; i++)
        {
            if (i < ingredientNum)
            {
                _ingredientsImages[i].gameObject.SetActive(true);
                _ingredientsImages[i].sprite = order.requestedItemRecipe.ingredients[i].icon;
            }
            else
            {
                _ingredientsImages[i].sprite = null;
                _ingredientsImages[i].gameObject.SetActive(false);
            }
        }
    }
}
