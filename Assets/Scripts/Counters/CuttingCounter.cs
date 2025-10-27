using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventsArgs> OnProgressChanged;
    public event EventHandler OnCanCut;
    public event EventHandler OnCutFinished;

    public event EventHandler onCut;
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;    
    }

    [SerializeField] public CuttingObjectsSO[] cuttingObjectSOArray;

    public int cuttingProgress;

    public override void Interact(Player player)
    {
        {
            if (!HasKitchenObject())
            {
                if (player.HasKitchenObject())
                {
                    if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject kitchenObject = player.GetKitchenObject();

                        player.GetKitchenObject().SetKitchenObjectParent(this);

                        ResetCutProgressServerRpc();
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
                            OnCanCut?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }

                else
                {
                    GetKitchenObject().SetKitchenObjectParent(player);
                    
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetCutProgressServerRpc()
    {
        ResetProgressClientRpc();
    }

    [ClientRpc]
    private void ResetProgressClientRpc()
    {
        cuttingProgress = 0;


        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventsArgs
        {
            progressNormalized = 0
        });

    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            InteractAlternateServerRpc();
            cuttingProgressDoneOnceServerRpc();   
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractAlternateServerRpc()
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            InteractAlternateClientRpc();
        }
    }

    [ClientRpc]
    private void InteractAlternateClientRpc()
    {
        cuttingProgress++;

        onCut?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        CuttingObjectsSO cuttingObjectSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventsArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingObjectSO.maxCutProgress
        });

    }

    [ServerRpc(RequireOwnership = false)]
    private void cuttingProgressDoneOnceServerRpc()
    {

        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CuttingObjectsSO cuttingObjectSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());


            if (cuttingProgress >= cuttingObjectSO.maxCutProgress)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                KitchenObject.DestroyKitchenObject(GetKitchenObject());

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
            
    }

    public bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingObjectsSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingObjectsSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            OnCutFinished?.Invoke(this, EventArgs.Empty);
            return cuttingRecipeSO.output;
        }

        else
        {
            return null;
        }

        
    }   

    public CuttingObjectsSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        {
            foreach (CuttingObjectsSO cuttingObjectSO in cuttingObjectSOArray)
            {
                if (cuttingObjectSO.input == inputKitchenObjectSO)
                {
                    return cuttingObjectSO;
                }

            }

            return null;
        }
    }
  
}
