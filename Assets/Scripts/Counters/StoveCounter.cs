using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{

    public event EventHandler<IHasProgress.OnProgressChangedEventsArgs> OnProgressChanged;

    public event EventHandler <OnchangeStateEventArgs> OnStateChanged; 
    public class OnchangeStateEventArgs : EventArgs
    {
        public State state;
    }

    public event EventHandler OnIdle;
    public event EventHandler Onfrying;
    public event EventHandler OnFried;
    public event EventHandler OnBurned;

    [SerializeField] private FryingObjectSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burnedRecipeSOArray;


    public enum State { idle, frying, fried, burned}

    public NetworkVariable <float> fryingTimer = new NetworkVariable<float>(0f);
    public NetworkVariable<float> burnedTimer = new NetworkVariable<float>(0f);
    private FryingObjectSO fryingObjectSO;
    private BurningRecipeSO burningRecipeSO;
    private NetworkVariable <State> state = new NetworkVariable<State>(State.idle);

    

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burnedTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingObjectSO != null ? fryingObjectSO.fryTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventsArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        });;
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = burningRecipeSO != null ? burningRecipeSO.burnedTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventsArgs
        {
            progressNormalized = burnedTimer.Value / burningTimerMax
        });
    }

    private void State_OnValueChanged(State previousState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnchangeStateEventArgs
        {
            state = state.Value
        });

        if (state.Value == State.burned || state.Value == State.idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventsArgs
            {
                progressNormalized = 0f
            });
        }
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (HasKitchenObject())
        {
            switch (state.Value)
            {
                case State.idle:
                    OnIdle?.Invoke(this, EventArgs.Empty);
                    break;

                case State.frying:

                    fryingTimer.Value += Time.deltaTime;

                    Onfrying?.Invoke(this, EventArgs.Empty);                 

                    if (fryingTimer.Value > fryingObjectSO.fryTimerMax)
                    {                       
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(fryingObjectSO.output, this);

                        Debug.Log("FRIED");
                        state.Value = State.fried;
                        burnedTimer.Value = 0;

                        StoveInteractObjectStoveClientRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
                    }
                    break;

                case State.fried:
                    burnedTimer.Value += Time.deltaTime;
                    OnFried?.Invoke(this, EventArgs.Empty);

                    

                    if (burnedTimer.Value > burningRecipeSO.burnedTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state.Value = State.burned;                    
                    }
                    break;

                case State.burned:
                    OnBurned?.Invoke(this, EventArgs.Empty);
                    break;
            }

            Debug.Log(state); 
        }
    }

    public override void Interact(Player player)
    {
        {
            {
                if (!HasKitchenObject())
                {
                    if (player.HasKitchenObject())
                    {
                        if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenObject kitchenObject = player.GetKitchenObject();

                            kitchenObject.SetKitchenObjectParent(this);

                            InteractObjectStoveServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));


                        }
                    }

                    else
                    {

                    }
                }

                else
                {
                    if (player.HasKitchenObject())
                    {
                        if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                        {

                            if (plateKitchenObject.TryAddIngridient(GetKitchenObject().GetKitchenObjectSO()))
                            {
                                KitchenObject.DestroyKitchenObject(GetKitchenObject());

                                stateIdleValueServerRpc();
                            }
                        }


                    }

                    else
                    {
                        GetKitchenObject().SetKitchenObjectParent(player);
                        stateIdleValueServerRpc();
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void stateIdleValueServerRpc()
    {
        state.Value = State.idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractObjectStoveServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0f;
        state.Value = State.frying;
        FryingInteractObjectStoveClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void FryingInteractObjectStoveClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingObjectSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }

    [ClientRpc]
    private void StoveInteractObjectStoveClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    public bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
       FryingObjectSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingObjectSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }

        else
        {
            return null;
        }
    }

    private  FryingObjectSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {     
            foreach (FryingObjectSO fryingRecipeSO in fryingRecipeSOArray)
            {
                if (fryingRecipeSO.input == inputKitchenObjectSO)
                {
                    return fryingRecipeSO;
                }

            }
            return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO fryingRecipeSO in burnedRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }

        }
        return null;
    }

    public bool IsFried()
    {
        return state.Value == State.fried;
    }
}
