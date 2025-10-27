using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownStartUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownText;

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();
    }

    private void Update()
    {
        countDownText.text = Mathf.Ceil(KitchenGameManager.Instance.GetCountDownTimer()).ToString();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.isCountingDown())
        {
            Show();
        }
        else
        {
            Hide();
        }

    }

    private void Show()
    {
        countDownText.gameObject.SetActive(true);
    }

    private void Hide()
    {
        countDownText.gameObject.SetActive(false);

    }
}
