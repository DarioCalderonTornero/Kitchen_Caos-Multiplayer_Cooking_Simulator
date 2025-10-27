using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemove;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    private float spawnPlatesTimer;
    private float spawnPlatesTimeMax = 4f;

    private float spawnPlatesAmount;

    private int spawnPlatesAmountMax = 4;

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        spawnPlatesTimer += Time.deltaTime;
        if (spawnPlatesTimer > spawnPlatesTimeMax)
        {
            spawnPlatesTimer = 0;

            if (KitchenGameManager.Instance.isPlayingGame() && spawnPlatesAmount < spawnPlatesAmountMax)
            {
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        spawnPlatesAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (spawnPlatesAmount > 0)
            {
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                InteractLogicServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        spawnPlatesAmount--;
        OnPlateRemove?.Invoke(this, EventArgs.Empty);

    }


}


