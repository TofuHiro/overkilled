using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderCardUI : MonoBehaviour
{
    [SerializeField] RectTransform _card;
    [SerializeField] Image _productImage;
    [SerializeField] Image[] _ingredientsImages;
    [SerializeField] Slider _timeSlider;

    public bool Active { get; private set; }


    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetTimer(float time)
    {
        _timeSlider.value = time;
    }

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

        _card.sizeDelta = new Vector2(35 * ingredientNum, 100);
        
        _timeSlider.maxValue = order.timeLimit;
        _timeSlider.value = _timeSlider.maxValue;
        _productImage.sprite = order.requestedItemRecipe.product.icon;

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
