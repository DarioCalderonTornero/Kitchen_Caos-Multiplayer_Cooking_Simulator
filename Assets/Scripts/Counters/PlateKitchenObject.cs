using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler <OnIngredientAddedEventArgs> OnIngredientAdded;
    public static event EventHandler OnIngredientAdd;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    private List<KitchenObjectSO> kitchenObjectSOList;
    [SerializeField] public List<KitchenObjectSO> validIngredientsSOList;

    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();  
    }

    public bool TryAddIngridient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validIngredientsSOList.Contains(kitchenObjectSO))
        {
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {           
            return false;
        }

        else
        {
            AddIngridientServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
            return true;
        }
       
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngridientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngridientClientRpc(kitchenObjectSOIndex); 
    }

    [ClientRpc]
    private void AddIngridientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex); 

        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdd?.Invoke(this, EventArgs.Empty);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetKitchenSOList()
    {
        return kitchenObjectSOList;
    }

    public bool IsValidIngredient(KitchenObjectSO kitchenObjectSO)
    {
        return validIngredientsSOList.Contains(kitchenObjectSO);
    }
}

    
