using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button unReadyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;


    private void Awake()
    {
        unReadyButton.gameObject.SetActive(false);

        mainMenuButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenu);
        });

        readyButton.onClick.AddListener(() =>
        {
            TestingCharacterSelected.Instance.SetPlayerReady();
            readyButton.gameObject.SetActive(false);
            unReadyButton.gameObject.SetActive(true);      
            unReadyButton.Select();
        });

        unReadyButton.onClick.AddListener(() =>
        {
            TestingCharacterSelected.Instance.SetPlayerUnready();
            readyButton.gameObject.SetActive(true);
            unReadyButton.gameObject.SetActive(false);
            readyButton.Select();
        });
    }

    private void Start()
    {
        Lobby lobby = KitchenGameLobby.Instance.GetLobby();
        lobbyNameText.text = "LobbyName: " + lobby.Name;
        lobbyCodeText.text = "LobbyCode: " + lobby.LobbyCode;
    }

}
