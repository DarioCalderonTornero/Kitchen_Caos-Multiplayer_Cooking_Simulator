using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGame;
    [SerializeField] private Button joinGame;

    private void Awake()
    {
        createGame.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelect);
        });

        joinGame.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.Instance.StartClient();
        });
    }

}
