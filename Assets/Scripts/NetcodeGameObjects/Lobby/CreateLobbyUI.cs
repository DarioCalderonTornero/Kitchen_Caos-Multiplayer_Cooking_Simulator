using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    public static CreateLobbyUI Instance { get; private set; }  

    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_InputField nameLobbyInputField;

    public event EventHandler OnCloseButtonPressed;

    private void Awake()
    {
        Instance = this;

        Hide();
        createPublicButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(nameLobbyInputField.text, false);
        });

        createPrivateButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreateLobby(nameLobbyInputField.text, true);
        });

        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            Hide();
            OnCloseButtonPressed?.Invoke(this, EventArgs.Empty);
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        createPublicButton.Select();
    }
}
