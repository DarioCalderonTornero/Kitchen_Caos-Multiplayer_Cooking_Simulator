using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliverySuccesFailUI : MonoBehaviour
{

    [SerializeField] private Image backGround;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Sprite succesSprite;
    [SerializeField] private Sprite failSprite;
    [SerializeField] private Color succesColor;
    [SerializeField] private Color failColor;

    private void Start()
    {
        Hide();
        DelyveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DelyveryManager.Instance.OnRecipeFail += DelyveryManager_OnRecipeFail;
    }

    private void DelyveryManager_OnRecipeFail(object sender, System.EventArgs e)
    {
       

        Show();
        backGround.color = failColor;
        iconImage.sprite = failSprite;
        messageText.text = "DELIVERY\nFAIL";
        Invoke("Hide", 1f);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        Show();
        backGround.color = succesColor;
        iconImage.sprite = succesSprite;
        messageText.text = "DELIVERY\nSUCCESS";
        Invoke("Hide", 1f);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
