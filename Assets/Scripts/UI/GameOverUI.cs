using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredNumber;
    [SerializeField] private Image backGroundImage;
    [SerializeField] private TextMeshProUGUI gameOverRecipesDelivered;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button retryButton;

    [SerializeField] private TextMeshProUGUI retryStatusText;

    //private GameEnd_php gameEnd;
    //private Player player;
    //private string username;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenu);
        });

        retryButton.onClick.AddListener(() =>
        {
            retryButton.interactable = false;
            KitchenGameManager.Instance.SetPlayerRetry();
        });
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        KitchenGameManager.Instance.OnRetryChanged += KitchenGameManager_OnRetryChanged;
        Hide();
    }

    private void KitchenGameManager_OnRetryChanged(object sender, System.EventArgs e)
    {
        UpdateRetryText();
    }

    private void UpdateRetryText()
    {
        int totalCount = 0;
        int readyCount = 0; 

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            totalCount++;
            if (KitchenGameManager.Instance.HasPlayerPressedRetry(clientId))
            {
                readyCount++;
            }
        }

        retryStatusText.text = $"Retry: {readyCount}/{totalCount}";
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.isGameOver())
        {
            Show();

            // Obtener las recetas entregadas por el cliente actual
            ulong clientId = NetworkManager.Singleton.LocalClientId;
            int recipesDelivered = DelyveryManager.Instance.GetPlayerSuccessfulRecipes(clientId);

            recipesDeliveredNumber.text = recipesDelivered.ToString();

            //username = PlayerPrefs.GetString("Username", username);
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        retryButton.interactable = true;
        UpdateRetryText();
        recipesDeliveredNumber.gameObject.SetActive(true);
        backGroundImage.gameObject.SetActive(true);
        gameOverRecipesDelivered.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);      
        retryStatusText.gameObject.SetActive(true);  
        retryButton.Select();
    }

    private void Hide()
    {
        recipesDeliveredNumber.gameObject.SetActive(false);
        backGroundImage.gameObject.SetActive(false);
        gameOverRecipesDelivered.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        retryStatusText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
        KitchenGameManager.Instance.OnRetryChanged -= KitchenGameManager_OnRetryChanged;
    }
}
