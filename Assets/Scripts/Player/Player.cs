using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Player : NetworkBehaviour, IKitchenObject
{
    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnSpawnAnyPlayer;
    public static event EventHandler OnAnyPickUpSomething;

    public static void ResetStaticData()
    {
        OnSpawnAnyPlayer = null;
    }

    public event EventHandler<OnSelectedCounterChangeEventArgs> OnSelectedCounterChange;
    public event EventHandler OnPickUpSomething;
    public event EventHandler OnNotCounter;

    public TMP_Text gameNameText;

    public class OnSelectedCounterChangeEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float speed = 7f;
    [SerializeField] Animator myanimator;
    [SerializeField] PlayerInputs playerInputs;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private LayerMask collisionLayerMask;
    [SerializeField] Transform kitchenObjectPoint;
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;

    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    public bool isMoving;

    public string gameName;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
        transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        OnSpawnAnyPlayer?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }


    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void Start()
    {
        
        PlayerInputs.Instance.OnInteractAction += PlayerInputs_OnInteractAction;
        PlayerInputs.Instance.OnInteractAlternativeAction += PlayerInputs_OnInteractAlternativeAction;

        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));

        gameName = PlayerPrefs.GetString("GameName", "Jugador");  // "Jugador" es el valor por defecto si no hay datos guardados
        gameNameText.text = gameName;

    }


    private void PlayerInputs_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.isPlayingGame())
            return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void PlayerInputs_OnInteractAlternativeAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.isPlayingGame())
            return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        Movement();
        Interact();
    }

    void Interact()
    {
        Vector2 inputMovement = PlayerInputs.Instance.GetMovementNormalized();
        Vector3 moveDirection = new Vector3(inputMovement.x, 0, inputMovement.y);

        if (moveDirection != Vector3.zero)
        {
            lastInteractDirection = moveDirection;
        }

        float interactDistance = 2f;

        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, counterLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);                                  
                }            
            }
            else
            {
                OnNotCounter?.Invoke(this, EventArgs.Empty);
                SetSelectedCounter(null);
            }
        }
        else
        {
            OnNotCounter?.Invoke(this, EventArgs.Empty);
            SetSelectedCounter(null);
        }
    }

    void Movement()
    {
        Vector2 inputMovement = PlayerInputs.Instance.GetMovementNormalized();
        Vector3 moveDirection = new Vector3(inputMovement.x, 0f, inputMovement.y);

        float moveDistance = speed * Time.deltaTime;
        float playerRadius = .7f;

        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection, Quaternion.identity, moveDistance, collisionLayerMask);

        if (!canMove)
        {
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = (moveDirection.x < -.5f || moveDirection.x > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionLayerMask);

            if (canMove)
            {
                // Can move only on the X
                moveDirection = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = (moveDirection.z < -.5f || moveDirection.z > +.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionLayerMask);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDirection = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDirection * moveDistance;
        }

        if (!IsOwner)
        {
            return;
        }
        isMoving = moveDirection != Vector3.zero;
        myanimator.SetBool("IsWalking", moveDirection != Vector3.zero);

        if (moveDirection != Vector3.zero)
        {
            float rotationSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
        }
    }

    private void SetSelectedCounter(BaseCounter newSelectedCounter)
    {
        if (selectedCounter == newSelectedCounter) return;

        selectedCounter = newSelectedCounter;

        OnSelectedCounterChange?.Invoke(this, new OnSelectedCounterChangeEventArgs
        {
            selectedCounter = selectedCounter,
        }) ;
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickUpSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickUpSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
