using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class KitchenGameManager : NetworkBehaviour
{
    private Dictionary<ulong, bool> localPlayerReadyDictionary;

    public static KitchenGameManager Instance { get; private set; }
    public enum States {waitingToStart, CountDown, Playing, GameOver }

    private NetworkVariable <States> state  = new NetworkVariable<States>(States.waitingToStart);

    private NetworkVariable<float> countDownTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> playingTimer = new NetworkVariable<float>(0f);
    [SerializeField] private float playingTimerMax = 10f;

    [SerializeField] private Transform playerPrefab;

    private bool localPlayerReady = false;

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;
    public event EventHandler OnPlayingGame;
    public event EventHandler OnLocalPlayerReadyChanged;

    private bool isGamePaused;

    private void Awake()
    {
        Instance = this;
        localPlayerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        PlayerInputs.Instance.OnGamePaused += PlayerInputs_OnGamePaused;
        PlayerInputs.Instance.OnInteractAction += PlayerInputs_OnInteractAction;

    }

    

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void State_OnValueChanged(States previousValue, States newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
        
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong ClientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log("SpawnClients");
            Transform playerTransform = Instantiate(playerPrefab);
            NetworkObject networkObject = playerTransform.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(ClientId);
        }
    }

    private void PlayerInputs_OnInteractAction(object sender, EventArgs e)
    {

        if (state.Value == States.waitingToStart)
        {
            localPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerIsReadyServerRpc();

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIsReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        localPlayerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsAreReady = true;

        foreach(ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!localPlayerReadyDictionary.ContainsKey(clientID) || !localPlayerReadyDictionary[clientID])
            {
                allClientsAreReady = false;
                break;
            }
        }
        
        if (allClientsAreReady)
        {
            state.Value = States.CountDown;
        }

        Debug.Log("All clients ready" + allClientsAreReady);
    }

    private void PlayerInputs_OnGamePaused(object sender, EventArgs e)
    {
        if (isPlayingGame())
        {
            GamePaused();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(localPlayerReady);
        if (!IsServer)
        {
            return;
        }

        switch (state.Value)
        {
            case States.waitingToStart:
                break;
                

            case States.CountDown:
                countDownTimer.Value -= Time.deltaTime;
                if (countDownTimer.Value < 0f)
                    state.Value = States.Playing;
                playingTimer.Value = playingTimerMax;

                break;

            case States.Playing:
                OnPlayingGame?.Invoke(this, EventArgs.Empty);   
                playingTimer.Value -= Time.deltaTime;
                if (playingTimer.Value < 0f)
                    state.Value = States.GameOver;

                break;

            case States.GameOver:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
        Debug.Log(state);
    }

    public void GamePaused()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;  
            OnGamePaused?.Invoke(this, EventArgs.Empty);
            
        }
        if (!isGamePaused)
        {
            OnGameUnPaused?.Invoke(this, EventArgs.Empty);  
            Time.timeScale = 1.0f;
        }
    }

    public bool isPlayingGame()
    {
        return state.Value == States.Playing;
    }

    public bool isCountingDown()
    {
        return state.Value == States.CountDown;
    }

    public bool PlayerLocalReady()
    {
        return localPlayerReady;
    }

    public float GetCountDownTimer()
    {
        return countDownTimer.Value;
    }
    public bool isGameOver()
    {
        return state.Value == States.GameOver;
    }

    public bool isWaitingToStart()
    {
        return state.Value == States.waitingToStart;
    }

    public float GetGameTimerUI()
    {
        return 1 -(playingTimer.Value / playingTimerMax);
    }
}
