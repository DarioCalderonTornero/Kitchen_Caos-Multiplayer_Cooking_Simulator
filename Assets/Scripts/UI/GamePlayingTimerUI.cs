using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingTimerUI : MonoBehaviour
{
    [SerializeField] public Image timerImage;

    private void Awake()
    {
        timerImage.fillAmount = 0;
    }
    private void Update()
    {
        timerImage.fillAmount = KitchenGameManager.Instance.GetGameTimerUI();
    }
}
