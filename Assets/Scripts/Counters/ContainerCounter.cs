using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] KitchenObjectSO kitchenObjectSO;
    public event EventHandler OnObjectPicked;

    public override void Interact(Player player)
    {      
        if(!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            OnObjectPicked?.Invoke(this, EventArgs.Empty);  
        }
          
    }
 
}
